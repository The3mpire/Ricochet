using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using System;

public class _UICharacterSelectControler : MonoBehaviour {
    #region Private Variables
    Player playerInput1;
    Player playerInput2;
    Player playerInput3;
    Player playerInput4;
	private Player[] players;
	private Dictionary<int, bool> joystickAcceptingInput;
    #endregion

	#region Inspector Variables
	[Header("UI Character Select Model")]
	[SerializeField]
	private _UICharacterSelectModel model;
	[Header("Input settings")]
	[Tooltip("Delay before a joystick inputs")]
	[SerializeField]
	private float inputDelay = 1.25f;
	[Tooltip("How sensitive the left stick is before acknowledging input")]
	[SerializeField]
	private double stickDeadZone = 0.75;
	#endregion

    #region MonoBehaviour
    void Awake()
    {
        playerInput1 = ReInput.players.GetPlayer(0);
        playerInput2 = ReInput.players.GetPlayer(1);
        playerInput3 = ReInput.players.GetPlayer(2);
        playerInput4 = ReInput.players.GetPlayer(3);

		players = new Player[4] {playerInput1, playerInput2, playerInput3, playerInput4};

		joystickAcceptingInput = new Dictionary<int, bool> () {
			{ 1, true },
			{ 2, true },
			{ 3, true },
			{ 4, true }
		};
    }

    void Update()
    {
		for (int i = 0; i < players.Length; i++)
		{
			Player input = players [i];
			double x = Math.Abs(input.GetAxis ("MoveHorizontal"));
			double y = Math.Abs (input.GetAxis ("MoveVertical"));	
			if (x > stickDeadZone || y > stickDeadZone)
			{
				if (joystickAcceptingInput [i + 1]) 
				{
					//Debug.Log ("WE HERE BOI");
					joystickAcceptingInput [i + 1] = false;
					StartCoroutine(ReactivateAfterDelay (i + 1));
					HandOffInput (i + 1, input);
				}
			}

			if (input.GetButtonDown ("Jump")) {
				model.ChangeTeam (i + 1);
			}
		}
    }
    #endregion

	private void HandOffInput(int playerID, Player player)
	{
		Debug.Log ("PLAYER: " + playerID);
		double absX = Math.Abs (player.GetAxis ("MoveHorizontal"));
		double absY = Math.Abs (player.GetAxis ("MoveVertical"));

		double x = player.GetAxis ("MoveHorizontal");
		double y = player.GetAxis ("MoveVertical");

		if (absX > absY) {
			if (x < 0) {
				//Debug.Log ("left");
				model.MovePlayerSelection(playerID, 4);
			} else {
				//Debug.Log ("right");
				model.MovePlayerSelection(playerID, 2);
			}
		} else {
			if (y < 0) {
				//Debug.Log ("down");
				model.MovePlayerSelection(playerID, 3);
			} else {
				//Debug.Log ("up");
				model.MovePlayerSelection(playerID, 1);
			}
		}
	}

	private IEnumerator ReactivateAfterDelay (int playerID)
	{
		//Debug.Log ("waiting...");
		yield return new WaitForSeconds (inputDelay);
		joystickAcceptingInput [playerID] = true;
		//Debug.Log ("REACTIVATED: " + playerID);
	}
}
