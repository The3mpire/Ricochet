using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Enumerables;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EndGameManager : MonoBehaviour
{
    [Header("Game Stats")]
    [SerializeField] private GameDataSO gameData;
    [SerializeField] private Transform _tickerObject;
    [SerializeField] private Transform _tickerEnd;
    [SerializeField] private float _tickerDuration;
    [Tooltip("Drag the winning team text here")]
    [SerializeField] private Text _winTeamText;
    [Tooltip("Drag the winning message here")]
    [SerializeField] private Text _winMessageText;
    [Tooltip("Drag the winning team text here")]
    [SerializeField] private Text _redScoreText;
    [Tooltip("Drag the winning team text here")]
    [SerializeField] private Text _blueScoreText;

    [Header("Characters")]
    [SerializeField]
    [Tooltip("0 - Cop, 1 - Cat, 2 - Sushi, 3 - Computer, 4 - Tie Game")]
    private GameObject[] _panelCharacters = new GameObject[5];
    [SerializeField]
    [Tooltip("Position to move Character Panel 1 to.")]
    private Transform _charPanelTo;

    [Header("Menu")]
    [SerializeField] private GameObject _menuPanel;
    [SerializeField] private GameObject defaultSelectedButton;

    private Transform _charPanel;
    private Sequence _winTeamSequence;
    private Sequence _winMessageSequence;
    private int _blueScore = 0;
    private int _redScore = 0;
    private ETeam _winTeam = ETeam.None;
    private List<ECharacter> _winCharacters = new List<ECharacter>();
    private int charCount;

    #region MonoBehaviours
    private void Start()
    {
        _charPanel = _panelCharacters[0].transform.parent.transform;

        LoadRoundStats();
        SetStatText();
        RunTicker();
        ShowMenu();
        MoveCharacterPanel();

    }
    #endregion

    #region Coroutines


    #endregion

    #region Private Functions

    private void SwapCharacter()
    {
        int selected = -1;
        
        switch (_winCharacters[charCount])
        {
            case ECharacter.MallCop:
                selected = 0;
                break;
            case ECharacter.Cat:
                selected = 1;
                break;
            case ECharacter.Sushi:
                selected = 2;
                break;
            case ECharacter.Computer:
                selected = 3;
                break;
            default:
                selected = -1;
                break;
        }

        for (int i = 0; i < _panelCharacters.Length; i++)
        {
            if (i == selected)
            {
                _panelCharacters[i].SetActive(true);
            }
            else
            {
                _panelCharacters[i].SetActive(false);
            }
        }

        if (charCount >= _winCharacters.Count-1)
        {
            charCount = 0;
        }
        else
        {
            charCount++;
        }

    }

    private void MoveCharacterPanel()
    {
        if (_winCharacters.Count == 0)
        {
            _panelCharacters[4].SetActive(true);
        }
        Vector3 charStartPos = _charPanel.position;
        Sequence charSequence = DOTween.Sequence();
        charSequence.Append(_charPanel.DOMove(_charPanelTo.position, 2f));
        charSequence.AppendInterval(5f);
        charSequence.PrependCallback(SwapCharacter);
        if (_winCharacters.Count == 0)
        {
            charSequence.SetLoops(0);
        }
        else
        {
            charSequence.Append(_charPanel.DOMove(charStartPos, 2f));
            charSequence.SetLoops(-1);
        }
        charSequence.Play();
    }

    private void LoadRoundStats()
    {
        _blueScore = gameData.GetBlueScore();
        _redScore = gameData.GetRedScore();
        _winTeam = gameData.GetGameWinner();
        ETeam[] playerTeams = gameData.GetPlayerTeams();
        for (int i = 0; i<playerTeams.Length;i++)    
        {
            if (playerTeams[i] == _winTeam && gameData.GetPlayerCharacter(i) != ECharacter.None && gameData.GetActive(i+1))
            {
                _winCharacters.Add(gameData.GetPlayerCharacter(i));
            }
        }
    }

    private void SetStatText()
    {
        _blueScoreText.text = "Blue score: " + _blueScore;
        _blueScoreText.color = Color.blue;
        _redScoreText.text = "Red score: " + _redScore;
        _redScoreText.color = Color.red;
        _winTeamText.text = _winTeam == ETeam.BlueTeam ? "Blue Team" : _winTeam == ETeam.RedTeam ? "Red Team": "Tie Game!";
        _winTeamText.color = _winTeam == ETeam.BlueTeam ? Color.blue : _winTeam == ETeam.RedTeam ? Color.red : Color.green;
        if (_winTeam == ETeam.None)
        {
            _winMessageText.text = "Step it up!!";
        }

    }

    private void RunTicker()
    {
        Tween tickerTween = _tickerObject.DOMove(_tickerEnd.position, _tickerDuration).SetEase(Ease.Linear).SetLoops(-1);
    }
    #endregion

    #region Menu Functions
    public void ShowMenu()
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
