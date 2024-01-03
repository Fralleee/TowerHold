using UnityEngine;

public class PlaySound : MonoBehaviour
{
	public AudioClip AudioClip;
	void Start() => SoundManager.Instance.PlayEffect(AudioClip);
}
