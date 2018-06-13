using Rewired;
using UnityEngine;

public class CharSelect_Assign : MonoBehaviour
{
    [Tooltip("Drag CharSelectManager here")]
    [SerializeField] private GameObject managerPanel;

    private bool canAddKeyboard;
    private CharSelectManager manager;
    private Keyboard keyboard;
    private Player playerOne;

    #region MonoBehaviour
    private void Awake()
    {
        canAddKeyboard = true;
        manager = managerPanel.GetComponent<CharSelectManager>();
        playerOne = ReInput.players.GetPlayer(0);
        keyboard = ReInput.controllers.Keyboard;

        // remove keyboard and controllers from player one to make things conceptually simple
        playerOne.controllers.ClearAllControllers();

        ReInput.ControllerConnectedEvent += OnControllerConnected;
    }

    private void Update()
    {
        foreach (Joystick j in ReInput.controllers.Joysticks)
        {
            if (!ReInput.controllers.IsJoystickAssigned(j) && j.GetAnyButtonDown())
                AssignJoystickToNextOpenPlayer(j);
        }
        if (canAddKeyboard && keyboard.GetAnyButtonDown())
            AssignKeyboardToPlayerOne();
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
                manager.RouteActivationInput(p.id);
            }
        
        }
    }
    #endregion

    #region Helpers
    private bool AssignJoystickToNextOpenPlayer(Joystick j)
    {
        foreach (Player p in ReInput.players.Players)
        {
            if (p.controllers.joystickCount == 0 && !p.controllers.hasKeyboard)
            {
                p.controllers.AddController(j, true);
                manager.ActivatePlayer(p.id);
                manager.RouteActivationInput(p.id);
                return true;
            }
        }
        return false;
    }

    private void AssignKeyboardToPlayerOne()
    {
        if (playerOne.controllers.joystickCount > 0)
        {
            Joystick j = playerOne.controllers.Joysticks[0];
            if (!AssignJoystickToNextOpenPlayer(j))
                return;
        }

        playerOne.controllers.hasKeyboard = true;
        manager.ActivatePlayer(playerOne.id);
        manager.RouteActivationInput(playerOne.id);
        canAddKeyboard = false;
    }
    #endregion
}
