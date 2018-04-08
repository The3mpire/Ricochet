using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Enumerables;

public static class LevelSelect
{
    static LevelSelect()
    {
        SceneManager.sceneLoaded += PlaySceneTransitionSFX;
    }

    public static List<BuildIndex> glitchBallClassicLevels = new List<BuildIndex>() {
            BuildIndex.ELEVATOR,
            BuildIndex.UP_N_OVER_WIDE,
            BuildIndex.DIAGONAL_ALLEY,
            BuildIndex.EMPTY_LEVEL
        };

    public static void LoadRandomLevel(List<BuildIndex> levelSelection)
    {
        SceneManager.LoadSceneAsync((int)levelSelection[Random.Range(0, levelSelection.Count)]);
    }

    public static void LoadEndGameScene()
    {
        SceneManager.LoadSceneAsync((int)BuildIndex.END_GAME);
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
        SceneManager.LoadSceneAsync((int)BuildIndex.MAIN_MENU);
    }

    private static void PlaySceneTransitionSFX(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex != (int)BuildIndex.END_GAME)
        {
            SFXManager sfx;
            if (SFXManager.TryGetInstance(out sfx))
            {
                sfx.PlaySceneTraversalSound();
            }
        }
    }
}
