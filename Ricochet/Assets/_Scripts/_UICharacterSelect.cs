using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Inspector MonoBehaviour

public class _UICharacterSelect : MonoBehaviour {

    #region Inspector Variables
	[Header("Default settings")]
	[Tooltip("Default number of players")]
	[SerializeField]
	private int numberOfPlayers = 2;

	[Tooltip("Default number of teams")]
	[SerializeField]
	private int numberOfTeams = 2;

	[Header("Player UI Objects")]
	[Tooltip("Player 1")]
	[SerializeField]
	private _UICharacterDisplay player1;
	[Tooltip("Player 2")]
	[SerializeField]
	private _UICharacterDisplay player2;
	[Tooltip("Player 3")]
	[SerializeField]
	private _UICharacterDisplay player3;
	[Tooltip("Player 4")]
	[SerializeField]
	private _UICharacterDisplay player4;
	#endregion

	private class Player
	{
		public int team = 1;
		public string character = "bean";
		public int xPos = 0;
		public int yPos = 0;
		public bool ready = false;
	}

	private Dictionary<int,Player> players;
	private Dictionary<int, _UICharacterDisplay> spriteHandlers;
	private string[,] characters = { { "bean", "eggplant" } };

	void Start () {
		if (numberOfPlayers < 2) 
		{
			Debug.LogError ("You fucked up. Too few players",gameObject);
		}
		players = new Dictionary<int, Player> ();
		for (int i = 1; i <= numberOfPlayers; i++) 
		{
			players.Add (i, new Player ());
		}
		spriteHandlers = new Dictionary<int, _UICharacterDisplay>() 
		{
			{1, player1},
			{2, player2},
			{3, player3},
			{4, player4}
		};
	}

	public bool ValidateSetup()
	{
		return true;
	}

	void Update()
	{
		if (Input.GetKeyDown (KeyCode.A) ||
		    Input.GetKeyDown (KeyCode.D)) {
			ChangePlayerLoc (1, players [1]);
		} else if (Input.GetKeyDown (KeyCode.LeftArrow) ||
		           Input.GetKeyDown (KeyCode.RightArrow)) {
			ChangePlayerLoc(2, players[2]);
		} else if (Input.GetKeyDown (KeyCode.Space)) {
			ToggleReady (1, players [1]);
		} else if (Input.GetKeyDown (KeyCode.Return)) {
			ToggleReady(2, players[2]);
		} else if (Input.GetKeyDown (KeyCode.E)) {
			ChangeTeam (1, players [1]);
		} else if (Input.GetKeyDown (KeyCode.RightControl)) {
			ChangeTeam (2, players [2]);
		}
	}

	private void ChangePlayerLoc(int playerID, Player player)
	{
		if (Input.GetKeyDown (KeyCode.A) || Input.GetKeyDown (KeyCode.LeftArrow)) {
			player.xPos--;
			if (player.xPos < 0) {
				player.xPos = 0;
			}
			player.character = characters [player.xPos, player.yPos];
		} else if (Input.GetKeyDown (KeyCode.D) || Input.GetKeyDown (KeyCode.RightArrow)) {
			player.xPos++;
			if (player.xPos >= players.Count) {
				player.xPos = players.Count - 1;
			}
			player.character = characters [player.xPos, player.yPos];
		}
		spriteHandlers[playerID].UpdateCharacter(player.character);
	}

	private void ChangeTeam(int playerID, Player player) {
		player.team++;
		if (player.team > numberOfTeams) {
			player.team = 1;
		}
		// update team color
		spriteHandlers[playerID].UpdateTeam(player.team);
	}

	private void ToggleReady(int playerID, Player player) {
		player.ready = !player.ready;
		// update ready
		spriteHandlers[playerID].SetReady(player.ready);
	}

}

#endregion
