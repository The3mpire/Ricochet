using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Enumerables;

public static class LevelSelect
{
    public static List<BuildIndex> glitchBallClassicLevels = new List<BuildIndex>() {
            BuildIndex.ELEVATOR,
            BuildIndex.UP_N_OVER_WIDE,
            BuildIndex.DIAGONAL_ALLEY,
            BuildIndex.EMPTY_LEVEL
        };

    public static void LoadRandomLevel(List<BuildIndex> levelSelection)
    {
        PlaySceneTransitionSFX();
        SceneManager.LoadSceneAsync((int)levelSelection[Random.Range(0, levelSelection.Count)]);
    }

    public static void LoadEndGameScene()
    {
        PlaySceneTransitionSFX();
        SceneManager.LoadSceneAsync((int)BuildIndex.END_GAME);
    }

    public static void LoadLevel(string sceneName)
    {
        PlaySceneTransitionSFX();
        SceneManager.LoadSceneAsync(sceneName);
    }

    public static void LoadLevel(int buildIndex)
    {
        PlaySceneTransitionSFX();
        SceneManager.LoadSceneAsync(buildIndex);
    }

    public static void LoadLevel(BuildIndex buildIndex)
    {
        PlaySceneTransitionSFX();
        SceneManager.LoadSceneAsync((int)buildIndex);
    }

    public static void LoadCharacterSelect()
    {
        PlaySceneTransitionSFX();
        SceneManager.LoadSceneAsync((int)BuildIndex.CHARACTER_SELECT);
    }

    public static void LoadLevelSelect()
    {
        PlaySceneTransitionSFX();
        SceneManager.LoadSceneAsync((int)BuildIndex.LEVEL_SELECT);
    }

    public static void LoadMainMenu()
    {
        PlaySceneTransitionSFX();
        SceneManager.LoadSceneAsync((int)BuildIndex.MAIN_MENU);
    }

    private static void PlaySceneTransitionSFX()
    {
        SFXManager sfx;
        if (SFXManager.TryGetInstance(out sfx))
        {
            sfx.PlaySceneTraversalSound();
        }
    }
}
