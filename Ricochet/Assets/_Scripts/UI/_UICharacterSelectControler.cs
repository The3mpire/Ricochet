using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using System;
using PlayerSelectData;

public class _UICharacterSelectControler : MonoBehaviour
{
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

        players = new Player[4] { playerInput1, playerInput2, playerInput3, playerInput4 };
        
        joystickAcceptingInput = new Dictionary<int, bool>() {
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
            Player input = players[i];
            double x = Math.Abs(input.GetAxis("MoveHorizontal"));
            double y = Math.Abs(input.GetAxis("MoveVertical"));
            if (x > stickDeadZone || y > stickDeadZone)
            {
                if (joystickAcceptingInput[i + 1])
                {
                    joystickAcceptingInput[i + 1] = false;
                    StartCoroutine(ReactivateAfterDelay(i + 1));
                    HandOffInput(i + 1, input);
                }
            }

            if (input.GetButtonDown("Jump"))
            {
                model.ChangeTeam(i + 1);
            }
            if (input.GetButtonDown("A Button"))
            {
                model.ToggleReady(i + 1, true);
            }
            if (input.GetButtonDown("B Button"))
            {
                model.ToggleReady(i + 1, false);
            }
            if(input.GetButtonDown("Start"))
            {
                model.StartGame();
            }
        }
    }
    #endregion

    private void HandOffInput(int playerID, Player player)
    {
        double x = player.GetAxis("MoveHorizontal");
        double y = player.GetAxis("MoveVertical");

        double absX = Math.Abs(x);
        double absY = Math.Abs(y);

        if (absX > absY)
        {
            if (x < 0)
            {
                model.MovePlayerSelection(playerID, 4);
            }
            else
            {
                model.MovePlayerSelection(playerID, 2);
            }
        }
        else
        {
            if (y < 0)
            {
                model.MovePlayerSelection(playerID, 3);
            }
            else
            {
                model.MovePlayerSelection(playerID, 1);
            }
        }
    }

    private IEnumerator ReactivateAfterDelay(int playerID)
    {
        yield return new WaitForSeconds(inputDelay);
        joystickAcceptingInput[playerID] = true;
    }
}
