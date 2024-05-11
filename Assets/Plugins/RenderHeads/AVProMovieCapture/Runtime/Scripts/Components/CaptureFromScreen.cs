using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;

//-----------------------------------------------------------------------------
// Copyright 2012-2022 RenderHeads Ltd.  All rights reserved.
//-----------------------------------------------------------------------------

namespace RenderHeads.Media.AVProMovieCapture
{
	/// <summary>
	/// Capture from the screen (backbuffer).  Everything is captured as it appears on the screen, including IMGUI rendering.
	/// This component waits for the frame to be completely rendered and then captures it.
	/// </summary>
	[AddComponentMenu("AVPro Movie Capture/Capture From Screen", 0)]
	public class CaptureFromScreen : CaptureBase
	{
		//private const int NewFrameSleepTimeMs = 6;
		[SerializeField] bool _captureMouseCursor = false;
		[SerializeField] MouseCursor _mouseCursor = null;

		private System.IntPtr _targetNativePointer = System.IntPtr.Zero;
		private RenderTexture _resolveTexture = null;
		private CommandBuffer _commandBuffer = null;

#if !UNITY_EDITOR && UNITY_ANDROID && AVPMC_ANDROID_VULKAN_PRETRANSFORM
		private bool _hasBeenWarnedAboutVulkanPreTransform = false;
#endif

		public bool CaptureMouseCursor
		{
			get { return _captureMouseCursor; }
			set { _captureMouseCursor = value; }
		}

		public MouseCursor MouseCursor
		{
			get { return _mouseCursor; }
			set { _mouseCursor = value; }
		}

