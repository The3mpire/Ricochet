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
    [Tooltip("Amount of time to wait in seconds after all players are ready")]
    private float waitTime;

    [Header("Player 1")]
    [Tooltip("Press A to Join Icon")]
    [SerializeField]
    private GameObject p1JoinIcon;
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
    [Tooltip("Press A to Join Icon")]
    [SerializeField]
    private GameObject p2JoinIcon;
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
    [Tooltip("Press A to Join Icon")]
    [SerializeField]
    private GameObject p3JoinIcon;
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
    [Tooltip("Press A to Join Icon")]
    [SerializeField]
    private GameObject p4JoinIcon;
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

    [Header("Misc UI Objects")]
    [SerializeField]
    [Tooltip("Drag all ready text objects for each player in order")]
    private GameObject[] readyTags;
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

    private SelectionPhase[] playerPhase = new SelectionPhase[4];
    private float timer;

    private bool timerActive;

    private bool bHeld;
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
        var playerData = new PSettings();
        switch (playerNumber)
        {
            case 0:
                p1DefaultToken.SetActive(true);
                p1ActiveToken = p1DefaultToken;
                playerPhase[0] = SelectionPhase.None;
                playerData = LoadPlayerSettings(playerNumber);
                if (playerData.Character == ECharacter.None)
                {
                    playerData.Character = p1DefaultToken.GetComponentInParent<CharacterInfo>().getCharacterId();
                }
                p1ActiveToken = MoveSelectionTokenTo(playerData.Character, p1ActiveToken, p1Tokens);
                p1Team = playerData.Team;
                p1TeamPanel.color = GetTeamColor(p1Team);
                break;
            case 1:
                p2DefaultToken.SetActive(true);
                p2ActiveToken = p2DefaultToken;
                playerPhase[1] = SelectionPhase.None;
                playerData = LoadPlayerSettings(playerNumber);
                if (playerData.Character == ECharacter.None)
                {
                    playerData.Character = p2DefaultToken.GetComponentInParent<CharacterInfo>().getCharacterId();
                }
                p2ActiveToken = MoveSelectionTokenTo(playerData.Character, p2ActiveToken, p2Tokens);
                p2Team = playerData.Team;
                p2TeamPanel.color = GetTeamColor(p2Team);
                break;
            case 2:
                p3DefaultToken.SetActive(true);
                p3ActiveToken = p3DefaultToken;
                playerPhase[2] = SelectionPhase.None;
                playerData = LoadPlayerSettings(playerNumber);
                if (playerData.Character == ECharacter.None)
                {
                    playerData.Character = p3DefaultToken.GetComponentInParent<CharacterInfo>().getCharacterId();
                }
                p3ActiveToken = MoveSelectionTokenTo(playerData.Character, p3ActiveToken, p3Tokens);
                p3Team = playerData.Team;
                p3TeamPanel.color = GetTeamColor(p3Team);
                break;
            case 3:
                p4DefaultToken.SetActive(true);
                p4ActiveToken = p4DefaultToken;
                playerPhase[3] = SelectionPhase.None;
                playerData = LoadPlayerSettings(playerNumber);
                if (playerData.Character == ECharacter.None)
                {
                    playerData.Character = p4DefaultToken.GetComponentInParent<CharacterInfo>().getCharacterId();
                }
                p4ActiveToken = MoveSelectionTokenTo(playerData.Character, p4ActiveToken, p4Tokens);
                p4Team = playerData.Team;
                p4TeamPanel.color = GetTeamColor(p4Team);
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

    public void RouteInputA(int playerNumber, Color playerColor)
    {
        var phase = playerPhase[playerNumber - 1];
        switch (phase)
        {
            case SelectionPhase.None:
                PlayerJoin(playerNumber);
                break;
            case SelectionPhase.CharacterSelect:
                SelectCharacter(playerNumber, playerColor);
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
                ClearImages(playerNumber);
                ClearSelection(playerNumber);
                break;
            case SelectionPhase.Ready:
                UndoReady(playerNumber);
                break;
        }
    }
    public void RouteInputAltB(int playerNumber)
    {
        if (playerPhase[playerNumber - 1] == SelectionPhase.CharacterSelect)
        {
            bHeld = true;
        }
    }

    public void RouteInputBack()
    {
        LevelSelect.LoadMainMenu();
    }
    #endregion

    #region Selection Functions
    private void MoveSelectionToken(int playerNumber, int direction)
    {
        switch (playerNumber)
        {
            case 1:
                p1ActiveToken = SwitchTokensToNext(p1ActiveToken, p1Tokens, direction);
                break;
            case 2:
                p2ActiveToken = SwitchTokensToNext(p2ActiveToken, p2Tokens, direction);
                break;
            case 3:
                p3ActiveToken = SwitchTokensToNext(p3ActiveToken, p3Tokens, direction);
                break;
            case 4:
                p4ActiveToken = SwitchTokensToNext(p4ActiveToken, p4Tokens, direction);
                break;
        }
    }

    private void SelectCharacter(int playerNumber, Color playerColor)
    {
        var color = Color.white;
        var selectedChar = Enumerables.ECharacter.None;
        switch (playerNumber)
        {
            case 1:
                foreach (var token in p1Tokens)
                {
                    if (token.activeInHierarchy)
                    {
                        var selected = token.GetComponentInParent<CharacterInfo>();
                        if (selected.GetIsSelectable())
                        {
                            selected.SetIsSelectable(false);
                            selectedChar = selected.getCharacterId();
                            gameData.SetPlayerCharacter(0, selected.getCharacterId());
                            p1TeamImage.sprite = selected.getCharacterImage();
                            color.a = 1;
                            p1TeamImage.color = color;
                            token.GetComponent<Shadow>().enabled = false;
                            token.GetComponent<ParticleSystem>().Play();
                            playerPhase[0] = SelectionPhase.TeamSelect;
                        }
                    }
                }
                break;
            case 2:
                foreach (var token in p2Tokens)
                {
                    if (token.activeInHierarchy)
                    {
                        var selected = token.GetComponentInParent<CharacterInfo>();
                        if (selected.GetIsSelectable())
                        {
                            selected.SetIsSelectable(false);
                            selectedChar = selected.getCharacterId();
                            gameData.SetPlayerCharacter(1, selected.getCharacterId());
                            p2TeamImage.sprite = selected.getCharacterImage();
                            color.a = 1;
                            p2TeamImage.color = color;
                            token.GetComponent<Shadow>().enabled = false;
                            token.GetComponent<ParticleSystem>().Play();
                            playerPhase[1] = SelectionPhase.TeamSelect;
                        }
                    }
                }
                break;
            case 3:
                foreach (var token in p3Tokens)
                {
                    if (token.activeInHierarchy)
                    {
                        var selected = token.GetComponentInParent<CharacterInfo>();
                        if (selected.GetIsSelectable())
                        {
                            selected.SetIsSelectable(false);
                            selectedChar = selected.getCharacterId();
                            gameData.SetPlayerCharacter(2, selected.getCharacterId());
                            p3TeamImage.sprite = selected.getCharacterImage();
                            color.a = 1;
                            p3TeamImage.color = color;
                            token.GetComponent<Shadow>().enabled = false;
                            token.GetComponent<ParticleSystem>().Play();
                            playerPhase[2] = SelectionPhase.TeamSelect;
                        }
                    }
                }
                break;
            case 4:
                foreach (var token in p4Tokens)
                {
                    if (token.activeInHierarchy)
                    {
                        var selected = token.GetComponentInParent<CharacterInfo>();
                        if (selected.GetIsSelectable())
                        {
                            selected.SetIsSelectable(false);
                            selectedChar = selected.getCharacterId();
                            gameData.SetPlayerCharacter(3, selected.getCharacterId());
                            p4TeamImage.sprite = selected.getCharacterImage();
                            color.a = 1;
                            p4TeamImage.color = color;
                            token.GetComponent<Shadow>().enabled = false;
                            token.GetComponent<ParticleSystem>().Play();
                            playerPhase[3] = SelectionPhase.TeamSelect;
                        }
                    }
                }
                break;
        }
        
        switch (selectedChar)
        {
            case Enumerables.ECharacter.Cat:
                char1Image.color = playerColor;
                break;
            case Enumerables.ECharacter.Computer:
                char2Image.color = playerColor;
                break;
            case Enumerables.ECharacter.MallCop:
                char3Image.color = playerColor;
                break;
            case Enumerables.ECharacter.Sushi:
                char4Image.color = playerColor;
                break;
            default:
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
		// lock in a team
        switch (playerNumber)
        {
            case 1:
                playerPhase[0] = SelectionPhase.Ready;
                readyTags[0].SetActive(true);
                gameData.SetPlayerTeam(0, p1Team);
                break;
            case 2:
                playerPhase[1] = SelectionPhase.Ready;
                readyTags[1].SetActive(true);
                gameData.SetPlayerTeam(1, p2Team);
                break;
            case 3:
                playerPhase[2] = SelectionPhase.Ready;
                readyTags[2].SetActive(true);
                gameData.SetPlayerTeam(2, p3Team);
                break;
            case 4:
                playerPhase[3] = SelectionPhase.Ready;
                readyTags[3].SetActive(true);
                gameData.SetPlayerTeam(3, p4Team);
                break;
        }
    }

    private void PlayerJoin(int playerNumber)
    {
        // lock in a team
        switch (playerNumber)
        {
            case 1:
                playerPhase[0] = SelectionPhase.CharacterSelect;
                p1JoinIcon.SetActive(false);
                break;
            case 2:
                playerPhase[1] = SelectionPhase.CharacterSelect;
                p2JoinIcon.SetActive(false);
                break;
            case 3:
                playerPhase[2] = SelectionPhase.CharacterSelect;
                p3JoinIcon.SetActive(false);
                break;
            case 4:
                playerPhase[3] = SelectionPhase.CharacterSelect;
                p4JoinIcon.SetActive(false);
                break;
        }
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

        gameData.SetPlayerCount(readyCount);
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
                return Color.grey;
            default:
                return Color.grey;
        }
    }

    private PSettings LoadPlayerSettings(int playerNumber)
    {
        var character = ECharacter.None;
        var team = ETeam.None;

        character = gameData.GetPlayerCharacter(playerNumber);
        team = gameData.GetPlayerTeam(playerNumber);
        
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

    private GameObject SwitchTokensToNext(GameObject activeToken, List<GameObject> tokens, int direction)
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
            activeToken = tokens[currentIndex + direction];
        }
        activeToken.SetActive(true);
        return activeToken;
    }

    private void ClearImages(int playerNumber)
    {
        var selectedChar = Enumerables.ECharacter.None;
        switch (playerNumber)
        {
            case 1:
                var selectedImage1 = p1ActiveToken.GetComponentInParent<CharacterInfo>();
                selectedImage1.SetIsSelectable(true);
                selectedChar = selectedImage1.getCharacterId();
                break;
            case 2:
                var selectedImage2 = p2ActiveToken.GetComponentInParent<CharacterInfo>();
                selectedImage2.SetIsSelectable(true);
                selectedChar = selectedImage2.getCharacterId();
                break;
            case 3:
                var selectedImage3 = p3ActiveToken.GetComponentInParent<CharacterInfo>();
                selectedImage3.SetIsSelectable(true);
                selectedChar = selectedImage3.getCharacterId();
                break;
            case 4:
                var selectedImage4 = p4ActiveToken.GetComponentInParent<CharacterInfo>();
                selectedImage4.SetIsSelectable(true);
                selectedChar = selectedImage4.getCharacterId();
                break;
        }

        switch (selectedChar)
        {
            case Enumerables.ECharacter.Cat:
                char1Image.color = defaultColor;
                break;
            case Enumerables.ECharacter.Computer:
                char2Image.color = defaultColor;
                break;
            case Enumerables.ECharacter.MallCop:
                char3Image.color = defaultColor;
                break;
            case Enumerables.ECharacter.Sushi:
                char4Image.color = defaultColor;
                break;
            default:
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
        switch (playerNumber)
        {
            case 1:
                gameData.SetPlayerCharacter(0, ECharacter.None);
                p1TeamImage.sprite = null;
                color = p1TeamImage.color;
                color.a = 0;
                p1TeamImage.color = color;
                p1ActiveToken.GetComponent<Shadow>().enabled = true;
                p1ActiveToken.GetComponent<ParticleSystem>().Stop();
                playerPhase[0] = SelectionPhase.CharacterSelect;
                break;
            case 2:
                gameData.SetPlayerCharacter(1, ECharacter.None);
                p2TeamImage.sprite = null;
                color = p2TeamImage.color;
                color.a = 0;
                p2TeamImage.color = color;
                p2ActiveToken.GetComponent<Shadow>().enabled = true;
                p2ActiveToken.GetComponent<ParticleSystem>().Stop();
                playerPhase[1] = SelectionPhase.CharacterSelect;
                break;
            case 3:
                gameData.SetPlayerCharacter(2, ECharacter.None);
                p3TeamImage.sprite = null;
                color = p3TeamImage.color;
                color.a = 0;
                p3TeamImage.color = color;
                p3ActiveToken.GetComponent<Shadow>().enabled = true;
                p3ActiveToken.GetComponent<ParticleSystem>().Stop();
                playerPhase[2] = SelectionPhase.CharacterSelect;
                break;
            case 4:
                gameData.SetPlayerCharacter(3, ECharacter.None);
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
        timerActive = false;
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
