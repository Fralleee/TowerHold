using UnityEngine;

public class PlaySound : MonoBehaviour
{
	public AudioClip AudioClip;
	void Start()
	{
		AudioManager.PlayEffect(AudioClip);
	}
}