		public override bool PrepareCapture()
		{
			if (_capturing)
			{
				return false;
			}
#if UNITY_EDITOR_WIN || (!UNITY_EDITOR && UNITY_STANDALONE_WIN)
			if (SystemInfo.graphicsDeviceVersion.StartsWith("Direct3D 9"))
			{
				Debug.LogError("[AVProMovieCapture] Direct3D9 not yet supported, please use Direct3D11 instead.");
				return false;
			}
			else if (SystemInfo.graphicsDeviceVersion.StartsWith("OpenGL") && !SystemInfo.graphicsDeviceVersion.Contains("emulated"))
			{
				Debug.LogError("[AVProMovieCapture] OpenGL not yet supported for CaptureFromScreen component, please use Direct3D11 instead. You may need to switch your build platform to Windows.");
				return false;
			}
#endif

			if (_mouseCursor != null)
			{
				_mouseCursor.enabled = _captureMouseCursor;
			}

#if UNITY_EDITOR
			if (Display.displays.Length > 1)
			{
				bool isSecondDisplayActive = false;
				for (int i = 1; i < Display.displays.Length; i++)
				{
					if (Display.displays[i].active)
					{
						isSecondDisplayActive = true;
						break;
					}
				}
				if (isSecondDisplayActive)
				{
					Debug.LogError("[AVProMovieCapture] CaptureFromScreen doesn't work correctly (can cause stretching or incorrect display capture) when there are multiple displays are active.  Use CaptureFromCamera instead.");
				}				
			}
#endif

			int width = Screen.width;
			int height = Screen.height;

			_Transparency = Transparency.None;
			if ( !NativePlugin.IsBasicEdition() )
			{
				if( (_outputTarget == OutputTarget.VideoFile || _outputTarget == OutputTarget.NamedPipe) && GetEncoderHints().videoHints.transparency != Transparency.None )
				{
					_Transparency = GetEncoderHints().videoHints.transparency;
				}
				else if( _outputTarget == OutputTarget.ImageSequence && GetEncoderHints().imageHints.transparency != Transparency.None )
				{
					_Transparency = GetEncoderHints().imageHints.transparency;
				}

				switch( _Transparency )
				{
					case Transparency.TopBottom:	height *= 2;	break;
					case Transparency.LeftRight:	width *= 2;		break;
				}
			}

			SelectRecordingResolution(width, height);

			_pixelFormat = NativePlugin.PixelFormat.RGBA32;

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
			if (SystemInfo.graphicsDeviceVersion.StartsWith("OpenGL") && !SystemInfo.graphicsDeviceVersion.Contains("emulated"))
			{
				// TODO: add this back in once we have fixed opengl support
				_pixelFormat = NativePlugin.PixelFormat.BGRA32;
				_isTopDown = true;
			}
			else
			{
				_isTopDown = false;
			}
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX || (!UNITY_EDITOR && UNITY_IOS)
			_isTopDown = false;
#elif !UNITY_EDITOR && UNITY_ANDROID
	#if UNITY_2022_1_OR_NEWER
			bool isVulkan = SystemInfo.graphicsDeviceType == GraphicsDeviceType.Vulkan;
			_isTopDown = isVulkan;
	#else
			_isTopDown = false;
			#if AVPMC_ANDROID_VULKAN_PRETRANSFORM
				if (_Transparency == Transparency.None || _Transparency == Transparency.Codec)
				{
					switch (Screen.orientation)
					{
						case ScreenOrientation.Portrait:
							Debug.Log("Android Vulkan PreTransform is enabled, screen orientation is Portrait");
							GetEncoderHints().videoHints.androidVulkanPreTransform = AndroidVulkanPreTransform.Portrait;
							GetEncoderHints().imageHints.androidVulkanPreTransform = AndroidVulkanPreTransform.Portrait;
							break;

						case ScreenOrientation.PortraitUpsideDown:
							Debug.Log("Android Vulkan PreTransform is enabled, screen orientation is PortraitUpsideDown");
							GetEncoderHints().videoHints.androidVulkanPreTransform = AndroidVulkanPreTransform.PortraitUpsideDown;
							GetEncoderHints().imageHints.androidVulkanPreTransform = AndroidVulkanPreTransform.PortraitUpsideDown;
							break;

						case ScreenOrientation.LandscapeLeft:
							Debug.Log("Android Vulkan PreTransform is enabled, screen orientation is LandscapeLeft");
							GetEncoderHints().videoHints.androidVulkanPreTransform = AndroidVulkanPreTransform.LandscapeLeft;
							GetEncoderHints().imageHints.androidVulkanPreTransform = AndroidVulkanPreTransform.LandscapeLeft;
							break;

						case ScreenOrientation.LandscapeRight:
							Debug.Log("Android Vulkan PreTransform is enabled, screen orientation is LandscapeRight");
							GetEncoderHints().videoHints.androidVulkanPreTransform = AndroidVulkanPreTransform.LandscapeRight;
							GetEncoderHints().imageHints.androidVulkanPreTransform = AndroidVulkanPreTransform.LandscapeRight;
							break;

						case ScreenOrientation.AutoRotation:
							// Best guess based on screen resolution
							if (Screen.width > Screen.height)
							{
								Debug.Log("Android Vulkan PreTransform is enabled, screen resolution is Landscape");
								GetEncoderHints().videoHints.androidVulkanPreTransform = AndroidVulkanPreTransform.LandscapeRight;
								GetEncoderHints().imageHints.androidVulkanPreTransform = AndroidVulkanPreTransform.LandscapeRight;
							}
							else
							{
								Debug.Log("Android Vulkan PreTransform is enabled, screen resolution is Portrait");
								GetEncoderHints().videoHints.androidVulkanPreTransform = AndroidVulkanPreTransform.Portrait;
								GetEncoderHints().imageHints.androidVulkanPreTransform = AndroidVulkanPreTransform.Portrait;
							}
							break;
					}
				}
			#endif
	#endif
#endif
			GenerateFilename();

			// Kick-off the final render capture coroutine
			_finalRenderCapture = FinalRenderCapture();
			_doFinalRenderCapture = false;
			StartCoroutine(_finalRenderCapture);

			return base.PrepareCapture();
		}

		private void CopyRenderTargetToTexture()
		{
#if false
			// RJT TODO: If using D3D12 we need to read the current 'Display.main.colorBuffer', pass it down
			// to native and extract the texture using 'IUnityGraphicsD3D12v5::TextureFromRenderBuffer()'
			// - Although, as is, this doesn't work: https://forum.unity.com/threads/direct3d12-native-plugin-render-to-screen.733025/
			if (_targetNativePointer == System.IntPtr.Zero)
			{
				_targetNativePointer = Display.main.colorBuffer.GetNativeRenderBufferPtr();
//				_targetNativePointer = Graphics.activeColorBuffer.GetNativeRenderBufferPtr();
				NativePlugin.SetColourBuffer(_handle, _targetNativePointer);
			}
#endif
			if ((_targetNativePointer == System.IntPtr.Zero) || 
				(_resolveTexture && ((_resolveTexture.width != Screen.width) || (_resolveTexture.height != Screen.height))))
			{
				FreeRenderResources();

				// Create RT matching screen extents
				_resolveTexture = RenderTexture.GetTemporary(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB, 1);
				_resolveTexture.Create();

				// Create command buffer
				_commandBuffer = new CommandBuffer();
				_commandBuffer.name = "AVPMC CopyRenderTarget";
				_commandBuffer.Blit(BuiltinRenderTextureType.CurrentActive, _resolveTexture);

				RenderTexture sourceTexture = _resolveTexture;

				if ( !NativePlugin.IsBasicEdition() )
				{
					if( _Transparency == Transparency.TopBottom || _Transparency == Transparency.LeftRight )
					{
						InitialiseSideBySideTransparency( Screen.width, Screen.height, true);
						if( _sideBySideTexture )
						{
							// Add to command buffer
							_commandBuffer.Blit(_resolveTexture, _sideBySideTexture, _sideBySideMaterial);
							sourceTexture = _sideBySideTexture;
						}
					}
				}

				_targetNativePointer = sourceTexture.GetNativeTexturePtr();
				NativePlugin.SetTexturePointer(_handle, _targetNativePointer);
			}

			Graphics.ExecuteCommandBuffer(_commandBuffer);

		}

