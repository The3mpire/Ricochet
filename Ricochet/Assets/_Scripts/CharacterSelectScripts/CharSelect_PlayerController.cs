using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class CharSelect_PlayerController : MonoBehaviour
{
    #region Inspector Variables

    [Tooltip("Set the player number")]
    [SerializeField]
    private int playerNumber;
    [Tooltip("Drag CharSelectManager here")]
    [SerializeField]
    private GameObject managerPanel;
    [Tooltip("Color of player icon")]
    [SerializeField]
    private Color playerColor;

    [Header("Input settings")]
    [Tooltip("Delay before a joystick inputs")]
    [SerializeField]
    private float inputDelay = 1.25f;

    #endregion

    #region Private Variables

    private Player player;
    private CharSelectManager manager;
    private bool joystickAcceptingInput = true;

    #endregion

    #region MonoBehaviour
    // Use this for initialization
    void Awake()
    {
        player = ReInput.players.GetPlayer(playerNumber - 1);
        manager = managerPanel.GetComponent<CharSelectManager>();
    }

    // Update is called once per frame
    void Update()
    {
        var moveX = Math.Sign(player.GetAxis("UIHorizontal"));
        if (moveX != 0 && joystickAcceptingInput)
        {
            joystickAcceptingInput = false;
            manager.RouteInputAxis(playerNumber - 1, moveX);
            StartCoroutine(ReactivateAfterDelay());
        }

        if (player.GetButtonDown("UISubmit"))
        {
            manager.RouteInputA(playerNumber - 1, playerColor);
        }
        if (player.GetButtonDown("UICancel"))
        {
            manager.RouteInputB(playerNumber - 1);
        }
        if (player.GetButton("UICancel"))
        {
            manager.RouteInputAltB(playerNumber - 1);
        }
    }
    #endregion

    #region Private Helpers
    private IEnumerator ReactivateAfterDelay()
    {
        yield return new WaitForSeconds(inputDelay);
        joystickAcceptingInput = true;
    }
    #endregion

    #region Getters and Setters

    public Color GetPlayerColor()
    {
        return playerColor;
    }

    #endregion

}
