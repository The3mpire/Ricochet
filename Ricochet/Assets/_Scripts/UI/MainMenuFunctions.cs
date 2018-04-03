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
    [SerializeField]
    private GameObject _menuPanel;

    [SerializeField]
    private GameObject _characterArtPanel;

    [SerializeField]
    [Tooltip("0 - Main Menu. 1 - Play Menu, 2 - Options Menu, 3 - Credits")]
    private GameObject[] _characterImages = new GameObject[4];

    [Tooltip("The first button to be selected in the scene")]
    [SerializeField]
    private GameObject defaultButton;

    [SerializeField]
    private GameObject mainMenuPanel;

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
        //mainMenuPanel.SetActive(false);
        //playPanel.SetActive(true);
        SwapPanels(mainMenuPanel, playPanel, _currentCharacterArt, _characterImages[1], playPanelDefaultItem);
        //Debug.Log(EventSystem.current.currentInputModule);
        //EventSystem.current.SetSelectedGameObject(playPanelDefaultItem);
    }

    public void ClosePlayMenu()
    {
        if (_blockInput)
        {
            return;
        }
        //mainMenuPanel.SetActive(true);
        //playPanel.SetActive(false);
        SwapPanels(playPanel, mainMenuPanel, _currentCharacterArt, _characterImages[0], defaultButton);
        //EventSystem.current.SetSelectedGameObject(defaultButton);
    }

    public void OpenOptionsMenu()
    {
        if (_blockInput)
        {
            return;
        }
        //mainMenuPanel.SetActive(false);
        //optionsPanel.SetActive(true);
        SwapPanels(mainMenuPanel, optionsPanel, _currentCharacterArt, _characterImages[2], optionsPanelDefaultItem);
        //EventSystem.current.SetSelectedGameObject(optionsPanelDefaultItem);
    }

    public void CloseOptionsMenu()
    {
        if (_blockInput)
        {
            return;
        }
        //mainMenuPanel.SetActive(true);
        //optionsPanel.SetActive(false);
        SwapPanels(optionsPanel, mainMenuPanel, _currentCharacterArt, _characterImages[0], defaultButton);
        //EventSystem.current.SetSelectedGameObject(defaultButton);
    }

    public void OpenCredits()
    {
        if (_blockInput)
        {
            return;
        }
        //TODO: Implement Credits page/panel.
        SwapPanels(mainMenuPanel, mainMenuPanel, _currentCharacterArt, _characterImages[3], defaultButton);

    }

    public void CloseCredits()
    {
        if (_blockInput)
        {
            return;
        }
        //mainMenuPanel.SetActive(true);
        //optionsPanel.SetActive(false);
        SwapPanels(mainMenuPanel, mainMenuPanel, _currentCharacterArt, _characterImages[0], defaultButton);
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

    private void SwapPanels(GameObject fromPanel, GameObject toPanel, GameObject fromCharacter, GameObject toCharacter, GameObject button)
    {
        var charPanelSlide = _characterArtPanel.GetComponent<PanelSlide>();
        var mainPanelSlide = _menuPanel.GetComponent<PanelSlide>();
        //Move panels out
        Tween cTween = charPanelSlide.ExecuteMoveBack(1);
        Tween mTween = mainPanelSlide.ExecuteMoveBack(1);

        StartCoroutine(DoSwap(cTween, mTween, fromPanel, toPanel, fromCharacter, toCharacter, button));

    }
    private void SwapCharacterArt(GameObject fromcharacter, GameObject toCharacter)
    {
        fromcharacter.SetActive(false);
        toCharacter.SetActive(true);
        _currentCharacterArt = toCharacter;
    }

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
        cTween = _characterArtPanel.GetComponent<PanelSlide>().ExecuteMoveTo(1);
        mTween = _menuPanel.GetComponent<PanelSlide>().ExecuteMoveTo(1);

        StartCoroutine(SetSelectedButton(cTween, mTween, button));
    }

    IEnumerator SetSelectedButton(Tween cTween, Tween mTween, GameObject button)
    {
        yield return new WaitUntil(() => !cTween.IsPlaying() && !mTween.IsPlaying());

        EventSystem.current.SetSelectedGameObject(button);
    }
    #endregion
}
