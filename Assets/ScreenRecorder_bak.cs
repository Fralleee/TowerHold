using UnityEngine;
using UnityEngine.Rendering;
using FFmpegOut;
using System.Collections;
using Sirenix.OdinInspector;

public class ScreenRecorderBAK : MonoBehaviour
{
	public FFmpegPreset Preset = FFmpegPreset.H264Default;
	public int FrameRate = 30;

	CommandBuffer _commandBuffer;
	RenderTexture _renderTexture;
	Camera _captureCamera;
	FFmpegSession _ffmpegSession;
	YieldInstruction _waitForEndOfFrame;
	YieldInstruction _waitForSeconds;
	Coroutine _captueScreenCoroutine;

	float _timeBetweenFrames;

	void Start()
	{
		_captureCamera = Camera.main;
		if (_captureCamera == null)
		{
			Debug.LogError("No main camera found for screen recording.");
			return;
		}

		_timeBetweenFrames = 1.0f / FrameRate;

		_waitForEndOfFrame = new WaitForEndOfFrame();
		_waitForSeconds = new WaitForSeconds(_timeBetweenFrames);
		Application.runInBackground = true;
	}

	[Button]
	public void StartRecording()
	{
		var sessionName = "ScreenCapture_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
		_ffmpegSession = FFmpegSession.Create(sessionName, Screen.width, Screen.height, FrameRate, Preset);
		Debug.Log("FFmpeg session started: " + sessionName);

		_commandBuffer = new CommandBuffer { name = "Capture Screen Command Buffer" };
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
		// Command buffer
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
			_ffmpegSession.Close();
			_ffmpegSession.Dispose();
			_ffmpegSession = null;
		}

		FreeRenderResources();
	}

	IEnumerator Capture()
	{
		while (true)
		{
			// yield return _waitForSeconds;
			yield return _waitForEndOfFrame;

			// Prepare resources for new frame capture
			if (_renderTexture != null)
			{
				RenderTexture.ReleaseTemporary(_renderTexture);
			}
			_renderTexture = RenderTexture.GetTemporary(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB, 1);

			_commandBuffer.Clear();

			// Capture the screen into the render texture
			_commandBuffer.Blit(BuiltinRenderTextureType.CurrentActive, _renderTexture);
			Graphics.ExecuteCommandBuffer(_commandBuffer);
			// _commandBuffer.Release();

			// Push the frame to the FFmpeg session
			_ffmpegSession.PushFrame(_renderTexture);
			_ffmpegSession.CompletePushFrames();

			// Wait for the next frame time before capturing again
			yield return new WaitForSeconds(_timeBetweenFrames);
		}
	}

	void OnDisable()
	{
		Dispose();
	}
}
