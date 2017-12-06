using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Enumerables;

public class MainMenuFunctions : MonoBehaviour {

    #region Reference Variables
    [Tooltip("The panel to be enabled when start is pressed")]
    [SerializeField]
    private GameObject optionsPanel;
    [Tooltip("The first button to be selected when the panel is on")]
    [SerializeField]
    private GameObject optionsButton;
    [Tooltip("The first button to be selected in the scene")]
    [SerializeField]
    private GameObject defaultButton;
    [Tooltip("The dropdown menu for game modes")]
    [SerializeField]
    private Dropdown gameModes;
    #endregion

    #region Hidden Variables
    private GameManager gameManagerInstance;
    #endregion

    void Awake()
    {
        SendGameMode();
    }

    public void OpenGameOptions()
    {
        optionsPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(optionsButton);
    }

    public void CloseGameOptions()
    {
        optionsPanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(defaultButton);
    }

    public void SendGameMode()
    {
        if (gameManagerInstance != null || GameManager.TryGetInstance(out gameManagerInstance))
        {
            EMode mode;
            switch (gameModes.value)
            {
                case 0:
                    mode = EMode.Soccer;
                    break;
                case 1:
                    mode = EMode.Deathmatch;
                    break;
                default:
                    mode = EMode.Soccer;
                    break;
            }
            GameData.gameMode = mode;
        }
    }
}
