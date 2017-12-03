using DG.Tweening;
using Rewired;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelSelectManager : MonoBehaviour
{
    #region Inspector Variables
    [SerializeField] private RectTransform levelSelectMenu;
    [SerializeField] private RectTransform settingsMenu;

    [SerializeField] private GameObject defaultOptionsGameObject;

    [SerializeField] private string loadLevelName;
    [SerializeField] private Button[] levelButtons;
    #endregion

    #region Private Variables
    private GameObject defaultSelectedLevelButton;
    private bool optionsOpen;
    private IList<Player> players;
    #endregion

    #region Monobehaviours
    private void Awake()
    {
        optionsOpen = false;
    }

    private void Start()
    {
        List<int> levels = LevelSelect.glitchBallClassicLevels;
        // TODO: Switch statement here that looks at GameData for game type enum and sets the appropriate levels

        bool defaultFound = false;

        foreach (Button b in levelButtons)
        {
            UI_MenuButton script = b.GetComponent<UI_MenuButton>();
            if (levels.Contains(script.GetValue()))
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
        if (optionsOpen)
        {
            bool options = false;
            bool cancel = false;
            foreach (Player p in players)
            {
                if (p.GetButton("UICancel"))
                {
                    cancel = true;
                    break;
                }
                if (p.GetButton("UIMenu"))
                {
                    options = true;
                    break;
                }
            }

            if (cancel)
            {
                OnCancel();
            }

            if (options)
            {
                OpenOptionsMenu();
            }
        }

    }
    #endregion

    #region Public Functions
    public void SetMatchScoreLimit(Slider slider)
    {
        GameData.matchScoreLimit = (int)slider.value;
    }

    public void SetMatchTimeLimit(Slider slider)
    {
        GameData.matchTimeLimit = (int)slider.value;
    }

    public void SetLoadLevel(string levelName)
    {
        loadLevelName = levelName;
        LevelSelect.LoadLevel(levelName);
    }

    public void OpenOptionsMenu()
    {
        optionsOpen = true;
        settingsMenu.DOLocalMove(Vector3.left * 200, .4f);
        EventSystem.current.SetSelectedGameObject(defaultOptionsGameObject);
    }

    public void OnCancel()
    {
        if (optionsOpen)
        {
            optionsOpen = false;
            settingsMenu.DOLocalMove(Vector3.right * 200, .4f);
            EventSystem.current.SetSelectedGameObject(defaultSelectedLevelButton);
        }
        else
        {
            LevelSelect.LoadCharacterSelect();
        }
    }

    public void LoadLevel()
    {
        LevelSelect.LoadLevel(loadLevelName);
    }
    #endregion
}
