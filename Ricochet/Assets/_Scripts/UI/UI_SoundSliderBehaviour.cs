using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_SoundSliderBehaviour : MonoBehaviour
{
    #region Inspector Variables
    [Tooltip("Drag the slider here")]
    [SerializeField]
    private Slider slider;
    #endregion

    #region Hidden Variables
    private GameManager gameManagerInstance;
    #endregion

    #region Mono Behaviour
    void Start()
    {
        if (gameManagerInstance != null || GameManager.TryGetInstance(out gameManagerInstance))
        {
            slider.value = gameManagerInstance.GetMusicVolume();
        }
    }
    #endregion

    #region Slider Methods
    public void SetMusicVolume(float vol = 0.8f)
    {
        if (gameManagerInstance != null || GameManager.TryGetInstance(out gameManagerInstance))
        {
            gameManagerInstance.SetMusicVolume(vol);
        }
        slider.value = vol;
    }
    #endregion

}