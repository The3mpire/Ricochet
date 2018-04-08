using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using DG.Tweening;
using Enumerables;

public class SFXManager : MonoBehaviour
{
    #region Inspector Variables
    [Tooltip("Drag the SoundStorage object here")]
    [SerializeField]
    private SoundStorage soundStorage;
    [Tooltip("Drag  in FXSource here")]
    [SerializeField]
    private AudioSource fxSource;
    [Tooltip("The lowest a sound effect will randomly pitched")]
    [SerializeField]
    private float lowPitchRange = .95f;
    [Tooltip("The highest a sound effect will be randomly pitched")]
    [SerializeField]
    private float highPitchRange = 1.05f;
    #endregion

    #region Hidden Variables
    private static SFXManager instance = null;
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
        volumeLock = false;
    }
    #endregion

    #region SFX
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
    #endregion

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
