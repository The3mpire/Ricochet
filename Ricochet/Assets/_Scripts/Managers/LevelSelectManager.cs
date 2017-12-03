using DG.Tweening;
using Rewired;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectManager : MonoBehaviour
{

    #region Inspector Variables
    [SerializeField] private RectTransform levelSelectMenu;
    [SerializeField] private RectTransform settingsMenu;

    [SerializeField] private GameObject defaultSelectedLevelButton;
    [SerializeField] private GameObject defaultSelectedSettingsItem;

    [SerializeField] private string loadLevelName;
    #endregion

    #region Private Variables
    private bool levelSelected;
    private IList<Player> players;
    #endregion

    #region Monobehaviours
    private void Awake()
    {
        levelSelected = false;
    }

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(defaultSelectedLevelButton);

        players = ReInput.players.AllPlayers;
    }

    private void Update()
    {
        if (levelSelected)
        {
            bool cancel = false;
            foreach (Player p in players)
            {
                if (p.GetButton("UICancel"))
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
        foreach(Player p in players)
        {
            if (p.GetButtonDown("UIMenu"))
            {
                SceneManager.LoadScene("CharacterSelect");
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
        EventSystem.current.SetSelectedGameObject(defaultSelectedSettingsItem);
        settingsMenu.DOLocalMove(Vector3.zero, .4f);
        levelSelectMenu.DOLocalMove(Vector3.up * 400f, .4f);
        levelSelected = true;
    }

    public void OnCancel()
    {
        EventSystem.current.SetSelectedGameObject(defaultSelectedLevelButton);
        settingsMenu.DOLocalMove(Vector3.down * 400, .4f);
        levelSelectMenu.DOLocalMove(Vector3.zero, .4f);
        levelSelected = false;
    }

    public void LoadLevel()
    {
        SceneManager.LoadSceneAsync(loadLevelName);
    }
    #endregion
}
