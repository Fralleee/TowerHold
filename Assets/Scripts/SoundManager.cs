using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : SerializedSingleton<SoundManager>
{
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource effectSource;
    [SerializeField] private Dictionary<GameStage, List<AudioClip>> stagePlaylists;

    private List<AudioClip> currentPlaylist;
    private int currentTrackIndex = 0;
    private bool isPlayingMusic = false;
    private GameStage currentStage;
    private float fadeDuration = 1.5f; // Duration of the fade in seconds

    private void Start()
    {
        GameController.OnStageChanged += ChangePlaylist;
        currentPlaylist = stagePlaylists[GameStage.Low];
        PlayMusic();
    }

    private void Update()
    {
        if (!musicSource.isPlaying && isPlayingMusic)
        {
            PlayRandomTrack();
        }
    }

    public void PlayEffect(AudioClip clip)
    {
        effectSource.PlayOneShot(clip);
    }

    private void PlayMusic()
    {
        if (currentPlaylist.Count > 0)
        {
            isPlayingMusic = true;
            musicSource.clip = currentPlaylist[currentTrackIndex];
            StartCoroutine(FadeIn(musicSource, fadeDuration));
        }
    }

    private void PlayRandomTrack()
    {
        if (currentPlaylist.Count > 0)
        {
            isPlayingMusic = true;
            musicSource.clip = currentPlaylist[UnityEngine.Random.Range(0, currentPlaylist.Count)];
            StartCoroutine(FadeIn(musicSource, fadeDuration));
        }
    }

    private void ChangePlaylist(GameStage newStage)
    {
        if (newStage != currentStage)
        {
            currentStage = newStage;
            currentPlaylist = stagePlaylists[currentStage];
            currentTrackIndex = 0;

            // Start fading out the current track and then play the next track
            StartCoroutine(FadeOut(musicSource, fadeDuration, PlayMusic));
        }
    }

    public void StopMusic()
    {
        musicSource.Stop();
        isPlayingMusic = false;
    }

    private IEnumerator FadeOut(AudioSource audioSource, float duration, Action onComplete = null)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / duration;
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
        onComplete?.Invoke();
    }

    private IEnumerator FadeIn(AudioSource audioSource, float duration)
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
