using System.Collections;
using DG.Tweening;
using Enumerables;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EndGameManager : MonoBehaviour
{
    [Header("Game Stats")]
    [SerializeField] private GameDataSO gameData;
    [Tooltip("Drag the winning team text here")]
    [SerializeField] private Text _winTeamText;
    [Tooltip("Drag the winning message here")]
    [SerializeField] private Text _winMessageText;
    [SerializeField] private Transform _winMessageEnd;
    [Tooltip("Drag the winning team text here")]
    [SerializeField] private Text _redScoreText;
    [Tooltip("Drag the winning team text here")]
    [SerializeField] private Text _blueScoreText;

    [Header("Characters")]
    [SerializeField]
    [Tooltip("0 - Cop, 1 - Cat, 2 - Sushi, 3 - Computer")]
    private GameObject[] _panel1Characters = new GameObject[4];
    [SerializeField]
    [Tooltip("0 - Cop, 1 - Cat, 2 - Sushi, 3 - Computer")]
    private GameObject[] _panel2Characters = new GameObject[4];
    [SerializeField]
    [Tooltip("Position to move Character Panel 1 to.")]
    private Transform _charPanel1to;
    [SerializeField]
    [Tooltip("Position to move Character Panel 1 to.")]
    private Transform _charPanel2to;

    [Header("Menu")]
    [SerializeField] private GameObject _menuPanel;
    [SerializeField] private GameObject defaultSelectedButton;

    private Transform _charPanel1;
    private Transform _charPanel2;
    private Sequence _winTeamSequence;
    private Sequence _winMessageSequence;

    #region MonoBehaviours
    private void Start()
    {
        _charPanel1 = _panel1Characters[0].transform.parent.transform;
        _charPanel2 = _panel2Characters[0].transform.parent.transform;
        SetUpSequences();
        StartCoroutine(ShowGameWinner());
        ShowMenu();
        
    }
    #endregion

    #region Coroutines

    IEnumerator ShowGameWinner()
    {
        _winTeamSequence.Play();
        _winMessageSequence.Play();
        yield return _winTeamSequence.WaitForCompletion();
        _charPanel1.DOMove(_charPanel1to.position, 2f);
        _charPanel2.DOMove(_charPanel2to.position, 2f);
    }

    #endregion

    #region Private Functions

    private void SetUpSequences()
    {
        _winTeamSequence = DOTween.Sequence();
        _winTeamSequence.Append(_winTeamText.transform.DOPunchScale(Vector3.one, 2f, 2, 0f));
        _winTeamSequence.Pause();

        _winMessageSequence = DOTween.Sequence();
        _winMessageSequence.SetDelay(1f);
        _winMessageSequence.Append(_winMessageText.transform.DOMove(_winMessageEnd.position, 1f));
        _winMessageSequence.Append(_winMessageText.transform.DOPunchPosition(_winMessageText.transform.right*10, 2f, 10, 0));
        _winMessageSequence.Pause();
    }

    #endregion


    public void ShowMenu()
    {
        EventSystem.current.SetSelectedGameObject(defaultSelectedButton);
    }

    #region Menu Functions
    public void PlayAgain()
    {
        gameData.ResetGameStatistics();
        LevelSelect.LoadLevel(gameData.GetGameLevel());
    }

    public void PlayRandomLevel()
    {
        gameData.ResetGameStatistics();
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
    #endregion
}
