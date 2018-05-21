using UnityEngine;
using UnityEngine.UI;
using Enumerables;

public class SFXManager : MonoBehaviour
{
    #region Inspector Variables
    [Tooltip("Drag the SoundStorage object here")]
    [SerializeField]
    private SoundStorage soundStorage;
    [Tooltip("Drag in FXSource here")]
    [SerializeField]
    private AudioSource fxSource;
    [Tooltip("Drag the GameData scriptable object here")]
    [SerializeField]
    private GameDataSO gameData;
    [Tooltip("(Optional) Drag the sfx volume slider here to set the starting value correctly")]
    [SerializeField]
    private Slider sfxSlider;
    [Tooltip("The lowest a sound effect will randomly pitched")]
    [SerializeField]
    private float lowPitchRange = .95f;
    [Tooltip("The highest a sound effect will be randomly pitched")]
    [SerializeField]
    private float highPitchRange = 1.05f;
    
    #endregion

    #region Hidden Variables
    private static SFXManager instance = null;
    private float volume;
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

        volume = gameData.SFXVolume;
        if (sfxSlider != null)
        {
            sfxSlider.value = volume;
        }
        fxSource.volume = volume;
    }
    #endregion

    #region Play SFX
    public void PlaySound(AudioClip clip)
    {
        fxSource.PlayOneShot(clip);
    }

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
        if (fxSource != null)
        {
            fxSource.PlayOneShot(soundStorage.GetMenuTraverseSounds());
        }
    }

    public void PlaySceneTraversalSound()
    {
        fxSource.PlayOneShot(soundStorage.GetSceneTransitionSound());
    }

    public void PlayTeamWinSound(ETeam team)
    {
        fxSource.PlayOneShot(soundStorage.GetTeamWinSound(team));
    }

    public void PlayPingSound()
    {
        fxSource.PlayOneShot(soundStorage.GetPingSound());
    }
    #endregion

    public float GetSFXVolume()
    {
        return volume;
    }

    public void SetSFXVolume(float _volume)
    {
        gameData.SFXVolume = _volume;
        volume = _volume;
        fxSource.volume = volume;
    }

    #region Helpers
    public static bool TryGetInstance(out SFXManager sm)
    {
        sm = instance;
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
