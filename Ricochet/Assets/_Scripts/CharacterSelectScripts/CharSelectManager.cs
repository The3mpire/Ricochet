#pragma warning disable CS0649

using System.Collections;
using System.Collections.Generic;
using Enumerables;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharSelectManager : MonoBehaviour
{
    #region Private
    [SerializeField]
    private GameDataSO gameData;

    [SerializeField]
    private SFXManager sfxManager;

    [SerializeField]
    [Tooltip("Amount of time to wait in seconds after all players are ready")]
    private float waitTime;

    [SerializeField] private CharacterSelectObjects[] _playerObjects;

    [Header("Misc UI Objects")]
    [SerializeField]
    [Tooltip("Drag timer text object here")]
    private Text timerText;
    [SerializeField]
    [Tooltip("Drag back timer here")]
    private Slider backSlider;

    [Header("Character UI Images")]
    [Tooltip("Character 1 Image (Cat)")]
    [SerializeField]
    private Image char1Image;
    [Tooltip("Character 2 Image (Computer)")]
    [SerializeField]
    private Image char2Image;
    [Tooltip("Character 3 Image (Mall Cop)")]
    [SerializeField]
    private Image char3Image;
    [Tooltip("Character 4 Image (Sushi)")]
    [SerializeField]
    private Image char4Image;
    [Tooltip("Default color")]
    [SerializeField]
    private Color defaultColor;

    [Header("Team Selection Panels")]
    [SerializeField] private GameObject char1TeamPanel;
    [SerializeField] private GameObject char2TeamPanel;
    [SerializeField] private GameObject char3TeamPanel;
    [SerializeField] private GameObject char4TeamPanel;
    [SerializeField] private GameObject pressToJoinPanel;

    [Header("Team Text")]
    [SerializeField] private Text char1TeamText;
    [SerializeField] private Text char2TeamText;
    [SerializeField] private Text char3TeamText;
    [SerializeField] private Text char4TeamText;

    private readonly SelectionPhase[] playerPhase = new SelectionPhase[4];
    private readonly PSettings[] playerSettings = new PSettings[4];
    private float timer;

    private bool timerActive;

    private bool bHeld;
    private bool _goingBack = false;

    public int playersIn = 0;
    #endregion

    #region Phase Enum
    private enum SelectionPhase
    {
        None,
        CharacterSelect,
        TeamSelect,
        Ready
    }
    #endregion

    #region MonoBehaviour
    private void Awake()
    {
        timerActive = false;
        bHeld = false;
        for(int i = 1; i<= 4;i++)
        {
            gameData.SetPlayerInactive(i);
        }
    }

    void Start()
    {
        timer = waitTime;
    }

    void Update()
    {
        if (bHeld)
        {
            backSlider.value += (Time.deltaTime * 1.75f);
            if (backSlider.value >= backSlider.maxValue)
            {
                RouteInputBack();
                _goingBack = true;
            }
        }
        else
        {
            backSlider.value -= Time.deltaTime * 2;
        }
        bHeld = false;
        if (CheckReady())
        {
            if (!timerActive)
            {
                timerActive = true;
                StartCoroutine(LevelSelectTimer());
            }
            CountdownToLevelSelect();
        }
        
    }
    #endregion

    #region Public
    /// <summary>
    /// Activates to given player. Enables selection token and moves player to Character Selection phase.
    /// </summary>
    /// <param name="playerNumber">Player ID to activate</param>
    public void ActivatePlayer(int playerNumber)
    {
        playerPhase[playerNumber] = SelectionPhase.None;
        playerSettings[playerNumber] = LoadPlayerSettings(playerNumber);
        if (playerSettings[playerNumber].Character == ECharacter.None)
        {
            playerSettings[playerNumber].Character = _playerObjects[playerNumber].DefaultToken.GetComponentInParent<CharacterInfo>().getCharacterId();
        }
        _playerObjects[playerNumber].Team = playerSettings[playerNumber].Team;
    }
    #endregion

    #region InputRouting
    public void RouteInputAxis(int playerNumber, int direction)
    {
        var phase = playerPhase[playerNumber];
        switch (phase)
        {
            case SelectionPhase.CharacterSelect:
                MoveSelectionToken(playerNumber, direction);
                sfxManager.PlayMenuTraversalSound();
                break;
            case SelectionPhase.TeamSelect:
                ChangeTeam(playerNumber, direction);
                sfxManager.PlayMenuTraversalSound();
                break;
            case SelectionPhase.Ready:
                break;
        }
    }

    public void RouteInputA(int playerNumber, Color playerColor)
    {
        var phase = playerPhase[playerNumber];
        switch (phase)
        {
            case SelectionPhase.None:
                PlayerJoin(playerNumber);
                sfxManager.PlayMenuClickSound();
                break;
            case SelectionPhase.CharacterSelect:
                SelectCharacter(playerNumber, playerColor);
                sfxManager.PlayMenuClickSound();
                break;
            case SelectionPhase.TeamSelect:
                SelectTeam(playerNumber);
                sfxManager.PlayMenuClickSound();
                break;
            case SelectionPhase.Ready:
                break;
        }
    }
    public void RouteInputB(int playerNumber)
    {
        var phase = playerPhase[playerNumber];
        switch (phase)
        {
            case SelectionPhase.CharacterSelect:
                break;
            case SelectionPhase.TeamSelect:
                ClearImages(playerNumber);
                ClearSelection(playerNumber);
                sfxManager.PlayMenuBackSound();
                break;
            case SelectionPhase.Ready:
                UndoReady(playerNumber);
                sfxManager.PlayMenuBackSound();
                break;
        }
    }
    public void RouteInputAltB(int playerNumber)
    {
        if (playerPhase[playerNumber] == SelectionPhase.CharacterSelect || playerPhase[playerNumber] == SelectionPhase.None)
        {
            bHeld = true;
        }
    }

    public void RouteInputBack()
    {
        if (!_goingBack)
        {
            sfxManager.PlayMenuBackSound();
            gameData.SetSkipToMode(true);
            LevelSelect.LoadMainMenu();
        }
    }
    #endregion

    #region Selection Functions
    private void MoveSelectionToken(int playerNumber, int direction)
    {
        _playerObjects[playerNumber].ActiveToken = _playerObjects[playerNumber].SwitchTokensToNext(_playerObjects[playerNumber].ActiveToken, direction);
    }

    private void SelectCharacter(int playerNumber, Color playerColor)
    {
        var color = Color.white;
        var selectedChar = ECharacter.None;
        foreach (var token in _playerObjects[playerNumber].Tokens)
        {
            if (token.activeInHierarchy)
            {
                var selected = token.GetComponentInParent<CharacterInfo>();
                if (selected.GetIsSelectable())
                {
                    selected.SetIsSelectable(false);
                    selectedChar = selected.getCharacterId();
                    gameData.SetPlayerCharacter(playerNumber, selected.getCharacterId());

                    token.GetComponent<Shadow>().enabled = false;
                    token.GetComponent<ParticleSystem>().Play();
                    playerPhase[playerNumber] = SelectionPhase.TeamSelect;
                }
            }
        }
        
        Color teamColor = GetTeamColor(_playerObjects[playerNumber].Team);
        string teamText;
        if(_playerObjects[playerNumber].Team == ETeam.RedTeam)
            teamText = "Red Team";
        else
            teamText = "Blue Team";
        switch (selectedChar)
        {
            case ECharacter.Cat:
                char1Image.color = teamColor;
                char1TeamPanel.SetActive(true);
                char1TeamText.text = teamText;
                break;
            case ECharacter.Computer:
                char2Image.color = teamColor;
                char2TeamPanel.SetActive(true);
                char2TeamText.text = teamText;
                break;
            case ECharacter.MallCop:
                char3Image.color = teamColor;
                char3TeamPanel.SetActive(true);
                char3TeamText.text = teamText;
                break;
            case ECharacter.Sushi:
                char4Image.color = teamColor;
                char4TeamPanel.SetActive(true);
                char4TeamText.text = teamText;
                break;
        }
    }

    private void ChangeTeam(int playerNumber, int direction)
    {
        //TODO: Move to CharacterSelectObjects class?
        _playerObjects[playerNumber].Team = GetNextTeam(_playerObjects[playerNumber].Team);
        ECharacter selectedChar = gameData.GetPlayerCharacter(playerNumber);
        string teamText;
        if(_playerObjects[playerNumber].Team == ETeam.RedTeam)
            teamText = "Red Team";
        else
            teamText = "Blue Team";
        switch (selectedChar)
        {
            case ECharacter.Cat:
                char1Image.color = GetTeamColor(_playerObjects[playerNumber].Team);
                char1TeamText.text = teamText;
                break;
            case ECharacter.Computer:
                char2Image.color = GetTeamColor(_playerObjects[playerNumber].Team);
                char2TeamText.text = teamText;
                break;
            case ECharacter.MallCop:
                char3Image.color = GetTeamColor(_playerObjects[playerNumber].Team);
                char3TeamText.text = teamText;
                break;
            case ECharacter.Sushi:
                char4Image.color = GetTeamColor(_playerObjects[playerNumber].Team);
                char4TeamText.text = teamText;
                break;
        }
    }

    private void SelectTeam(int playerNumber)
    {
        playerPhase[playerNumber] = SelectionPhase.Ready;
        gameData.SetPlayerTeam(playerNumber, _playerObjects[playerNumber].Team);
        ECharacter selectedChar = gameData.GetPlayerCharacter(playerNumber);
        switch (selectedChar)
        {
            case ECharacter.Cat:
                char1TeamText.text = "Ready";
                break;
            case ECharacter.Computer:
                char2TeamText.text = "Ready";
                break;
            case ECharacter.MallCop:
                char3TeamText.text = "Ready";
                break;
            case ECharacter.Sushi:
                char4TeamText.text = "Ready";
                break;
        }
    }

    private void PlayerJoin(int playerNumber)
    {
        playerPhase[playerNumber] = SelectionPhase.CharacterSelect;
        playersIn++;
        if(playersIn == 4)
        {
            pressToJoinPanel.SetActive(false);
        }
        _playerObjects[playerNumber].DefaultToken.SetActive(true);
        _playerObjects[playerNumber].ActiveToken = _playerObjects[playerNumber].DefaultToken;
        if (playerSettings[playerNumber] == null)
        {
            playerSettings[playerNumber] = LoadPlayerSettings(playerNumber);
        }
        _playerObjects[playerNumber].ActiveToken = MoveSelectionTokenTo(playerSettings[playerNumber].Character, _playerObjects[playerNumber].ActiveToken, _playerObjects[playerNumber].Tokens);
        gameData.SetPlayerActive(playerNumber + 1);
    }

    #endregion

    #region Private Helpers
    private bool CheckReady()
    {
        var allReady = true;
        int readyCount = 0;
        foreach (var item in playerPhase)
        {
            if (item == SelectionPhase.CharacterSelect || item == SelectionPhase.TeamSelect)
            {
                allReady = false;
                break;
            }
            if (item == SelectionPhase.Ready)
            {
                readyCount++;
            }
        }

        if (readyCount < 1)
        {
            allReady = false;
        }

        return allReady;
    }

    private IEnumerator LevelSelectTimer()
    {
        yield return new WaitForSeconds(waitTime);
        LevelSelect.LoadLevelSelect();
    }

    private void CountdownToLevelSelect()
    {
        timer -= Time.deltaTime;
        int seconds = (int)(timer % 60);
        if (seconds >= 0)
        {
            timerText.text = "Level Select in: " + (seconds + 1);
        }
    }

    private ETeam GetNextTeam(ETeam team)
    {
        switch (team)
        {
            case ETeam.BlueTeam:
                return ETeam.RedTeam;
            case ETeam.RedTeam:
                return ETeam.BlueTeam;
            case ETeam.None:
                return ETeam.BlueTeam;
            default:
                return ETeam.BlueTeam;
        }
    }

    private Color GetTeamColor(ETeam team)
    {
        switch (team)
        {
            case ETeam.BlueTeam:
                return new Color32(102, 141, 229,255);
            case ETeam.RedTeam:
                return new Color32(237, 109, 121,255);
            case ETeam.None:
                return new Color32(187, 150, 255, 255);
            default:
                return Color.grey;
        }
    }

    private PSettings LoadPlayerSettings(int playerNumber)
    {
        var character = gameData.GetPlayerCharacter(playerNumber);
        var team = gameData.GetPlayerTeam(playerNumber);
        if (character == ECharacter.None)
        {
            character = ECharacter.Cat;
        }
        
        return new PSettings(character, team);
    }

    private GameObject MoveSelectionTokenTo(ECharacter character, GameObject active, IEnumerable<GameObject> tokens)
    {
        foreach (var token in tokens)
        {
            if (token.GetComponentInParent<CharacterInfo>().getCharacterId() != character) continue;
            SwitchActiveToken(active, token);
            return token;
        }
        return null;
    }

    private void SwitchActiveToken(GameObject fromToken, GameObject toToken)
    {
        fromToken.SetActive(false);
        toToken.SetActive(true);
    }

    private void ClearImages(int playerNumber)
    {
        var selectedImage = _playerObjects[playerNumber].ActiveToken.GetComponentInParent<CharacterInfo>();
        selectedImage.SetIsSelectable(true);

        switch (selectedImage.getCharacterId())
        {
            case ECharacter.Cat:
                char1Image.color = defaultColor;
                char1TeamPanel.SetActive(false);
                break;
            case ECharacter.Computer:
                char2Image.color = defaultColor;
                char2TeamPanel.SetActive(false);
                break;
            case ECharacter.MallCop:
                char3Image.color = defaultColor;
                char3TeamPanel.SetActive(false);
                break;
            case ECharacter.Sushi:
                char4Image.color = defaultColor;
                char4TeamPanel.SetActive(false);
                break;
        }
    }

    /// <summary>
    /// Reset player's character selection to none. Re-enable character selecting for player.
    /// </summary>
    /// <param name="playerNumber"></param>
    private void ClearSelection(int playerNumber)
    {
        var color = Color.white;
        gameData.SetPlayerCharacter(playerNumber, ECharacter.None);
        _playerObjects[playerNumber].ActiveToken.GetComponent<Shadow>().enabled = true;
        _playerObjects[playerNumber].ActiveToken.GetComponent<ParticleSystem>().Stop();
        playerPhase[playerNumber] = SelectionPhase.CharacterSelect;
        //_playerObjects[playerNumber].TeamPanel.color = GetTeamColor(ETeam.None);
    }

    private void UndoReady(int playerNumber)
    {
        StopAllCoroutines();
        timerActive = false;
        timer = waitTime;
        timerText.text = "Waiting...";
        playerPhase[playerNumber] = SelectionPhase.TeamSelect;

        ECharacter selectedChar = gameData.GetPlayerCharacter(playerNumber);
        string teamText;
        if(_playerObjects[playerNumber].Team == ETeam.RedTeam)
            teamText = "Red Team";
        else
            teamText = "Blue Team";
        switch (selectedChar)
        {
            case ECharacter.Cat:
                char1Image.color = GetTeamColor(_playerObjects[playerNumber].Team);
                char1TeamText.text = teamText;
                break;
            case ECharacter.Computer:
                char2Image.color = GetTeamColor(_playerObjects[playerNumber].Team);
                char2TeamText.text = teamText;
                break;
            case ECharacter.MallCop:
                char3Image.color = GetTeamColor(_playerObjects[playerNumber].Team);
                char3TeamText.text = teamText;
                break;
            case ECharacter.Sushi:
                char4Image.color = GetTeamColor(_playerObjects[playerNumber].Team);
                char4TeamText.text = teamText;
                break;
        }
    }
    #endregion
}
