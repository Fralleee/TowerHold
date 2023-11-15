using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource effectSource;
    [SerializeField] private List<AudioClip> musicTracks;

    private int currentTrackIndex = 0;
    private bool isPlayingMusic = false;

    private void Start()
    {
        PlayMusic();
    }

    private void Update()
    {
        if (!musicSource.isPlaying && isPlayingMusic)
        {
            PlayNextTrack();
        }
    }

    public void PlayEffect(AudioClip clip)
    {
        effectSource.PlayOneShot(clip);
    }

    private void PlayMusic()
    {
        if (musicTracks.Count > 0)
        {
            isPlayingMusic = true;
            musicSource.clip = musicTracks[currentTrackIndex];
            musicSource.Play();
        }
    }

    private void PlayNextTrack()
    {
        currentTrackIndex = (currentTrackIndex + 1) % musicTracks.Count;
        musicSource.clip = musicTracks[currentTrackIndex];
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
        isPlayingMusic = false;
    }
}
