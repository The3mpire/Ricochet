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
    private GameObject defaultButton;

    [Tooltip("The panel to be enabled when Play is selected")]
    [SerializeField]
    private GameObject playPanel;

    [Tooltip("The first button to be selected when the panel is on")]
    [SerializeField]
    private GameObject playPanelDefaultItem;

    [Tooltip("The panel to be enabled when Options is selected")]
    [SerializeField]
    private GameObject optionsPanel;

    [Tooltip("The first button to be selected when the panel is on")]
    [SerializeField]
    private GameObject optionsPanelDefaultItem;

    [SerializeField]
    private GameDataSO gameData;

    [Header("Test Settings")]
    [SerializeField]
    private Toggle _dashSettingToggle;
    #endregion

    public void Start()
    {
        _dashSettingToggle.isOn = gameData.GetDashSetting();
    }

    #region Public Functions
    public void OpenPlayMenu()
    {
        mainMenuPanel.SetActive(false);
        playPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(playPanelDefaultItem);
    }

    public void ClosePlayMenu()
    {
        mainMenuPanel.SetActive(true);
        playPanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(defaultButton);
    }

    public void OpenOptionsMenu()
    {
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(optionsPanelDefaultItem);
    }

    public void CloseOptionsMenu()
    {
        mainMenuPanel.SetActive(true);
        optionsPanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(defaultButton);
    }

    public void OpenCredits()
    {
        //TODO: Implement Credits page/panel.

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

    public void ExitGame()
    {
        Application.Quit();
    }

    public void SetDashSetting()
    {
        gameData.SetDashSetting(_dashSettingToggle.isOn);
    }

    public void SelectDefaultOption()
    {
        EventSystem.current.SetSelectedGameObject(defaultButton);
        defaultButton.GetComponent<Button>().Select();
    }
    #endregion
}
