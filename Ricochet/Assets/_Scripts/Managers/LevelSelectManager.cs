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
    [SerializeField] private RectTransform levelSelectMenu;
    [SerializeField] private RectTransform settingsMenu;
    [SerializeField] private RectTransform confirmationMenu;

    [SerializeField] private GameObject defaultOptionsGameObject;
    [SerializeField] private GameObject defaultConfirmationGameObject;

    [SerializeField] private BuildIndex loadLevelBuildIndex;
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
        bool options = false;
        bool cancel = false;
        foreach (Player p in players)
        {
            if (p.GetButtonDown("UICancel"))
            {
                cancel = true;
                break;
            }
            if (p.GetButtonDown("UIOptions"))
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

    public void SetLoadLevel(BuildIndex levelInt)
    {
        loadLevelBuildIndex = levelInt;
        OpenConfirmationMenu();
    }
           
    public void OpenOptionsMenu()
    {
        optionsOpen = true;
        settingsMenu.DOLocalMove(Vector3.left * 200, .4f);
        EventSystem.current.SetSelectedGameObject(defaultOptionsGameObject);
    }

    public void CloseConfirmationMenu()
    {
        confirmationMenu.gameObject.SetActive(false);
        EventSystem.current.SetSelectedGameObject(defaultSelectedLevelButton);
    }

    public void OnCancel()
    {
        if (optionsOpen)
        {
            optionsOpen = false;
            settingsMenu.DOLocalMove(Vector3.right * 500, .4f);
            EventSystem.current.SetSelectedGameObject(defaultSelectedLevelButton);
        }
        else
        {
            loadLevelBuildIndex = BuildIndex.CHARACTER_SELECT;
            OpenConfirmationMenu();
        }
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
