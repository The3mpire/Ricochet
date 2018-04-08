using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EndGameManager : MonoBehaviour
{
    [Tooltip("Drag the UI's EndGameText here")]
    [SerializeField] private Text EndGameText;
    [SerializeField] private GameObject defaultSelectedButton;
    [SerializeField] private GameDataSO gameData;

    private SFXManager sfxManager;

    #region MonoBehaviours
    private void Start()
    {
        ShowGameOverMenu();
        EndGameText.text = (gameData.GetGameWinner() == Enumerables.ETeam.None) ? "Tie Game!" : "Congratulations " + gameData.GetGameWinner().ToString() + "!"; 
        if (SFXManager.TryGetInstance(out sfxManager))
        {
            sfxManager.PlayTeamWinSound(gameData.GetGameWinner());
        }
        
    }
    #endregion

    public void ShowGameOverMenu()
    {
        EventSystem.current.SetSelectedGameObject(defaultSelectedButton);
    }

    public void PlayAgain()
    {
        gameData.ResetGameStatistics();
        LevelSelect.LoadLevel(gameData.GetGameLevel());
    }

    public void PlayRandomLevel()
    {
        gameData.ResetGameStatistics();
        // TODO: Switch here on GameData game type to load from correct playlist
        LevelSelect.LoadRandomLevel(LevelSelect.glitchBallClassicLevels);
    }

    public void LoadLevelSelect()
    {
        gameData.ResetGameStatistics();
        LevelSelect.LoadLevelSelect();
    }

    public void LoadCharacterSelect()
    {
        gameData.ResetGameStatistics();
        LevelSelect.LoadCharacterSelect();
    }

    public void LoadMainMenu()
    {
        gameData.ResetGameStatistics();
        LevelSelect.LoadMainMenu();
    }
}
