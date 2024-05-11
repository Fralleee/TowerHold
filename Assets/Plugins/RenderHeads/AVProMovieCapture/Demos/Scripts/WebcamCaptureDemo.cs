
using UnityEngine;
using System.Collections;

//-----------------------------------------------------------------------------
// Copyright 2012-2022 RenderHeads Ltd.  All rights reserved.
//-----------------------------------------------------------------------------

namespace RenderHeads.Media.AVProMovieCapture.Demos
{
	/// <summary>
	/// Allows the user to select from a list of webcams and creates a capture instance for the webcam recording.
	/// Currently only a single webcam can be captured at once.
	/// </summary>
	public class WebcamCaptureDemo : MonoBehaviour
	{
		#pragma warning disable 0414	// x is assigned but its value is never used
		[SerializeField] GUISkin _skin = null;
		[SerializeField] GameObject _prefab = null;
		[SerializeField] int _webcamResolutionWidth = 640;
		[SerializeField] int _webcamResolutionHeight = 480;
		[SerializeField] int _webcamFrameRate = 30;
		#pragma warning restore 0414
		
#if AVPRO_MOVIECAPTURE_WEBCAMTEXTURE_SUPPORT

		private class Instance
		{
			public string name;
			public WebCamTexture texture;
			public CaptureFromWebCamTexture capture;
			public CaptureGUI gui;
		}

		// State
		private Instance[] _instances;
		private int _selectedWebcamIndex = -1;

		private IEnumerator Start()
		{
			Application.targetFrameRate = 60;

			// Make sure we're authorised for using the camera. On iOS the OS will forcibly
			// close the application if authorisation has not been granted. Make sure the
			// "Camera Usage Description" field has been filled in the player settings.
			// This needs to be done first otherwise no cameras will be reported.
			if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
			{
				yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
			}

			// Make sure we're authorised for using the microphone. On iOS the OS will forcibly
			// close the application if authorisation has not been granted. Make sure the
			// "Microphone Usage Description" field has been filled in the player settings.
			// if (_capture.AudioCaptureSource == AudioCaptureSource.Microphone)
			{
				if (!Application.HasUserAuthorization(UserAuthorization.Microphone))
				{
					yield return Application.RequestUserAuthorization(UserAuthorization.Microphone);
				}
				if (Application.HasUserAuthorization(UserAuthorization.Microphone))
				{
					// On iOS modified the audio session to allow recording from the microphone.
					NativePlugin.SetMicrophoneRecordingHint(true);
				}
			}

			// Create instance data per webcam
			int numCams = WebCamTexture.devices.Length;
			_instances = new Instance[numCams];
			for (int i = 0; i < numCams; i++)
			{
				GameObject go = (GameObject)GameObject.Instantiate(_prefab);

				Instance instance = new Instance();
				instance.name = WebCamTexture.devices[i].name;
				instance.capture = go.GetComponent<CaptureFromWebCamTexture>();
				instance.capture.FilenamePrefix = "Demo4Webcam-" + i;
				instance.gui = go.GetComponent<CaptureGUI>();
				instance.gui.ShowUI = false;

				_instances[i] = instance;

#if false
				WebCamDevice device = WebCamTexture.devices[i];
				Resolution[] resolutions = device.availableResolutions;
				if (resolutions != null)
				{
					Debug.Log($"Device '{device.name}' has {resolutions.Length} supported resolutions:");
					foreach (Resolution resolution in resolutions)
					{
						Debug.Log($"{resolution.width}x{resolution.height}@{resolution.refreshRate}");
					}
				}
#endif
			}

			if (numCams > 0)
			{
				SelectWebcam(0);
			}
		}

		private void StartWebcam(Instance instance)
		{
			// NOTE: WebcamTexture can be slow for high resolutions, this can cause issues with audio-video sync.
			// Our plugins AVPro Live Camera or AVPro DeckLink can be used to capture high resolution devices
			Debug.LogFormat("_webcamResolutionWidth: {0}, _webcamResolutionHeight: {1}, _webcamFrameRate: {2}", _webcamResolutionWidth, _webcamResolutionHeight, _webcamFrameRate);
			instance.texture = new WebCamTexture(instance.name);
			instance.texture.requestedWidth = _webcamResolutionWidth;
			instance.texture.requestedHeight = _webcamResolutionHeight;
			instance.texture.requestedFPS = _webcamFrameRate;
			instance.texture.Play();
			if (instance.texture.isPlaying)
			{
				instance.capture.WebCamTexture = instance.texture;
			}
			else
			{
				Debug.Log(string.Format("Unable to start webcam in mode {0}x{1}@{2}", _webcamResolutionWidth, _webcamResolutionHeight, _webcamFrameRate));
				StopWebcam(instance);
			}
		}

