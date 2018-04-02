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

    private bool _blockInput = false;

    public void Start()
    {
        _dashSettingToggle.isOn = gameData.GetDashSetting();
    }

    #region Public Functions
    public void OpenPlayMenu()
    {
        if (_blockInput)
        {
            return;
        }
        mainMenuPanel.SetActive(false);
        playPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(playPanelDefaultItem);
    }

    public void ClosePlayMenu()
    {
        if (_blockInput)
        {
            return;
        }
        mainMenuPanel.SetActive(true);
        playPanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(defaultButton);
    }

    public void OpenOptionsMenu()
    {
        if (_blockInput)
        {
            return;
        }
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(optionsPanelDefaultItem);
    }

    public void CloseOptionsMenu()
    {
        if (_blockInput)
        {
            return;
        }
        mainMenuPanel.SetActive(true);
        optionsPanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(defaultButton);
    }

    public void OpenCredits()
    {
        if (_blockInput)
        {
            return;
        }
        //TODO: Implement Credits page/panel.

    }

    public void LaunchClassicMode()
    {
        if (_blockInput)
        {
            return;
        }
        gameData.SetGameMode(EMode.Soccer);
        gameData.SetGameLevel(BuildIndex.LEVEL_SELECT);
        LevelSelect.LoadCharacterSelect();
    }

    public void LaunchDeathmatchMode()
    {
        if (_blockInput)
        {
            return;
        }
        gameData.SetGameMode(EMode.Deathmatch);
        gameData.SetGameLevel(BuildIndex.LEVEL_SELECT);
        LevelSelect.LoadCharacterSelect();
    }

    public void ExitGame()
    {
        if (_blockInput)
        {
            return;
        }
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
