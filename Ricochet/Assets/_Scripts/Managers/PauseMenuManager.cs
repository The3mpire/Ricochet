using Rewired;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PauseMenuManager : MonoBehaviour
{
    #region Inspector Variables
    [Tooltip("The Pause gui")]
    [SerializeField]
    private GameObject pausePanel;
    [Tooltip("The button that is first highlighted")]
    [SerializeField]
    private GameObject defaultSelectedButton;
    [SerializeField]
    private GameDataSO gameData;
    [SerializeField]
    private AudioSource fxSource;
    #endregion

    #region Hidden Variables
    private Player lastPlayer = null;
    private GameManager gameManagerInstance;
    private List<Player> players = new List<Player>();
    #endregion

    #region MonoBehaviour
    public void Awake()
    {
        // Iterating through Players (excluding the System Player)
        for (int i = 0; i < ReInput.players.playerCount; i++)
        {
            players.Add(ReInput.players.Players[i]);
        }
    }

    public void Update()
    {
        foreach (Player p in players)
        {
            if (lastPlayer == null || p == lastPlayer)
            {
                if (p.GetButtonDown("UIMenu") && !pausePanel.activeSelf)
                {
                    pausePanel.SetActive(true);
                    //TODO play the pause soundddddd
                    EventSystem.current.SetSelectedGameObject(defaultSelectedButton);
                    Time.timeScale = 0;
                    lastPlayer = p;
                }
                else if (p.GetButtonDown("UIMenu") && pausePanel.activeSelf)
                {
                    ResumeGame();
                }
            }
        }
    }
    #endregion

    #region Button Functions
    public void ResumeGame()
    {
        pausePanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
        Time.timeScale = 1;
        lastPlayer = null;
    }

    public void MainMenu()
    {
        gameData.ResetGameStatistics();
        Time.timeScale = 1;
        LevelSelect.LoadMainMenu();
    }

    public void CharacterSelect()
    {
        gameData.ResetGameStatistics();
        Time.timeScale = 1;
        LevelSelect.LoadCharacterSelect();
    }

    public void LoadLevelSelect()
    {
        gameData.ResetGameStatistics();
        Time.timeScale = 1;
        LevelSelect.LoadLevelSelect();
    }
    #endregion
}
