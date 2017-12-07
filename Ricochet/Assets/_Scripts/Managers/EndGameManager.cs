using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EndGameManager : MonoBehaviour
{
    [Tooltip("Drag the UI's EndGameText here")]
    [SerializeField] private Text EndGameText;
    [SerializeField] private GameObject defaultSelectedButton;

    #region MonoBehaviours
    private void Start()
    {
        ShowGameOverMenu();
        EndGameText.text = GameData.gameWinner == Enumerables.ETeam.None ? "Tie Game!" : "Congratulations " + GameData.gameWinner.ToString() + "!";
    }
    #endregion

    public void ShowGameOverMenu()
    {
        EventSystem.current.SetSelectedGameObject(defaultSelectedButton);
    }

    public void PlayAgain()
    {
        GameData.ResetGameStatistics();
        LevelSelect.LoadLevel(GameData.GameLevel);
    }

    public void PlayRandomLevel()
    {
        GameData.ResetGameStatistics();
        // TODO: Switch here on GameData game type to load from correct playlist
        LevelSelect.LoadRandomLevel(LevelSelect.glitchBallClassicLevels);
    }

    public void LoadLevelSelect()
    {
        GameData.ResetGameStatistics();
        LevelSelect.LoadLevelSelect();
    }

    public void LoadCharacterSelect()
    {
        GameData.ResetGameStatistics();
        LevelSelect.LoadCharacterSelect();
    }

    public void LoadMainMenu()
    {
        GameData.ResetGameStatistics();
        GameData.ResetGameSetup();
        LevelSelect.LoadMainMenu();
    }
}
