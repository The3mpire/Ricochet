using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Enumerables;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    #region Inspector Variables
    [Tooltip("Drag the SoundStorage object here")]
    [SerializeField]
    private SoundStorage soundStorage;
    [Tooltip("Drag the AudioSource for music here")]
    [SerializeField]
    private AudioSource musicSource;
    [Tooltip("Drag the GameData scriptable object here")]
    [SerializeField]
    private GameDataSO gameData;
    [Tooltip("(Optional) Drag the music volume slider here to set the starting value correctly")]
    [SerializeField]
    private Slider musicSlider;
    [Tooltip("The lowest a sound effect will randomly pitched")]
    [SerializeField]
    private float lowPitchRange = .95f;
    [Tooltip("The highest a sound effect will be randomly pitched")]
    [SerializeField]
    private float highPitchRange = 1.05f;
    [Tooltip("The current song")]
    [SerializeField]
    private AudioClip currentSong;
    [Tooltip("The menu song")]
    [SerializeField]
    private float musicFadeDuration;
    #endregion

    #region Hidden Variables
    private static MusicManager instance = null;
    private float volume;
    private bool volumeLock;
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
        volume = gameData.MusicVolume;
        if (musicSlider != null)
        {
            musicSlider.value = volume;
        }
        //musicSource.volume = volume;
        volumeLock = false;
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
            instance.musicSource.loop = true;
            instance.musicSource.Play();
            IEnumerator fadeInCoroutine = FadeIn(0f, 3f);
            StartCoroutine(fadeInCoroutine);
        }
    }
    #endregion

    #region Music
    public void FadeOutMusic()
    {
        musicSource.DOFade(0, musicFadeDuration);
    }

    public void FadeInMusic()
    {
        musicSource.DOFade(volume, musicFadeDuration);

    }

    private IEnumerator FadeIn(float startVol, float duration)
    {
        if (startVol != 0)
        {
            volumeLock = true;
            float finalVol = volume;
            musicSource.volume = startVol;
            float volumesPerUpdate = (finalVol - startVol) / duration;
            while (musicSource.volume <= finalVol)
            {
                finalVol = volume;
                musicSource.volume += volumesPerUpdate * Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }
            musicSource.volume = volume;
            volumeLock = false;
        }
    }

    public void SceneMusic(AudioClip song)
    {
        if (instance.musicSource != null || !instance.musicSource.clip.Equals(song))
        {
            instance.musicSource.clip = song;
            instance.musicSource.loop = true;
            instance.musicSource.Play();
        }
    }

    public void SetMusicVolume(float _volume)
    {
        gameData.MusicVolume = _volume;
        volume = _volume;
        if (!volumeLock)
            musicSource.volume = volume;
    }

    public float GetManagerVolume()
    {
        return volume;
    }

    public float GetMusicVolume()
    {
        return musicSource.volume;
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
