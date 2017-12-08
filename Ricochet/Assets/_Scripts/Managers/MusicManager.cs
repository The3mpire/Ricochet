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
    [Tooltip("The menu song")]
    [SerializeField]
    private AudioClip menuSong;
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
    }

    void Start()
    {

        Cursor.visible = true;

        instance.musicSource.clip = menuSong;
        instance.musicSource.Play();
    }

    void OnLevelWasLoaded()
    {
        if (SceneManager.GetActiveScene().buildIndex == LevelIndex.MAIN_MENU)
        {
            if (instance.musicSource.clip != menuSong)
            {
                instance.musicSource.clip = menuSong;
                instance.musicSource.Play();
            }
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
        fxSource.PlayOneShot(soundStorage.GetScoringSound());
    }
    #endregion

    #region Helpers
    #endregion
}