using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : SerializedSingleton<AudioManager>
{
	public GameContext CurrentContext = GameContext.MainMenu;

	[SerializeField] AudioSource _musicSource;
	[SerializeField] AudioSource _effectSource;
	[SerializeField] List<AudioClip> _menuPlayList;
	[SerializeField] List<AudioClip> _gamePlayList;

	List<AudioClip> _currentPlaylist;
	int _currentTrackIndex = 0;
	bool _isPlayingMusic = false;
	readonly float _fadeDuration = 1.5f;
	float _musicVolume;

	protected override void Awake()
	{
		base.Awake();

		DontDestroyOnLoad(gameObject);

		SceneManager.activeSceneChanged += OnSceneChanged;
	}

	void Start()
	{
		_musicVolume = _musicSource.volume;

		if (SceneManager.GetActiveScene().name != "MenuScene")
		{
			UpdateMenuContext(GameContext.InGameMenu);
		}
		else
		{
			UpdateMenuContext(GameContext.MainMenu);
		}
	}

	void Update()
	{
		if (!_musicSource.isPlaying && _isPlayingMusic)
		{
			PlayRandomTrack();
		}
	}

	void OnSceneChanged(Scene _, Scene next)
	{
		if (next.name != "MenuScene")
		{
			UpdateMenuContext(GameContext.InGameMenu);
		}
		else
		{
			UpdateMenuContext(GameContext.MainMenu);
		}
	}

	public void UpdateMenuContext(GameContext newContext)
	{
		CurrentContext = newContext;
		if (newContext == GameContext.MainMenu)
		{
			_currentPlaylist = _menuPlayList;
		}
		else if (newContext == GameContext.InGameMenu)
		{
			_currentPlaylist = _gamePlayList;
		}

		PlayMusic();
	}

	public void PlayEffect(AudioClip clip)
	{
		_effectSource.PlayOneShot(clip);
	}

	void PlayMusic()
	{
		if (_currentPlaylist.Count > 0)
		{
			_isPlayingMusic = true;
			_currentTrackIndex = UnityEngine.Random.Range(0, _currentPlaylist.Count);
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

		while (audioSource.volume < _musicVolume)
		{
			audioSource.volume += Time.deltaTime / duration;
			audioSource.volume = Mathf.Clamp(audioSource.volume, 0, _musicVolume);
			yield return null;
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();

		SceneManager.activeSceneChanged -= OnSceneChanged;
	}
}
