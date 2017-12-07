using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Enumerables;

public static class LevelSelect
{
    public static List<BuildIndex> glitchBallClassicLevels = new List<BuildIndex>() {
            BuildIndex.ELEVATOR,
            BuildIndex.UP_N_OVER,
            BuildIndex.DIAGONAL_ALLEY
        };

    public static void LoadRandomLevel(List<BuildIndex> levelSelection)
    {
        SceneManager.LoadSceneAsync((int)levelSelection[Random.Range(0, levelSelection.Count)]);
    }

    public static void LoadLevel(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName);
    }

    public static void LoadLevel(int buildIndex)
    {
        SceneManager.LoadSceneAsync(buildIndex);
    }

    public static void LoadLevel(BuildIndex buildIndex)
    {
        SceneManager.LoadSceneAsync((int)buildIndex);
    }

    public static void LoadCharacterSelect()
    {
        SceneManager.LoadSceneAsync((int)BuildIndex.CHARACTER_SELECT);
    }

    public static void LoadLevelSelect()
    {
        SceneManager.LoadSceneAsync((int)BuildIndex.LEVEL_SELECT);
    }

    public static void LoadMainMenu()
    {
        SceneManager.LoadSceneAsync((int)BuildIndex.LEVEL_SELECT);
    }
}
