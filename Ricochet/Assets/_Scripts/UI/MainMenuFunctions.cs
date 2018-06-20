using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Enumerables;
using Rewired.Integration.UnityUI;

public class MainMenuFunctions : MonoBehaviour
{
    #region Reference Variables
    [SerializeField] private GameObject _menuPanel;
    [SerializeField] private GameObject _characterArtPanel;

    [Tooltip("0 - Main Menu. 1 - Play Menu, 2 - Options Menu, 3 - Credits")]
    [SerializeField] private GameObject[] _characterImages = new GameObject[4];

    [Tooltip("The how fast the panels move in or out.")]
    [SerializeField] private float _panelSwapSpeed = 1f;

    [Tooltip("The first button to be selected in the scene")]
    [SerializeField] private GameObject defaultButton;

    [SerializeField] private GameObject mainMenuPanel;

    [Tooltip("The panel to be enabled when Play is selected")]
    [SerializeField] private GameObject playPanel;

    [Tooltip("The first button to be selected when the panel is on")]
    [SerializeField] private GameObject playPanelDefaultItem;

    [Tooltip("The panel to be enabled when Controls is selected")]
    [SerializeField] private GameObject controlsPanel;

    [Tooltip("The first button to be selected when the panel is on")]
    [SerializeField] private GameObject controlsPanelDefaultItem;

    [Tooltip("The panel to be enabled when Controls is selected")]
    [SerializeField] private GameObject controllerMapPanel;

    [Tooltip("The first button to be selected when the panel is on")]
    [SerializeField] private GameObject controllerMapDefaultItem;

    [Tooltip("The panel to be enabled when Controls is selected")]
    [SerializeField] private GameObject keyboardMapPanel;

    [Tooltip("The first button to be selected when the panel is on")]
    [SerializeField] private GameObject keyboardMapDefaultItem;

    [Tooltip("The panel to be enabled when Options is selected")]
    [SerializeField] private GameObject optionsPanel;

    [Tooltip("The first button to be selected when the panel is on")]
    [SerializeField] private GameObject optionsPanelDefaultItem;

    [Tooltip("The panel to be enabled when Credits is selected")]
    [SerializeField] private GameObject creditsPanel;

    [Tooltip("The first button to be selected when the panel is on")]
    [SerializeField] private GameObject creditsPanelDefaultItem;

    [SerializeField] private GameDataSO gameData;

    [Header("Test Settings")]
    [SerializeField] private Toggle _dashSettingToggle;
    #endregion

    #region Private Variables
    private bool _blockInput = false;
    private GameObject _currentCharacterArt;
    #endregion

    #region MonoBehaviour

    public void Start()
    {
        //if (EventSystem.current.currentInputModule == null)
        //{
        //    RewiredStandaloneInputModule inMod =
        //        EventSystem.current.gameObject.GetComponent<RewiredStandaloneInputModule>();
        //    EventSystem.current.gameObject.GetComponent<RewiredStandaloneInputModule>().ActivateModule();
        //}
        _dashSettingToggle.isOn = gameData.GetDashSetting();
        _currentCharacterArt = _characterImages[0];
        _currentCharacterArt.SetActive(true);
        //if (gameData.GetSkipToMode())
        //{
        //    gameData.SetSkipToMode(false);
        //    OpenPlayMenu();
        //}
    }

    #endregion

    #region Public Functions
    public void OpenPlayMenu()
    {
        if (_blockInput)
        {
            return;
        }
        SwapPanels(mainMenuPanel, playPanel, _currentCharacterArt, _characterImages[1], playPanelDefaultItem);
        //Debug.Log(EventSystem.current.currentInputModule);
    }

    public void ClosePlayMenu()
    {
        if (_blockInput)
        {
            return;
        }
        SwapPanels(playPanel, mainMenuPanel, _currentCharacterArt, _characterImages[0], defaultButton);
    }

    public void OpenControlsMenu()
    {
        if (_blockInput)
        {
            return;
        }
        SwapPanels(mainMenuPanel, controlsPanel, _currentCharacterArt, _characterImages[3], controlsPanelDefaultItem);
    }

    public void CloseControlsMenu()
    {
        if (_blockInput)
        {
            return;
        }
        SwapPanels(controlsPanel, mainMenuPanel, _currentCharacterArt, _characterImages[0], defaultButton);
    }

    public void OpenOptionsMenu()
    {
        if (_blockInput)
        {
            return;
        }
        SwapPanels(mainMenuPanel, optionsPanel, _currentCharacterArt, _characterImages[2], optionsPanelDefaultItem);
    }

    public void CloseOptionsMenu()
    {
        if (_blockInput)
        {
            return;
        }
        SwapPanels(optionsPanel, mainMenuPanel, _currentCharacterArt, _characterImages[0], defaultButton);
    }

