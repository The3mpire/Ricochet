using DG.Tweening;
using Enumerables;
using Rewired;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelSelectManager : MonoBehaviour
{
    #region Inspector Variables
    [SerializeField] private RectTransform confirmationMenu;
    
    [SerializeField] private GameObject defaultConfirmationGameObject;
    
    [SerializeField] private Button[] levelButtons;

    [SerializeField] private Slider timeSlider;
    [SerializeField] private Slider scoreSlider;
    [SerializeField] private Slider backSlider;

    [SerializeField] private GameDataSO gameData;
    #endregion

    #region Private Variables
    private BuildIndex loadLevelBuildIndex;
    private GameObject defaultSelectedLevelButton;
    private IList<Player> players;
    private Player P1;
    private bool bHeld = false;
    private bool _goingBack = false;
    #endregion

    #region Monobehaviours
    private void Start()
    {
        timeSlider.value = gameData.GetTimeLimit();
        scoreSlider.value = gameData.GetScoreLimit();

        List<BuildIndex> levels = LevelSelect.glitchBallClassicLevels;
        // TODO: Switch statement here that looks at GameData for game type enum and sets the appropriate levels

        bool defaultFound = false;

        foreach (Button b in levelButtons)
        {
            UI_LevelButton script = b.GetComponent<UI_LevelButton>();
            script.SetManager(this);
            if (!defaultFound)
            {
                defaultSelectedLevelButton = b.gameObject;
                defaultFound = true;
            }
        }

        EventSystem.current.SetSelectedGameObject(defaultSelectedLevelButton);

        players = ReInput.players.AllPlayers;
        P1 = players[1]; // System is player 0
    }

    private void Update()
    {
        bool cancel = P1.GetButton("UICancel");

        if (cancel)
        {
            if (confirmationMenu.gameObject.activeSelf)
            {
                CloseConfirmationMenu();
            }
            else
            {
                backSlider.value += (Time.deltaTime * 1.75f);
                if (backSlider.value >= backSlider.maxValue)
                {
                    OnCancel();
                    _goingBack = true;
                }
            }
        }
        else
        {
            backSlider.value -= Time.deltaTime * 2;
        }
    }
    #endregion

    #region Public Functions
    public void SetMatchScoreLimit(Slider slider)
    {
        gameData.SetScoreLimit((int)slider.value);
    }

    public void SetMatchTimeLimit(Slider slider)
    {
        gameData.SetTimeLimit((int)slider.value);
    }

    public void SetLoadLevel(BuildIndex levelInt)
    {
        if (levelInt == BuildIndex.RANDOM)
        {
            levelInt = SelectRandomLevel();
        }
        gameData.SetGameLevel(levelInt);
        loadLevelBuildIndex = levelInt;
        OpenConfirmationMenu();
    }

    public void CloseConfirmationMenu()
    {
        confirmationMenu.gameObject.SetActive(false);
        EventSystem.current.SetSelectedGameObject(defaultSelectedLevelButton);
    }

    public void OnCancel()
    {
        if (!_goingBack)
        {
            LevelSelect.LoadLevel(BuildIndex.CHARACTER_SELECT);
        }
    }

    public void LoadLevel()
    {
        if (loadLevelBuildIndex == BuildIndex.CHARACTER_SELECT)
        {
            LevelSelect.LoadLevel(loadLevelBuildIndex);
            return;
        }
        LevelSelect.LoadLevel(BuildIndex.CONTROLLER_MAP);
    }

    private void OpenConfirmationMenu()
    {
        confirmationMenu.gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(defaultConfirmationGameObject);
    }

    private BuildIndex SelectRandomLevel()
    {
        int level = Random.Range(0, 3);
        return LevelSelect.glitchBallClassicLevels[level];
    }
    #endregion
}
