using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Enumerables;
using Rewired;

public class MenuManager : MonoBehaviour
{
    #region Inspector Variables
    [Tooltip("The Pause gui")]
    [SerializeField]
    private GameObject pausePanel;
    [Tooltip("The button that is first highlighted")]
    [SerializeField]
    private GameObject defaultSelectedButton;
    #endregion

    #region Hidden Variables
    private bool paused = false;
    private GameManager gameManagerInstance;

    #endregion

    #region MonoBehaviour
    void Awake()
    {
        if(SceneManager.GetActiveScene().buildIndex == LevelIndex.MAIN_MENU)
        {
            EventSystem.current.SetSelectedGameObject(defaultSelectedButton);
        }
    }
    #endregion

    #region Button Functions
    public void PauseGame()
    {
        pausePanel.SetActive(true);
        paused = true;
        Cursor.visible = true;
        Time.timeScale = 0;
        EventSystem.current.SetSelectedGameObject(pausePanel.GetComponentInChildren<Button>().gameObject);
    }

    public void UnpauseGame()
    {
        EventSystem.current.SetSelectedGameObject(null);
        pausePanel.SetActive(false);
        paused = false;
        Time.timeScale = 1;
    }

    public void ExitLevel()
    {
        Time.timeScale = 1;
        pausePanel.SetActive(false);
        if (gameManagerInstance != null || GameManager.TryGetInstance(out gameManagerInstance))
        {
            gameManagerInstance.ExitLevel();
        }
    }
    
    public void StartGame()
    {
        if (gameManagerInstance != null || GameManager.TryGetInstance(out gameManagerInstance))
        {
            gameManagerInstance.CharacterSelect();
        }
    }
    
    public void ExitGame()
    {
        if (gameManagerInstance != null || GameManager.TryGetInstance(out gameManagerInstance))
        {
            gameManagerInstance.ExitGame();
        }
    }
    #endregion

    #region Getters And Setters
    public bool GetPaused()
    {
        return paused;
    }
    #endregion
}