    public void OpenCredits()
    {
        if (_blockInput)
        {
            return;
        }
        SwapPanels(mainMenuPanel, creditsPanel, _currentCharacterArt, _characterImages[3], creditsPanelDefaultItem);

    }

    public void CloseCredits()
    {
        if (_blockInput)
        {
            return;
        }
        SwapPanels(creditsPanel, mainMenuPanel, _currentCharacterArt, _characterImages[0], defaultButton);
    }

    public void OpenControllerMap()
    {
        if (_blockInput)
        {
            return;
        }
        SwapPanels(controlsPanel, controllerMapPanel, _currentCharacterArt, _characterImages[3], controllerMapDefaultItem);
    }

    public void CloseControllerMap()
    {
        if (_blockInput)
        {
            return;
        }
        SwapPanels(controllerMapPanel, controlsPanel, _currentCharacterArt, _characterImages[3], controlsPanelDefaultItem);
    }

    public void OpenKeyboardMap()
    {
        if (_blockInput)
        {
            return;
        }
        SwapPanels(controlsPanel, keyboardMapPanel, _currentCharacterArt, _characterImages[3], keyboardMapDefaultItem);
    }

    public void CloseKeyboardMap()
    {
        if (_blockInput)
        {
            return;
        }
        SwapPanels(keyboardMapPanel, controlsPanel, _currentCharacterArt, _characterImages[3], controlsPanelDefaultItem);
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

    #region Private Functions
    /// <summary>
    /// Swaps out MainMenu and CharacterArt panel contents.
    /// </summary>
    /// <param name="fromPanel">The panel changing from</param>
    /// <param name="toPanel">The panel to change to</param>
    /// <param name="fromCharacter">The current displayed character panel</param>
    /// <param name="toCharacter">The character panel to change to</param>
    /// <param name="button">The button on the new menu panel to set as active</param>
    private void SwapPanels(GameObject fromPanel, GameObject toPanel, GameObject fromCharacter, GameObject toCharacter, GameObject button)
    {
        var charPanelSlide = _characterArtPanel.GetComponent<PanelSlide>();
        var mainPanelSlide = _menuPanel.GetComponent<PanelSlide>();
        //Move panels out
        Tween cTween = charPanelSlide.ExecuteMoveBack(_panelSwapSpeed);
        Tween mTween = mainPanelSlide.ExecuteMoveBack(_panelSwapSpeed);

        StartCoroutine(DoSwap(cTween, mTween, fromPanel, toPanel, fromCharacter, toCharacter, button));

    }
    private void SwapCharacterArt(GameObject fromcharacter, GameObject toCharacter)
    {
        fromcharacter.SetActive(false);
        toCharacter.SetActive(true);
        _currentCharacterArt = toCharacter;
    }

    /// <summary>
    /// Swap coroutine. Swaps out menu panels and character art panels after the provided Tween has stopped playing.
    /// </summary>
    /// <param name="cTween">Character panel Tween</param>
    /// <param name="mTween">Menu panel Tween</param>
    /// <param name="fromPanel">The panel changing from</param>
    /// <param name="toPanel">The panel to change to</param>
    /// <param name="fromCharacter">The current displayed character panel</param>
    /// <param name="toCharacter">The character panel to change to</param>
    /// <param name="button">The button on the new menu panel to set as active</param>
    /// <returns></returns>
    IEnumerator DoSwap(Tween cTween, Tween mTween, GameObject fromPanel, GameObject toPanel, GameObject fromCharacter, GameObject toCharacter, GameObject button)
    {
        //wait until panels have finished moving out
        yield return new WaitUntil(() => !cTween.IsPlaying() && !mTween.IsPlaying());
        //Swap menu panels
        fromPanel.SetActive(false);
        toPanel.SetActive(true);
        //Switch out character art/animation
        SwapCharacterArt(fromCharacter, toCharacter);

        //Move panels in
        cTween = _characterArtPanel.GetComponent<PanelSlide>().ExecuteMoveTo(_panelSwapSpeed);
        mTween = _menuPanel.GetComponent<PanelSlide>().ExecuteMoveTo(_panelSwapSpeed);

        StartCoroutine(SetSelectedButton(cTween, mTween, button));
    }

    /// <summary>
    /// Sets the selected GameObject to the provided button/Selectable object after provided Tweens have stopped playing.
    /// </summary>
    /// <param name="cTween">Character panel tween</param>
    /// <param name="mTween">Menu panel Tween</param>
    /// <param name="button">Button to set as selected</param>
    /// <returns></returns>
    IEnumerator SetSelectedButton(Tween cTween, Tween mTween, GameObject button)
    {
        yield return new WaitUntil(() => !cTween.IsPlaying() && !mTween.IsPlaying());

        EventSystem.current.SetSelectedGameObject(button);
    }
    #endregion
}
