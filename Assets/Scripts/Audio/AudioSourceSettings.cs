using UnityEngine;

[CreateAssetMenu(fileName = "AudioSettings", menuName = "Settings/Audio Settings")]
public class AudioSettings : ScriptableObject
{
	public float MinDistance = 100f;
	public float MaxDistance = 300f;
	public float SpatialBlend = 1f;
	public AudioRolloffMode RolloffMode = AudioRolloffMode.Logarithmic;
	// Add any other settings you need
}
