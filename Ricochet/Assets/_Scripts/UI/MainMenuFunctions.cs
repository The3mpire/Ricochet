using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Enumerables;

public class MainMenuFunctions : MonoBehaviour
{
    #region Reference Variables
    [SerializeField]
    private GameObject mainMenuPanel;

    [Tooltip("The first button to be selected in the scene")]
    [SerializeField]
    private GameObject mainMenuButton;

    [Tooltip("The panel to be enabled when start is pressed")]
    [SerializeField]
    private GameObject optionsPanel;

    [Tooltip("The first button to be selected when the panel is on")]
    [SerializeField]
    private GameObject optionsButton;

    [SerializeField]
    private GameDataSO gameData;
    #endregion

    #region Public Functions
    public void OpenGameOptions()
    {
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(optionsButton);

    }

    public void CloseGameOptions()
    {
        mainMenuPanel.SetActive(true);
        optionsPanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(mainMenuButton);
    }

    public void LaunchClassicMode()
    {
        gameData.SetGameMode(EMode.Soccer);
        gameData.SetGameLevel(BuildIndex.LEVEL_SELECT);
        LevelSelect.LoadCharacterSelect();
    }

    public void LaunchDeathmatchMode()
    {
        gameData.SetGameMode(EMode.Deathmatch);
        gameData.SetGameLevel(BuildIndex.LEVEL_SELECT);
        LevelSelect.LoadCharacterSelect();
    }
    #endregion
}
