using UnityEngine;
using UnityEngine.Rendering;
using FFmpegOut;
using System.Collections;
using System.IO;
using Sirenix.OdinInspector;

public class ScreenRecorder : MonoBehaviour
{
	public FFmpegPreset Preset = FFmpegPreset.H264Default;
	public int FrameRate = 30;

	[SerializeField] AudioRecorder _audioRecorder;

	CommandBuffer _commandBuffer;
	RenderTexture _renderTexture;
	Camera _captureCamera;
	FFmpegSession _ffmpegSession;
	YieldInstruction _waitForEndOfFrame;
	Coroutine _captureScreenCoroutine;

	float _timeBetweenFrames;
	float _nextFrameTime;
	string _videoOutputPath;
	string _audioOutputPath;

	void Start()
	{
		SetupRecording();
	}

	void SetupRecording()
	{
		_captureCamera = Camera.main;
		if (_captureCamera == null)
		{
			Debug.LogError("No main camera found for screen recording.");
			return;
		}

		var sessionName = "ScreenCapture_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
		_videoOutputPath = Path.Combine(Application.persistentDataPath, sessionName + ".mp4");
		_audioOutputPath = Path.Combine(Application.persistentDataPath, sessionName + ".wav");

		_ffmpegSession = FFmpegSession.CreateWithOutputPath(_videoOutputPath, Screen.width, Screen.height, FrameRate, Preset);
		Debug.Log("FFmpeg session started: " + _videoOutputPath);

		_timeBetweenFrames = 1.0f / FrameRate;
		_waitForEndOfFrame = new WaitForEndOfFrame();
		_commandBuffer = new CommandBuffer { name = "Capture Screen Command Buffer" };
	}

	[Button]
	public void StartRecording()
	{
		if (_captureScreenCoroutine == null)
		{
			_captureScreenCoroutine = StartCoroutine(Capture());
			_audioRecorder.StartRecording(_audioOutputPath);
		}
	}

	[Button]
	public void StopRecording()
	{
		if (_captureScreenCoroutine != null)
		{
			StopCoroutine(_captureScreenCoroutine);
			_captureScreenCoroutine = null;
			_audioRecorder.StopRecording();
			Dispose();
		}
	}

	void FreeRenderResources()
	{
		_commandBuffer?.Release();
		_commandBuffer = null;

		if (_renderTexture)
		{
			RenderTexture.ReleaseTemporary(_renderTexture);
			_renderTexture = null;
		}
	}

	void Dispose()
	{
		_ffmpegSession?.Dispose();
		_ffmpegSession = null;

		FreeRenderResources();
	}

	IEnumerator Capture()
	{
		_nextFrameTime = Time.time;
		while (true)
		{
			yield return _waitForEndOfFrame;

			if (Time.time >= _nextFrameTime)
			{
				if (_renderTexture)
				{
					RenderTexture.ReleaseTemporary(_renderTexture);
					_renderTexture = null;
				}

				_renderTexture = RenderTexture.GetTemporary(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB, 1);
				_renderTexture.Create();

				_commandBuffer.Clear();
				_commandBuffer.Blit(BuiltinRenderTextureType.CurrentActive, _renderTexture);

				Graphics.ExecuteCommandBuffer(_commandBuffer);

				_ffmpegSession.PushFrame(_renderTexture);
				_ffmpegSession.CompletePushFrames();

				_nextFrameTime += _timeBetweenFrames;
			}
			else
			{
				_ffmpegSession.PushFrame(null);
			}
		}
	}

	void OnDisable()
	{
		Dispose();
	}
}
