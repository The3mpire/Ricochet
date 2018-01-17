using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Enumerables;

public class MenuManager : MonoBehaviour
{
    #region Inspector Variables
    [Tooltip("Whether this scene has a pause panel")]
    [SerializeField]
    private bool hasPausePanel;
    [Tooltip("The Pause gui")]
    [SerializeField]
    private GameObject pausePanel;
    [Tooltip("The button that is first highlighted")]
    [SerializeField]
    private GameObject defaultSelectedButton;
    #endregion

    #region Hidden Variables
    private bool pauseMenuShowing = false;
    private GameManager gameManagerInstance;

    #endregion

    #region MonoBehaviour
    void Awake()
    {
        Cursor.visible = true;
        if (SceneManager.GetActiveScene().buildIndex == LevelIndex.MAIN_MENU)
        {
            EventSystem.current.SetSelectedGameObject(defaultSelectedButton);
        }
    }

    void Update()
    {
        if (hasPausePanel && Input.GetButtonDown("Cancel") && !pauseMenuShowing)
        {
            pausePanel.SetActive(true);
            EventSystem.current.SetSelectedGameObject(defaultSelectedButton);
            pauseMenuShowing = true;
            Cursor.visible = true;
            Time.timeScale = 0;
        }
        else if (hasPausePanel && Input.GetButtonDown("Cancel") && pauseMenuShowing)
        {
            pausePanel.SetActive(false);
            EventSystem.current.SetSelectedGameObject(null);
            pauseMenuShowing = false;
            Time.timeScale = 1;
        }
    }
    #endregion


    #region Button Functions
    public void ResumeGame()
    {
        if (hasPausePanel)
        {
            pausePanel.SetActive(false);
        }
        EventSystem.current.SetSelectedGameObject(null);
        pauseMenuShowing = false;
        Time.timeScale = 1;
    }

    public void ExitLevel()
    {
        Time.timeScale = 1;

        if (hasPausePanel)
        {
            pausePanel.SetActive(false);
        }

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
}
