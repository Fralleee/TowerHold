using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;
using System.Diagnostics;

namespace FFmpegOut
{
	public sealed class FFmpegSession : System.IDisposable
	{
		FFmpegPipe _pipe;
		Material _blitMaterial;
		readonly List<AsyncGPUReadbackRequest> _readbackQueue = new List<AsyncGPUReadbackRequest>(4);

		public static FFmpegSession Create(string name, int width, int height, float frameRate, FFmpegPreset preset)
		{
			name += System.DateTime.Now.ToString(" yyyy MMdd HHmmss");
			var path = name.Replace(" ", "_") + preset.GetSuffix();
			return CreateWithOutputPath(path, width, height, frameRate, preset);
		}

		public static FFmpegSession CreateWithOutputPath(string outputPath, int width, int height, float frameRate, FFmpegPreset preset)
		{
			return new FFmpegSession(
				$"-y -f rawvideo -vcodec rawvideo -pixel_format rgba -colorspace bt709 -video_size {width}x{height} -framerate {frameRate} -loglevel warning -i - {preset.GetOptions()} \"{outputPath}\""
			);
		}

		public static FFmpegSession CreateWithArguments(string arguments)
		{
			return new FFmpegSession(arguments);
		}

		FFmpegSession(string arguments)
		{
			if (!FFmpegPipe.IsAvailable)
			{
				UnityEngine.Debug.LogWarning("Failed to initialize an FFmpeg session due to missing executable file. Please check FFmpeg installation.");
				return;
			}

			if (!SystemInfo.supportsAsyncGPUReadback)
			{
				UnityEngine.Debug.LogWarning("Failed to initialize an FFmpeg session due to lack of async GPU readback support. Please try changing graphics API to readback-enabled one.");
				return;
			}

			_pipe = new FFmpegPipe(arguments);
			InitializeMaterials();
		}

		void InitializeMaterials()
		{
			var shader = Shader.Find("Hidden/FFmpegOut/Preprocess");
			if (shader != null)
			{
				_blitMaterial = new Material(shader);
			}
			else
			{
				UnityEngine.Debug.LogError("Shader 'Hidden/FFmpegOut/Preprocess' not found. Check your resources.");
			}
		}

		public void Dispose()
		{
			Close();
		}

		public void Close()
		{
			if (_pipe != null)
			{
				var error = _pipe.CloseAndGetOutput();
				if (!string.IsNullOrEmpty(error))
				{
					UnityEngine.Debug.LogWarning($"FFmpeg returned with warning/error messages:\n{error}");
				}
				_pipe.Dispose();
				_pipe = null;
			}

			if (_blitMaterial != null)
			{
				Object.Destroy(_blitMaterial);
				_blitMaterial = null;
			}
		}

		public void PushFrame(RenderTexture source)
		{
			if (_pipe == null)
			{
				UnityEngine.Debug.LogWarning("FFmpeg pipe is not initialized.");
				return;
			}

			ProcessQueue();
			if (source != null)
			{
				QueueFrame(source);
			}
		}

		public void CompletePushFrames()
		{
			_pipe?.SyncFrameData();
		}

		void QueueFrame(RenderTexture source)
		{
			if (_readbackQueue.Count > 6)
			{
				UnityEngine.Debug.LogWarning("Too many GPU readback requests.");
				return;
			}

			_readbackQueue.Add(AsyncGPUReadback.Request(source));
		}

		void ProcessQueue()
		{
			while (_readbackQueue.Count > 0)
			{
				var req = _readbackQueue[0];
				if (!req.done)
				{
					if (_readbackQueue.Count > 1 && _readbackQueue[1].done)
					{
						_readbackQueue[0].WaitForCompletion();
					}
					else
					{
						break;
					}
				}

				_readbackQueue.RemoveAt(0);
				if (req.hasError)
				{
					UnityEngine.Debug.LogWarning("GPU readback error was detected.");
					continue;
				}

				_pipe.PushFrameData(req.GetData<byte>());
			}
		}

		public void CombineAudioAndVideo(string videoPath, string audioPath, string outputPath)
		{
			var process = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = "ffmpeg",
					Arguments = $"-i \"{videoPath}\" -i \"{audioPath}\" -c:v copy -c:a aac -strict experimental \"{outputPath}\"",
					RedirectStandardOutput = true,
					UseShellExecute = false,
					CreateNoWindow = true
				}
			};

			process.Start();
			process.WaitForExit();

			if (process.ExitCode == 0)
			{
				UnityEngine.Debug.Log($"Successfully combined audio and video into {outputPath}");
			}
			else
			{
				UnityEngine.Debug.LogError("Failed to combine audio and video");
			}
		}
	}
}
