using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using DG.Tweening;
using Enumerables;

public class MusicManager : MonoBehaviour
{
    #region Inspector Variables
    [Tooltip("Drag the SoundStorage object here")]
    [SerializeField]
    private SoundStorage soundStorage;
    [Tooltip("Drag  in FXSource here")]
    [SerializeField]
    private AudioSource fxSource;
    [Tooltip("Drag in Music Source here")]
    [SerializeField]
    private AudioSource musicSource;
    [Tooltip("The lowest a sound effect will randomly pitched")]
    [SerializeField]
    private float lowPitchRange = .95f;
    [Tooltip("The highest a sound effect will be randomly pitched")]
    [SerializeField]
    private float highPitchRange = 1.05f;
    [Tooltip("The current song")]
    [SerializeField]
    private AudioClip currentSong;
    [Tooltip("The music volume at when the level is loaded")]
    [SerializeField]
    private float musicVol;
    [Tooltip("The menu song")]
    [SerializeField]
    private float musicFadeDuration;
    #endregion

    #region Hidden Variables
    private static MusicManager instance = null;
    #endregion

    #region Mono Behaviour
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
        OnLevelWasLoaded();
    }

    void OnLevelWasLoaded()
    {
        int buildIndex = SceneManager.GetActiveScene().buildIndex;
        BuildIndex levelIndex = (BuildIndex)buildIndex;
        AudioClip newSong = soundStorage.GetSceneMusic(levelIndex);
        if (instance.musicSource.isPlaying && currentSong == null)
        {
            currentSong = newSong;
        }
        else if (currentSong != newSong && newSong != null)
        {
            currentSong = newSong;
            instance.musicSource.clip = newSong;
            instance.musicSource.Play();
            IEnumerator fadeInCoroutine = FadeIn(0f, 3f);
            StartCoroutine(fadeInCoroutine);
        }
    }
    #endregion

    #region Music
    public void FadeOutMusic()
    {
        musicVol = musicSource.volume;
        musicSource.DOFade(0, musicFadeDuration);
    }

    public void FadeInMusic()
    {
        musicSource.DOFade(musicVol, musicFadeDuration);

    }

    private IEnumerator FadeIn(float startVol, float duration)
    {
        musicSource.volume = startVol;
        float volumesPerUpdate = (1f - startVol) / duration ;
        while (musicSource.volume <= 1f)
        {
            musicSource.volume += volumesPerUpdate * Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
        musicSource.volume = 1f;
    }

    public void SceneMusic(AudioClip song)
    {
        if (instance.musicSource != null || !instance.musicSource.clip.Equals(song))
        {
            instance.musicSource.clip = song;
            instance.musicSource.Play();
        }
    }

    public void SetMusicVolume(float vol = .8f)
    {
        musicVol = vol;
        musicSource.volume = vol;
    }

    public float GetMusicVolume()
    {
        return musicSource.volume;
    }
    #endregion

    #region SFX
    public void PlayMenuClickSound()
    {
        fxSource.PlayOneShot(soundStorage.GetMenuClickSound());
    }

    public void PlayMenuBackSound()
    {
        fxSource.PlayOneShot(soundStorage.GetMenuBackSound());
    }

    public void PlayMenuUnpauseSound()
    {
        fxSource.PlayOneShot(soundStorage.GetUnpauseSound());
    }

    public void PlayMenuPauseSound()
    {
        fxSource.PlayOneShot(soundStorage.GetPauseSound());
    }

    public void PlayMenuTraversalSound()
    {
        fxSource.PlayOneShot(soundStorage.GetMenuTraverseSounds());
    }
    #endregion

    #region Helpers
    public static bool TryGetInstance(out MusicManager mm)
    {
        mm = instance;
        if (instance == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    #endregion
}
