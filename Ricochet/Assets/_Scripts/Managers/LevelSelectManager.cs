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

    [SerializeField] private GameDataSO gameData;
    #endregion

    #region Private Variables
    private BuildIndex loadLevelBuildIndex;
    private GameObject defaultSelectedLevelButton;
    private IList<Player> players;
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
            if (levels.Contains(script.GetBuildIndex()))
            {
                if (!defaultFound)
                {
                    defaultSelectedLevelButton = b.gameObject;
                    defaultFound = true;
                }
                b.interactable = true;
            }
            else
            {
                b.interactable = false;
            }
        }

        EventSystem.current.SetSelectedGameObject(defaultSelectedLevelButton);

        players = ReInput.players.AllPlayers;
    }

    private void Update()
    {
        bool cancel = false;
        foreach (Player p in players)
        {
            if (p.GetButtonDown("UICancel"))
            {
                cancel = true;
                break;
            }
        }

        if (cancel)
        {
            OnCancel();
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
        loadLevelBuildIndex = BuildIndex.CHARACTER_SELECT;
        OpenConfirmationMenu();
    }

    public void LoadLevel()
    {
        LevelSelect.LoadLevel(loadLevelBuildIndex);
    }

    private void OpenConfirmationMenu()
    {
        confirmationMenu.gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(defaultConfirmationGameObject);
    }
    #endregion
}
