using UnityEngine;
using UnityEngine.Rendering;
using FFmpegOut;
using System.Collections;
using Sirenix.OdinInspector;

public class ScreenRecorder : MonoBehaviour
{
	public FFmpegPreset Preset = FFmpegPreset.H264Default;
	public int FrameRate = 30;

	CommandBuffer _commandBuffer;
	RenderTexture _renderTexture;
	Camera _captureCamera;
	FFmpegSession _ffmpegSession;
	YieldInstruction _waitForEndOfFrame;
	Coroutine _captueScreenCoroutine;

	float _timeBetweenFrames;
	float _nextFrameTime;

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
		_ffmpegSession = FFmpegSession.Create(sessionName, Screen.width, Screen.height, FrameRate, Preset);
		Debug.Log("FFmpeg session started: " + sessionName);

		_timeBetweenFrames = 1.0f / FrameRate;
		_waitForEndOfFrame = new WaitForEndOfFrame();
		_nextFrameTime = Time.time;
		_commandBuffer = new CommandBuffer { name = "Capture Screen Command Buffer" };
	}

	[Button]
	public void StartRecording()
	{
		_captueScreenCoroutine = StartCoroutine(Capture());
	}

	[Button]
	public void StopRecording()
	{
		if (_captueScreenCoroutine != null)
		{
			StopCoroutine(_captueScreenCoroutine);
			_captueScreenCoroutine = null;
			Dispose();
		}
	}

	void FreeRenderResources()
	{
		if (_commandBuffer != null)
		{
			_commandBuffer.Release();
			_commandBuffer = null;
		}

		if (_renderTexture)
		{
			RenderTexture.ReleaseTemporary(_renderTexture);
			_renderTexture = null;
		}
	}

	void Dispose()
	{
		if (_ffmpegSession != null)
		{
			_ffmpegSession.Dispose();
			_ffmpegSession = null;
		}

		FreeRenderResources();
	}

	IEnumerator Capture()
	{
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
