using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class CharSelect_PlayerController : MonoBehaviour
{
    #region Private
    private Player player;

    [SerializeField]
    [Tooltip("Set the player number")]
    private int playerNumber;

    [SerializeField]
    [Tooltip("Drag CharSelectManager here")]
    private GameObject managerPanel;
    private CharSelectManager manager;
    private bool joystickAcceptingInput = true;
    [Header("Input settings")]
    [Tooltip("Delay before a joystick inputs")]
    [SerializeField]
    private float inputDelay = 1.25f;

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
            manager.routeInputAxis(playerNumber, moveX);
            StartCoroutine(ReactivateAfterDelay());
        }

        if (player.GetButtonDown("UISubmit"))
        {
            manager.RouteInputA(playerNumber);
        }
        if (player.GetButtonDown("UICancel"))
        {
            manager.RouteInputB(playerNumber);

        }
        if (player.GetButtonDown("UIMenu"))
        {
            manager.RouteInputBack();
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

}
