using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "AudioSettings", menuName = "VAKT/Settings/Audio Settings")]
public class AudioSettings : ScriptableObject
{
	public float MinDistance = 100f;
	public float MaxDistance = 300f;
	public float SpatialBlend = 1f;
	public float DopplerLevel = 0f;
	public AudioMixerGroup OutputAudioMixerGroup;

	public AudioRolloffMode RolloffMode = AudioRolloffMode.Logarithmic;

	public void ApplySettings(AudioSource audioSource)
	{
		audioSource.outputAudioMixerGroup = OutputAudioMixerGroup;
		audioSource.minDistance = MinDistance;
		audioSource.maxDistance = MaxDistance;
		audioSource.spatialBlend = SpatialBlend;
		audioSource.rolloffMode = RolloffMode;
		audioSource.dopplerLevel = DopplerLevel;
	}
}
