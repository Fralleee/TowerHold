using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : SerializedSingleton<SoundManager>
{
	[SerializeField] AudioSource _musicSource;
	[SerializeField] AudioSource _effectSource;
	[SerializeField]
	Dictionary<GameStage, List<AudioClip>> _stagePlaylists = new Dictionary<GameStage, List<AudioClip>>{
		{GameStage.Low, new List<AudioClip>()},
		{GameStage.Medium, new List<AudioClip>()},
		{GameStage.High, new List<AudioClip>()}
	};

	List<AudioClip> _currentPlaylist;
	int _currentTrackIndex = 0;
	bool _isPlayingMusic = false;
	GameStage _currentStage;
	readonly float _fadeDuration = 1.5f;

	void Start()
	{
		GameController.OnStageChanged += ChangePlaylist;
		_currentPlaylist = _stagePlaylists[GameStage.Low];
		PlayMusic();
	}

	void Update()
	{
		if (!_musicSource.isPlaying && _isPlayingMusic)
		{
			PlayRandomTrack();
		}
	}

	public void PlayEffect(AudioClip clip) => _effectSource.PlayOneShot(clip);

	void PlayMusic()
	{
		if (_currentPlaylist.Count > 0)
		{
			_isPlayingMusic = true;
			_musicSource.clip = _currentPlaylist[_currentTrackIndex];
			StartCoroutine(FadeIn(_musicSource, _fadeDuration));
		}
	}

	void PlayRandomTrack()
	{
		if (_currentPlaylist.Count > 0)
		{
			_isPlayingMusic = true;
			_musicSource.clip = _currentPlaylist[UnityEngine.Random.Range(0, _currentPlaylist.Count)];
			StartCoroutine(FadeIn(_musicSource, _fadeDuration));
		}
	}

	void ChangePlaylist(GameStage newStage)
	{
		if (newStage != _currentStage)
		{
			_currentStage = newStage;
			_currentPlaylist = _stagePlaylists[_currentStage];
			_currentTrackIndex = 0;

			// Start fading out the current track and then play the next track
			StartCoroutine(FadeOut(_musicSource, _fadeDuration, PlayMusic));
		}
	}

	public void StopMusic()
	{
		_musicSource.Stop();
		_isPlayingMusic = false;
	}

	IEnumerator FadeOut(AudioSource audioSource, float duration, Action onComplete = null)
	{
		var startVolume = audioSource.volume;

		while (audioSource.volume > 0)
		{
			audioSource.volume -= startVolume * Time.deltaTime / duration;
			yield return null;
		}

		audioSource.Stop();
		audioSource.volume = startVolume;
		onComplete?.Invoke();
	}

	IEnumerator FadeIn(AudioSource audioSource, float duration)
	{
		audioSource.volume = 0;
		audioSource.Play();

		while (audioSource.volume < 1)
		{
			audioSource.volume += Time.deltaTime / duration;
			yield return null;
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		GameController.OnStageChanged -= ChangePlaylist; // Unsubscribe when destroyed
	}
}