		private void StopWebcam(Instance instance)
		{
			if (instance.texture != null)
			{
				if (instance.capture != null && instance.capture.IsCapturing())
				{
					instance.capture.WebCamTexture = null;
					instance.capture.StopCapture();
				}

				instance.texture.Stop();
				Destroy(instance.texture);
				instance.texture = null;
			}

			_selectedWebcamIndex = -1;
		}

		private void OnDestroy()
		{
			for (int i = 0; i < _instances.Length; i++)
			{
				StopWebcam(_instances[i]);
			}
		}

		private void SelectWebcam(int index)
		{
			// Stop any currently
			if (_selectedWebcamIndex >= 0)
			{
				StopWebcam(_instances[_selectedWebcamIndex]);
				_selectedWebcamIndex = -1;
			}

			if (index >= 0)
			{
				_selectedWebcamIndex = index;
				for (int j = 0; j < _instances.Length; j++)
				{
					_instances[j].gui.ShowUI = (j == _selectedWebcamIndex);
				}
				StartWebcam(_instances[_selectedWebcamIndex]);
			}
		}

		private void OnGUI()
		{
#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
			float sf = 2.0f;
#else
			float sf = 1.0f;
#endif
			GUI.matrix = Matrix4x4.Scale(new Vector3(sf, sf, 1f));
			float hw = Screen.width / (2f * sf);

			GUI.skin = _skin;
			GUILayout.BeginArea(new Rect(hw, 0, hw, Screen.height));
			GUILayout.BeginVertical();

			GUILayout.Label("Select webcam:");

			for (int i = 0; i < _instances.Length; i++)
			{
				Instance webcam = _instances[i];

				GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));

				if (webcam.capture.IsCapturing())
				{
					float t = Mathf.PingPong(Time.timeSinceLevelLoad, 0.25f) * 4f;
					GUI.backgroundColor = Color.Lerp(GUI.backgroundColor, Color.white, t);
					GUI.color = Color.Lerp(Color.red, Color.white, t);
				}

				if (_selectedWebcamIndex == i)
				{
					GUI.backgroundColor = Color.green;
				}

				if (GUILayout.Button(webcam.name, GUILayout.Width(hw * 0.5f), GUILayout.ExpandWidth(false)))
				{
					if (_selectedWebcamIndex != i)
					{
						SelectWebcam(i);
					}
					else
					{
						StopWebcam(webcam);
					}
				}

				GUI.backgroundColor = Color.white;
				GUI.color = Color.white;

				if (webcam.texture != null)
				{
					float a = (float)_webcamResolutionHeight / (float)_webcamResolutionWidth;
					float w = (float)hw * 0.5f;
					Rect camRect = GUILayoutUtility.GetRect(w, w * a);
					GUI.DrawTexture(camRect, webcam.texture);
				}
				else
				{
					GUILayout.Label(string.Empty, GUILayout.MinWidth(256.0f), GUILayout.MaxWidth(256.0f), GUILayout.ExpandWidth(false));
				}

				GUILayout.EndHorizontal();
			}

			if (_selectedWebcamIndex >= 0 && _selectedWebcamIndex < _instances.Length)
			{
				Instance instance = _instances[_selectedWebcamIndex];
				GUILayout.Label($"WebCam FPS: {instance.capture.WebCamFPS}");
			}

			GUILayout.EndVertical();
			GUILayout.EndArea();
		}
#else
				void Start()
		{
			Debug.LogError("[AVProMovieCapture] To use WebCamTexture capture component/demo you must add the string AVPRO_MOVIECAPTURE_WEBCAMTEXTURE_SUPPORT must be added to `Scriping Define Symbols` in `Player Settings > Other Settings > Script Compilation`");
		}
#endif  // AVPRO_MOVIECAPTURE_WEBCAMTEXTURE_SUPPORT
	}
}
