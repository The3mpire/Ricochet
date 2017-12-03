using UnityEngine;
using UnityEngine.EventSystems;

public class UI_GameOverMenu : MonoBehaviour
{
    [SerializeField] private GameObject defaultSelectedButton;

    #region MonoBehaviours
    private void Start()
    {
        ShowGameOverMenu();
    }
    #endregion

    public void ShowGameOverMenu()
    {
        EventSystem.current.SetSelectedGameObject(defaultSelectedButton);
    }

    public void PlayAgain()
    {
       // TODO: Once GameData stores the level being played, then we can load that level again here.
    }

    public void PlayRandomLevel()
    {
        // TODO: Switch here on GameData game type to load from correct playlist
        LevelSelect.LoadRandomLevel(LevelSelect.glitchBallClassicLevels);
    }

    public void LoadLevelSelect()
    {
        LevelSelect.LoadLevelSelect();
    }

    public void LoadCharacterSelect()
    {
        LevelSelect.LoadCharacterSelect();
    }

    public void LoadMainMenu()
    {
        LevelSelect.LoadMainMenu();
    }
}
