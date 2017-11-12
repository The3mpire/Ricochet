using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class _UICharacterSelectModel : MonoBehaviour {
    public class PlayerData
	{
        public bool active = false;
		public int team = 1;
		public string character = "bean";
		public int xPos = 0;
		public int yPos = 0;
		public bool ready = false;
	}

    #region Inspector Variables
	[Header("Default settings")]
	[Tooltip("Default number of teams")]
	[SerializeField]
	private int numberOfTeams = 2;

	[Header("Player UI Objects")]
	[Tooltip("Player 1")]
	[SerializeField]
	private _UICharacterSelectView player1;
	[Tooltip("Player 2")]
	[SerializeField]
	private _UICharacterSelectView player2;
	[Tooltip("Player 3")]
	[SerializeField]
	private _UICharacterSelectView player3;
	[Tooltip("Player 4")]
	[SerializeField]
	private _UICharacterSelectView player4;
    #endregion

    #region Private Variables
	private Dictionary<int, PlayerData> players;
	private Dictionary<int, _UICharacterSelectView> spriteHandlers;
	private string[,] characters = { { "bean", "eggplant" } };
    private int numberOfPlayers = 4;

    #endregion

    #region MonoBehaviour
    void Awake()
    {
        players = new Dictionary<int, PlayerData>();
        for (int i = 1; i <= numberOfPlayers; i++)
        {
            players.Add(i, new PlayerData());
        }
        spriteHandlers = new Dictionary<int, _UICharacterSelectView>()
        {
            {1, player1},
            {2, player2},
            {3, player3},
            {4, player4}
        };
    }
    #endregion

    public bool ValidateSetup()
    {
        bool team1 = false;
        bool team2 = false;
        foreach (PlayerData player in players.Values)
        {
            if (player.active)
            {
                if (!player.ready)
                {
                    return false;
                }
                else if (player.team == 1)
                {
                    team1 = true;
                }
                else if (player.team == 2)
                {
                    team2 = true;
                }
            }
        }
        return team1 && team2;
    }

    /// <summary>
    /// Move the characters selection in a direction
    /// </summary>
    /// <param name="playerID">player number from 1-4</param>
    /// <param name="direction">1: up, 2: right, 3: down, 4: left</param>
	public void MovePlayerSelection(int playerID, int direction)
    {
		if (CheckPlayer (playerID) == true) {
			Debug.Log ("cant change");
			return;
		}
		PlayerData player = players [playerID];
		_UICharacterSelectView playerView = spriteHandlers [playerID];

		switch (direction) {
		// check this if you make another row in the array
		case 1:
			player.yPos += 1;
			player.yPos = player.yPos >= characters.GetLength(0) ? player.yPos - 1 : player.yPos;
			break;
		case 2:
			player.xPos += 1;
			player.xPos = player.xPos >= characters.Length ? player.xPos - 1 : player.xPos;
			break;
		case 3:
			player.yPos -= 1;
			player.yPos = player.yPos  < 0 ? player.yPos + 1 : player.yPos;
			break;
		case 4:
			player.xPos -= 1;
			player.xPos = player.xPos < 0 ?  player.xPos + 1 : player.xPos;
			break;
		default:
			Debug.LogError ("Selection change direction not within expected bounds: " + direction, gameObject);
			return;
			break;
		}

		playerView.MoveCursor (characters [player.yPos, player.xPos]);
		playerView.UpdateCharacter (characters [player.yPos, player.xPos]);
    }

	/// <summary>
	/// Returns the status of the player(ready or not ready) and sets them as active.
	/// </summary>
	/// <returns><c>true</c>, if player was checked, <c>false</c> otherwise.</returns>
	/// <param name="playerID">Player I.</param>
	private bool CheckPlayer(int playerID)
	{
		players [playerID].active = true;
		return players [playerID].ready;
	}

	public void ChangeTeam(int playerID)
	{ 
		if (CheckPlayer (playerID) == true) {
			return;
		}
        PlayerData player = players[playerID];
		player.team++;
		if (player.team > numberOfTeams) {
			player.team = 1;
		}
		// update team color
		spriteHandlers[playerID].UpdateTeam(player.team);
	}

	public void ToggleReady(int playerID)
    {
        PlayerData player = players[playerID];
		player.ready = !player.ready;
        // update ready
        spriteHandlers[playerID].SetReady(player.ready);
		players [playerID].active = true;

	}

	public Dictionary<int, PlayerData> GetPlayerData() 
	{
		return players;
	}
}

