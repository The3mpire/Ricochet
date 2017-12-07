using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class LevelSelect
{
    public const int MAIN_MENU = 0;
    public const int CHARACTER_SELECT = 1;
    public const int LEVEL_SELECT = 2;
    public const int DIAGONAL_ALLEY = 3;
    public const int ELEVATOR = 4;
    public const int UP_N_OVER = 5;

    public static List<int> glitchBallClassicLevels = new List<int>() {
            ELEVATOR,
            UP_N_OVER,
            DIAGONAL_ALLEY
        };

    public static void ReplayLevel()
    {

    }

    public static void LoadRandomLevel(List<int> levelSelection)
    {
        SceneManager.LoadSceneAsync(levelSelection[Random.Range(0, levelSelection.Count)]);
    }

    public static void LoadLevel(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName);
    }

    public static void LoadLevel(int buildIndex)
    {
        SceneManager.LoadSceneAsync(buildIndex);
    }

    public static void LoadCharacterSelect()
    {
        SceneManager.LoadSceneAsync(CHARACTER_SELECT);
    }

    public static void LoadLevelSelect()
    {
        SceneManager.LoadSceneAsync(LEVEL_SELECT);
    }

    public static void LoadMainMenu()
    {
        SceneManager.LoadSceneAsync(MAIN_MENU);
    }
}
