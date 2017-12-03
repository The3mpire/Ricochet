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
    [Tooltip("Amount of time to wait in seconds after all players are ready")]
    private float waitTime;

    [Header("Player 1")]
    [SerializeField]
    [Tooltip("List of all Player 1 character tokens")]
    private List<GameObject> p1Tokens;
    [SerializeField]
    [Tooltip("Drag player 1 Team panel SelectedCharImage here.")]
    private Image p1TeamImage;
    [SerializeField]
    [Tooltip("Drag default active token for Player 1 here")]
    private GameObject p1DefaultToken;
    private GameObject p1ActiveToken;
    [SerializeField]
    [Tooltip("Drag player 1 Team panel here.")]
    private Image p1TeamPanel;
    private Enumerables.ETeam p1Team;

    [Header("Player 2")]
    [SerializeField]
    [Tooltip("List of all Player 2 character tokens")]
    private List<GameObject> p2Tokens;
    [SerializeField]
    [Tooltip("Drag player 2 Team panel SelectedCharImage here.")]
    private Image p2TeamImage;
    [SerializeField]
    [Tooltip("Drag default active token for Player 2 here")]
    private GameObject p2DefaultToken;
    private GameObject p2ActiveToken;
    [SerializeField]
    [Tooltip("Drag player 2 Team panel here.")]
    private Image p2TeamPanel;
    private Enumerables.ETeam p2Team;

    [Header("Player 3")]
    [SerializeField]
    [Tooltip("List of all Player 3 character tokens")]
    private List<GameObject> p3Tokens;
    [SerializeField]
    [Tooltip("Drag player 3 Team panel SelectedCharImage here.")]
    private Image p3TeamImage;
    [SerializeField]
    [Tooltip("Drag default active token for Player 3 here")]
    private GameObject p3DefaultToken;
    private GameObject p3ActiveToken;
    [SerializeField]
    [Tooltip("Drag player 3 Team panel here.")]
    private Image p3TeamPanel;
    private Enumerables.ETeam p3Team;

    [Header("Player 4")]
    [SerializeField]
    [Tooltip("List of all Player 4 character tokens")]
    private List<GameObject> p4Tokens;
    [SerializeField]
    [Tooltip("Drag player 4 Team panel SelectedCharImage here.")]
    private Image p4TeamImage;
    [SerializeField]
    [Tooltip("Drag default active token for Player 4 here")]
    private GameObject p4DefaultToken;
    private GameObject p4ActiveToken;
    [SerializeField]
    [Tooltip("Drag player 4 Team panel here.")]
    private Image p4TeamPanel;
    private Enumerables.ETeam p4Team;

    [Header("Ready Text Objects")]
    [SerializeField]
    [Tooltip("Drag all ready text objects for each player in order")]
    private GameObject[] readyTags;
    [SerializeField]
    [Tooltip("Drag timer text object here")]
    private Text timerText;

    private SelectionPhase[] playerPhase = new SelectionPhase[4];
    private float timer;
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
    void Start()
    {
        SetActiveTokens();
        SetDefaultTeams();
        timer = waitTime;
        GameData.playerCharacters = new ECharacter[4];
        GameData.playerTeams = new ETeam[4];
    }

    void Update()
    {
        if (CheckReady())
        {
            StartCoroutine(LevelSelectTimer());
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
        switch (playerNumber)
        {
            case 0:
                p1DefaultToken.SetActive(true);
                playerPhase[0] = SelectionPhase.CharacterSelect;
                break;
            case 1:
                p2DefaultToken.SetActive(true);
                playerPhase[1] = SelectionPhase.CharacterSelect;
                break;
            case 2:
                p3DefaultToken.SetActive(true);
                playerPhase[2] = SelectionPhase.CharacterSelect;
                break;
            case 3:
                p4DefaultToken.SetActive(true);
                playerPhase[3] = SelectionPhase.CharacterSelect;
                break;
        }
    }
    #endregion

    #region InputRouting
    public void RouteInputAxis(int playerNumber, int direction)
    {
        var phase = playerPhase[playerNumber - 1];
        switch (phase)
        {
            case SelectionPhase.CharacterSelect:
                MoveSelectionToken(playerNumber, direction);
                break;
            case SelectionPhase.TeamSelect:
                ChangeTeam(playerNumber, direction);
                break;
            case SelectionPhase.Ready:
                break;
        }
    }

    public void RouteInputA(int playerNumber)
    {
        var phase = playerPhase[playerNumber - 1];
        switch (phase)
        {
            case SelectionPhase.CharacterSelect:
                SelectCharacter(playerNumber);
                break;
            case SelectionPhase.TeamSelect:
                SelectTeam(playerNumber);
                break;
            case SelectionPhase.Ready:
                break;
        }
    }
    public void RouteInputB(int playerNumber)
    {
        var phase = playerPhase[playerNumber - 1];
        switch (phase)
        {
            case SelectionPhase.CharacterSelect:
                break;
            case SelectionPhase.TeamSelect:
                ClearSelection(playerNumber);
                break;
            case SelectionPhase.Ready:
                UndoReady(playerNumber);
                break;
        }
    }

    public void RouteInputBack()
    {
        SceneManager.LoadScene("MainMenu");
    }
    #endregion

    #region Selection Functions
    private void MoveSelectionToken(int playerNumber, int direction)
    {

        switch (playerNumber)
        {
            case 1:
                p1ActiveToken = SwitchTokens(p1ActiveToken, p1Tokens, direction);
                break;
            case 2:
                p2ActiveToken = SwitchTokens(p2ActiveToken, p2Tokens, direction);
                break;
            case 3:
                p3ActiveToken = SwitchTokens(p3ActiveToken, p3Tokens, direction);
                break;
            case 4:
                p4ActiveToken = SwitchTokens(p4ActiveToken, p4Tokens, direction);
                break;
        }
    }

    private void SelectCharacter(int playerNumber)
    {
        var color = Color.white;
        switch (playerNumber)
        {
            case 1:
                foreach (var token in p1Tokens)
                {
                    if (token.activeInHierarchy)
                    {
                        var selected = token.GetComponentInParent<CharacterInfo>();
                        GameData.playerCharacters[0] = selected.getCharacterId();
                        p1TeamImage.sprite = selected.getCharacterImage();
                        color.a = 1;
                        p1TeamImage.color = color;
                        token.GetComponent<Shadow>().enabled = false;
                        token.GetComponent<ParticleSystem>().Play();
                        playerPhase[0] = SelectionPhase.TeamSelect;
                    }
                }
                break;
            case 2:
                foreach (var token in p2Tokens)
                {
                    if (token.activeInHierarchy)
                    {
                        var selected = token.GetComponentInParent<CharacterInfo>();
                        GameData.playerCharacters[1] = selected.getCharacterId();
                        p2TeamImage.sprite = selected.getCharacterImage();
                        color.a = 1;
                        p2TeamImage.color = color;
                        token.GetComponent<Shadow>().enabled = false;
                        token.GetComponent<ParticleSystem>().Play();
                        playerPhase[1] = SelectionPhase.TeamSelect;
                    }
                }
                break;
            case 3:
                foreach (var token in p3Tokens)
                {
                    if (token.activeInHierarchy)
                    {
                        var selected = token.GetComponentInParent<CharacterInfo>();
                        GameData.playerCharacters[2] = selected.getCharacterId();
                        p3TeamImage.sprite = selected.getCharacterImage();
                        color.a = 1;
                        p3TeamImage.color = color;
                        token.GetComponent<Shadow>().enabled = false;
                        token.GetComponent<ParticleSystem>().Play();
                        playerPhase[2] = SelectionPhase.TeamSelect;
                    }
                }
                break;
            case 4:
                foreach (var token in p4Tokens)
                {
                    if (token.activeInHierarchy)
                    {
                        var selected = token.GetComponentInParent<CharacterInfo>();
                        GameData.playerCharacters[3] = selected.getCharacterId();
                        p4TeamImage.sprite = selected.getCharacterImage();
                        color.a = 1;
                        p3TeamImage.color = color;
                        token.GetComponent<Shadow>().enabled = false;
                        token.GetComponent<ParticleSystem>().Play();
                        playerPhase[3] = SelectionPhase.TeamSelect;
                    }
                }
                break;
        }
    }

    private void ChangeTeam(int playerNumber, int direction)
    {
        switch (playerNumber)
        {
            case 1:
                p1Team = GetNextTeam(p1Team);
                p1TeamPanel.color = GetTeamColor(p1Team);
                break;
            case 2:
                p2Team = GetNextTeam(p2Team);
                p2TeamPanel.color = GetTeamColor(p2Team);
                break;
            case 3:
                p3Team = GetNextTeam(p3Team);
                p3TeamPanel.color = GetTeamColor(p3Team);
                break;
            case 4:
                p4Team = GetNextTeam(p4Team);
                p4TeamPanel.color = GetTeamColor(p4Team);
                break;
        }
    }

    private void SelectTeam(int playerNumber)
    {
        switch (playerNumber)
        {
            case 1:
                playerPhase[0] = SelectionPhase.Ready;
                readyTags[0].SetActive(true);
                GameData.playerTeams[0] = p1Team;
                break;
            case 2:
                playerPhase[1] = SelectionPhase.Ready;
                readyTags[1].SetActive(true);
                GameData.playerTeams[1] = p2Team;
                break;
            case 3:
                playerPhase[2] = SelectionPhase.Ready;
                readyTags[2].SetActive(true);
                GameData.playerTeams[2] = p3Team;
                break;
            case 4:
                playerPhase[3] = SelectionPhase.Ready;
                readyTags[3].SetActive(true);
                GameData.playerTeams[3] = p4Team;
                break;
        }
    }
    #endregion

    #region Private Helpers
    private void SetActiveTokens()
    {
        p1ActiveToken = p1DefaultToken;
        p2ActiveToken = p2DefaultToken;
        p3ActiveToken = p3DefaultToken;
        p4ActiveToken = p4DefaultToken;
    }

    private void SetDefaultTeams()
    {
        p1Team = Enumerables.ETeam.BlueTeam;
        p2Team = Enumerables.ETeam.BlueTeam;
        p3Team = Enumerables.ETeam.RedTeam;
        p4Team = Enumerables.ETeam.RedTeam;

        p1TeamPanel.color = Color.blue;
        p2TeamPanel.color = Color.blue;
        p3TeamPanel.color = Color.red;
        p3TeamPanel.color = Color.red;
    }

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
        if (readyCount < 2)
        {
            allReady = false;
        }
		GameData.playerCount = readyCount;
        return allReady;
    }

    private IEnumerator LevelSelectTimer()
    {
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadSceneAsync(LevelIndex.LEVEL_SELECT);
    }

    private void CountdownToLevelSelect()
    {
        timer -= Time.deltaTime;
        int seconds = (int)(timer % 60);
        if (seconds >= 0)
        {
            timerText.text = "Level Select in: " + seconds;
        }
    }

    private Enumerables.ETeam GetNextTeam(Enumerables.ETeam team)
    {
        switch (team)
        {
            case Enumerables.ETeam.BlueTeam:
                return Enumerables.ETeam.RedTeam;
            case Enumerables.ETeam.RedTeam:
                return Enumerables.ETeam.BlueTeam;
            case Enumerables.ETeam.None:
                return Enumerables.ETeam.BlueTeam;
            default:
                return Enumerables.ETeam.BlueTeam;
        }
    }

    private Color GetTeamColor(Enumerables.ETeam team)
    {
        switch (team)
        {
            case Enumerables.ETeam.BlueTeam:
                return Color.blue;
            case Enumerables.ETeam.RedTeam:
                return Color.red;
            case Enumerables.ETeam.None:
                return Color.grey;
            default:
                return Color.gray;
        }
    }

    private GameObject SwitchTokens(GameObject activeToken, List<GameObject> tokens, int direction)
    {
        activeToken.SetActive(false);
        var currentIndex = tokens.FindIndex(t => (t == activeToken));
        if (currentIndex == 0)
        {
            if (direction > 0)
            {
                activeToken = tokens[currentIndex + direction];
            }
            else
            {
                activeToken = tokens[tokens.Count - 1];
            }
        }
        else if (currentIndex == tokens.Count - 1)
        {
            if (direction < 0)
            {
                activeToken = tokens[currentIndex + direction];
            }
            else
            {
                activeToken = tokens[0];
            }
        }
        else
        {
            activeToken = activeToken = tokens[currentIndex + direction];
        }
        activeToken.SetActive(true);
        return activeToken;
    }

    /// <summary>
    /// Reset player's character selection to none. Re-enable character selecting for player.
    /// </summary>
    /// <param name="playerNumber"></param>
    private void ClearSelection(int playerNumber)
    {
        var color = Color.white;
        switch (playerNumber)
        {
            case 1:
                GameData.playerCharacters[0] = Enumerables.ECharacter.None;
                p1TeamImage.sprite = null;
                color = p1TeamImage.color;
                color.a = 0;
                p1TeamImage.color = color;
                p1ActiveToken.GetComponent<Shadow>().enabled = true;
                p1ActiveToken.GetComponent<ParticleSystem>().Stop();
                playerPhase[0] = SelectionPhase.CharacterSelect;
                break;
            case 2:
                GameData.playerCharacters[1] = Enumerables.ECharacter.None;
                p2TeamImage.sprite = null;
                color = p2TeamImage.color;
                color.a = 0;
                p2TeamImage.color = color;
                p2ActiveToken.GetComponent<Shadow>().enabled = true;
                p2ActiveToken.GetComponent<ParticleSystem>().Stop();
                playerPhase[1] = SelectionPhase.CharacterSelect;
                break;
            case 3:
                GameData.playerCharacters[2] = Enumerables.ECharacter.None;
                p3TeamImage.sprite = null;
                color = p3TeamImage.color;
                color.a = 0;
                p3TeamImage.color = color;
                p3ActiveToken.GetComponent<Shadow>().enabled = true;
                p3ActiveToken.GetComponent<ParticleSystem>().Stop();
                playerPhase[2] = SelectionPhase.CharacterSelect;
                break;
            case 4:
                GameData.playerCharacters[3] = Enumerables.ECharacter.None;
                p4TeamImage.sprite = null;
                color = p4TeamImage.color;
                color.a = 0;
                p4TeamImage.color = color;
                p4ActiveToken.GetComponent<Shadow>().enabled = true;
                p4ActiveToken.GetComponent<ParticleSystem>().Stop();
                playerPhase[3] = SelectionPhase.CharacterSelect;
                break;
        }
    }

    private void UndoReady(int playerNumber)
    {
        StopAllCoroutines();
        timer = waitTime;
        timerText.text = "Waiting...";
        switch (playerNumber)
        {
            case 1:
                playerPhase[0] = SelectionPhase.TeamSelect;
                readyTags[0].SetActive(false);
                break;
            case 2:
                playerPhase[1] = SelectionPhase.TeamSelect;
                readyTags[1].SetActive(false);
                break;
            case 3:
                playerPhase[2] = SelectionPhase.TeamSelect;
                readyTags[2].SetActive(false);
                break;
            case 4:
                playerPhase[3] = SelectionPhase.TeamSelect;
                readyTags[3].SetActive(false);
                break;
        }
    }
    #endregion
}
