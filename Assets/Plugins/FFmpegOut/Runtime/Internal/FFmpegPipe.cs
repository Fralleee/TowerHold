using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Unity.Collections;
using UnityEngine;

namespace FFmpegOut
{
	public sealed class FFmpegPipe : System.IDisposable
	{
		public static bool IsAvailable => File.Exists(ExecutablePath);

		Process _subprocess;
		Thread _copyThread;
		Thread _pipeThread;
		readonly AutoResetEvent _copyPing = new AutoResetEvent(false);
		readonly AutoResetEvent _copyPong = new AutoResetEvent(false);
		readonly AutoResetEvent _pipePing = new AutoResetEvent(false);
		readonly AutoResetEvent _pipePong = new AutoResetEvent(false);
		bool _terminate;
		Queue<NativeArray<byte>> _copyQueue = new Queue<NativeArray<byte>>();
		Queue<byte[]> _pipeQueue = new Queue<byte[]>();
		Queue<byte[]> _freeBuffer = new Queue<byte[]>();

		public static string ExecutablePath
		{
			get
			{
				var basePath = Application.streamingAssetsPath;
				var platform = Application.platform;

				if (platform is RuntimePlatform.OSXPlayer or RuntimePlatform.OSXEditor)
					return Path.Combine(basePath, "FFmpegOut/macOS/ffmpeg");

				if (platform is RuntimePlatform.LinuxPlayer or RuntimePlatform.LinuxEditor)
					return Path.Combine(basePath, "FFmpegOut/Linux/ffmpeg");

				return Path.Combine(basePath, "FFmpegOut/Windows/ffmpeg.exe");
			}
		}

		public FFmpegPipe(string arguments)
		{
			try
			{
				_subprocess = Process.Start(new ProcessStartInfo
				{
					FileName = ExecutablePath,
					Arguments = arguments,
					UseShellExecute = false,
					CreateNoWindow = true,
					RedirectStandardInput = true,
					RedirectStandardOutput = true,
					RedirectStandardError = true
				});

				_copyThread = new Thread(CopyThread);
				_pipeThread = new Thread(PipeThread);
				_copyThread.Start();
				_pipeThread.Start();
			}
			catch (System.Exception ex)
			{
				UnityEngine.Debug.LogError($"Failed to start FFmpeg process: {ex.Message}");
			}
		}

		public void PushFrameData(NativeArray<byte> data)
		{
			lock (_copyQueue)
			{
				_copyQueue.Enqueue(data);
				_copyPing.Set();
			}
		}

		public void SyncFrameData()
		{
			while (_copyQueue.Count > 0)
			{
				_copyPong.WaitOne();
			}

			while (_pipeQueue.Count > 4)
			{
				_pipePong.WaitOne();
			}
		}

		public string CloseAndGetOutput()
		{
			_terminate = true;
			_copyPing.Set();
			_pipePing.Set();
			_copyThread.Join();
			_pipeThread.Join();

			_subprocess.StandardInput.Close();
			_subprocess.WaitForExit();

			var error = _subprocess.StandardError.ReadToEnd();
			_subprocess.Close();
			_subprocess.Dispose();

			_subprocess = null;
			_copyThread = null;
			_pipeThread = null;
			_copyQueue = null;
			_pipeQueue = _freeBuffer = null;

			return error;
		}

		public void Dispose()
		{
			if (!_terminate)
			{
				CloseAndGetOutput();
			}
		}

		~FFmpegPipe()
		{
			if (!_terminate)
			{
				UnityEngine.Debug.LogError("An unfinalized FFmpegPipe object was detected. It should be explicitly closed or disposed before being garbage-collected.");
			}
		}

		void CopyThread()
		{
			while (!_terminate)
			{
				_copyPing.WaitOne();

				while (_copyQueue.Count > 0)
				{
					NativeArray<byte> source;
					lock (_copyQueue)
					{
						source = _copyQueue.Peek();
					}

					byte[] buffer = null;
					if (_freeBuffer.Count > 0)
					{
						lock (_freeBuffer)
						{
							buffer = _freeBuffer.Dequeue();
						}
					}

					if (buffer == null || buffer.Length != source.Length)
					{
						buffer = new byte[source.Length];
					}

					source.CopyTo(buffer);

					lock (_pipeQueue)
					{
						_pipeQueue.Enqueue(buffer);
					}

					_pipePing.Set();

					lock (_copyQueue)
					{
						_copyQueue.Dequeue();
					}

					_copyPong.Set();
				}
			}
		}

		void PipeThread()
		{
			var pipe = _subprocess.StandardInput.BaseStream;

			while (!_terminate)
			{
				_pipePing.WaitOne();

				while (_pipeQueue.Count > 0)
				{
					byte[] buffer;
					lock (_pipeQueue)
					{
						buffer = _pipeQueue.Dequeue();
					}

					try
					{
						pipe.Write(buffer, 0, buffer.Length);
						pipe.Flush();
					}
					catch (System.Exception ex)
					{
						UnityEngine.Debug.LogError($"Error writing to FFmpeg pipe: {ex.Message}");
					}

					lock (_freeBuffer)
					{
						_freeBuffer.Enqueue(buffer);
					}

					_pipePong.Set();
				}
			}
		}
	}
}
