using UnityEngine;

//-----------------------------------------------------------------------------
// Copyright 2012-2022 RenderHeads Ltd.  All rights reserved.
//-----------------------------------------------------------------------------

namespace RenderHeads.Media.AVProMovieCapture
{
	/// <summary>
	/// Encodes audio directly from an AudioClip
	/// Requires AudioCaptureSource.Manual to be set on the Capture component
	/// Make sure the AudioClip sample rate and channel count matches that of the Capture component
	/// Designed to work in offline capture mode
	/// </summary>
	[AddComponentMenu("AVPro Movie Capture/Audio/Capture Audio (From AudioClip)", 500)]
	public class CaptureAudioFromAudioClip : MonoBehaviour
	{
		[SerializeField]
		CaptureBase _capture = null;
		
		[SerializeField]
		AudioClip _audioClip = null;
		
		//[SerializeField] bool _loopAudio = false;
		[SerializeField]
		bool _restartAudioClipOnCaptureStart = false;

		private int _videoOffsetInSamples = 0;
		private int _committedFrames = 0;
		private int _committedSamples = 0;
		private int _lastCommittedSample = -1;

		private float[] _frameBuffer = null;

		void Reset()
		{
			_videoOffsetInSamples = 0;
			_committedFrames = 0;
			_committedSamples = 0;
			_lastCommittedSample = -1;
		}

		void OnCaptureStart()
		{
			if (_restartAudioClipOnCaptureStart)
				Reset();
		}

		void OnEnable()
		{
			if (_capture == null)
			{
				Debug.LogWarning("CaptureAudioFromAudioClip - no capture component set, will try and get one from the attached game object...");
				if (!gameObject.TryGetComponent<CaptureBase>(out _capture))
				{
					Debug.LogError("CaptureAudioFromAudioClip - Failed to find capture component, disabling component");
					enabled = false;
					return;
				}
			}

			if (_audioClip == null)
			{
				Debug.LogError("CaptureAudioFromAudioClip - audio clip has not been set, disabling component");
				enabled = false;
				return;
			}

			if (_audioClip.samples == 0)
			{
				Debug.LogError("CaptureAudioFromAudioClip - zero length audio clip, disabling component");
				enabled = false;
				return;
			}

			if (_audioClip.frequency != _capture.ManualAudioSampleRate)
			{
				_capture.ManualAudioSampleRate = _audioClip.frequency;
			}

			if (_audioClip.channels != _capture.ManualAudioChannelCount)
			{
				_capture.ManualAudioChannelCount = _audioClip.channels;
			}

			_capture.OnCaptureStart.AddListener(OnCaptureStart);
			_lastCommittedSample = -1;
		}

		void Update()
		{
			if (_capture != null && _capture.IsCapturing())
			{
				float[] samples = GetAudioSamplesForFrame();
				if (samples != null)
				{
					_capture.EncodeAudio(samples);
				}
				_committedFrames++;
			}
		}

		private float[] GetAudioSamplesForFrame()
		{
			float[] result = null;
			int sampleCommitSize = (int)(_capture.ManualAudioSampleRate / _capture.FrameRate);
			float videoRenderTime = (float)_committedFrames / _capture.FrameRate;
			float videoTimeInSamples = videoRenderTime * (float)_audioClip.frequency;

			if (_lastCommittedSample < videoTimeInSamples && videoTimeInSamples >= 0)
			{
				int startSampleIndex = (_lastCommittedSample + 1) - _videoOffsetInSamples;
				int endSampleIndex = startSampleIndex + sampleCommitSize - 1;

				int requiredSamples = _audioClip.channels * sampleCommitSize;
				if (_lastCommittedSample < _audioClip.samples - 1)
				{
					if (endSampleIndex >= _audioClip.samples)
					{
						// We've reached the end of the clip samples, so just grab what remains
						requiredSamples = (_audioClip.channels * (_audioClip.samples - startSampleIndex));
					}

					// Allocate buffer if needed
					if (_frameBuffer == null || _frameBuffer.Length != requiredSamples)
					{
						_frameBuffer = new float[requiredSamples];
					}

					_audioClip.GetData(_frameBuffer, startSampleIndex);

					_committedSamples += 1;
					_lastCommittedSample = (_committedSamples * sampleCommitSize) - 1;
				}
				else
				{
					// We've reached the end of the audio clip, so just create empty audio data
					requiredSamples = (_audioClip.channels * sampleCommitSize);
					// Allocate buffer if needed
					if (_frameBuffer == null || _frameBuffer.Length != requiredSamples)
					{
						_frameBuffer = new float[requiredSamples];
					}
				}
				result = _frameBuffer;
			}
			return result;
		}
	}
}