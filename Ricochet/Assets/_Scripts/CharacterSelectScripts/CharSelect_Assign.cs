using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class CharSelect_Assign : MonoBehaviour {
    #region Private
    [SerializeField]
    [Tooltip("Drag player cursor objects here")]
    private GameObject[] cursors;
    #endregion
    #region MonoBehaviour
    void Awake()
    {
        ReInput.ControllerConnectedEvent += OnControllerConnected;

        // Assign each Joystick to a Player initially
        foreach (Joystick j in ReInput.controllers.Joysticks)
        {
            if (ReInput.controllers.IsJoystickAssigned(j))
            {
                Debug.Log("Controller already assigned.");
                continue;
            }
            
            AssignJoystickToNextOpenPlayer(j);
        }
    }
#endregion
    #region Rewired
    void OnControllerConnected(ControllerStatusChangedEventArgs args)
    {
        if (args.controllerType != ControllerType.Joystick) return;
        
        AssignJoystickToNextOpenPlayer(ReInput.controllers.GetJoystick(args.controllerId));
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
            //cursors[p.id-1].SetActive(true);
            return;
        }
    }
    #endregion
}