		private void FreeRenderResources()
		{
			// Command buffer
			if (_commandBuffer != null)
			{
				_commandBuffer.Release();
				_commandBuffer = null;
			}

			_targetNativePointer = System.IntPtr.Zero;
	
			if (_resolveTexture)
			{
				RenderTexture.ReleaseTemporary(_resolveTexture);
				_resolveTexture = null;
			}
		}

		public override void UnprepareCapture()
		{
			if (_handle != -1)
			{
				if (_targetNativePointer != System.IntPtr.Zero)
				{
					NativePlugin.SetTexturePointer(_handle, System.IntPtr.Zero);
				}
				else
				{
#if UNITY_ANDROID && !UNITY_EDITOR
					NativePlugin.SetRenderBuffer(_handle, System.IntPtr.Zero);
#else
					// Other platforms do the needful here
#endif
				}
			}

			StopCoroutine(_finalRenderCapture);
			_finalRenderCapture = null;

			FreeRenderResources();

			if (_mouseCursor != null)
			{
				_mouseCursor.enabled = false;
			}

			base.UnprepareCapture();
		}

		private IEnumerator _finalRenderCapture;
		private bool _doFinalRenderCapture = false;

		private IEnumerator FinalRenderCapture()
		{
			while (true)
			{
				yield return _waitForEndOfFrame;
				if (!_doFinalRenderCapture)
					continue;

				TickFrameTimer();

				bool canGrab = true;

				if (IsUsingMotionBlur())
				{
					// If the motion blur is still accumulating, don't grab this frame
					canGrab = _motionBlur.IsFrameAccumulated;
				}

				if (canGrab && CanOutputFrame())
				{
					// Grab final RenderTexture into texture and encode
					EncodeUnityAudio();

					if (_Transparency == Transparency.TopBottom || _Transparency == Transparency.LeftRight)
					{
						// Side-by-side transparency requires blitting outside native
						CopyRenderTargetToTexture();
					}
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
					else if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Direct3D12)
					{
						CopyRenderTargetToTexture();
					}
#elif !UNITY_EDITOR && UNITY_ANDROID
	#if UNITY_2022_1_OR_NEWER
					// Can no longer access the current render buffer in the plugin and passing the native render buffer
					// pointer down leads to bad flickering so we'll take a copy
					else if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Vulkan)
					{
						CopyRenderTargetToTexture();
		#if AVPMC_ANDROID_VULKAN_PRETRANSFORM
						if (!_hasBeenWarnedAboutVulkanPreTransform)
						{
							_hasBeenWarnedAboutVulkanPreTransform = true;
							Debug.LogWarning("AVProMovieCapture: PlayerSettings.vulkanEnablePreTransform is enabled, some screen orientations (typically landscape) may not be captured correctly. Please see https://docs.unity3d.com/Manual/vulkan-swapchain-pre-rotation.html for more information");
						}
		#endif
					}
	#else
					// Android Vulkan requires the current render buffer to do screen captures
					else if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Vulkan)
					{
						System.IntPtr renderBuffer = Display.main.colorBuffer.GetNativeRenderBufferPtr();
						NativePlugin.SetRenderBuffer(_handle, renderBuffer);
					}
	#endif
#elif UNITY_STANDALONE_OSX || (!UNITY_EDITOR && UNITY_IOS)
	#if UNITY_2022_1_OR_NEWER && (USING_URP || USING_HDRP)
					// URP/HDRP requires taking a copy of the frame buffer
					else
					{
						CopyRenderTargetToTexture();
					}
	#endif
#endif

					RenderThreadEvent(NativePlugin.PluginEvent.CaptureFrameBuffer);

					// RJT NOTE: Causes screen flickering under D3D12, even if we're not doing any rendering at native level
					if (SystemInfo.graphicsDeviceType != GraphicsDeviceType.Direct3D12)
					{
						GL.InvalidateState();
					}

					UpdateFPS();
				}

				RenormTimer();
			}
		}

		public override void UpdateFrame()
		{
			_doFinalRenderCapture = _capturing && !_paused;
			base.UpdateFrame();
		}
	}
}
