﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class CharSelect_Assign : MonoBehaviour
{
    #region Private
    [SerializeField]
    [Tooltip("Drag CharSelectManager here")]
    private GameObject managerPanel;
    private CharSelectManager manager;
    #endregion
    #region MonoBehaviour
    void Awake()
    {
        manager = managerPanel.GetComponent<CharSelectManager>();
        ReInput.ControllerConnectedEvent += OnControllerConnected;

        // Assign each Joystick to a Player initially
        foreach (Joystick j in ReInput.controllers.Joysticks)
        {
            if (ReInput.controllers.IsJoystickAssigned(j))
            {
                continue;
            }

            AssignJoystickToNextOpenPlayer(j);
        }
        foreach (var player in ReInput.players.Players)
        {
            if (player.controllers.joystickCount > 0)
            {
                manager.ActivatePlayer(player.id);
            }
        }
    }
    #endregion
    #region Rewired

    void OnControllerConnected(ControllerStatusChangedEventArgs args)
    {
        if (args.controllerType != ControllerType.Joystick) return;
        foreach (Player p in ReInput.players.Players) { 
            if (p.controllers.ContainsController(args.controllerType, args.controllerId)) 
            {
                manager.ActivatePlayer(p.id);
            }
        
        }
    }
    #endregion
    #region Helpers
    void AssignJoystickToNextOpenPlayer(Joystick j)
    {
        foreach (Player p in ReInput.players.Players)
        {
            if (p.controllers.joystickCount > 0)
            {
                continue;
            }
            p.controllers.AddController(j, true);
            return;
        }
    }
    #endregion
}
