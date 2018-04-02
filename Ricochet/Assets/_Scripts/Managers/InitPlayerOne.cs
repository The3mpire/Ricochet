using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Rewired;

[AddComponentMenu("")]
public class InitPlayerOne : MonoBehaviour
{
    [SerializeField] private SplashScreenManager _splashScreenManager;

    private void Update()
    {
        if (!ReInput.isReady) return;
        AssignJoysticksToPlayerOne();
    }

    private void AssignJoysticksToPlayerOne()
    {
        // Check all joysticks for a button press and assign it to player 1
        IList<Joystick> joysticks = ReInput.controllers.Joysticks;
        for (int i = 0; i < joysticks.Count; i++)
        {

            Joystick joystick = joysticks[i];
            if (ReInput.controllers.IsControllerAssigned(joystick.type, joystick.id)) continue; // joystick is already assigned to a Player

            // Chec if a button was pressed on the joystick
            if (joystick.GetAnyButtonDown())
            {
                Player player = ReInput.players.Players[0];
                player.controllers.AddController(joystick, false);
                Debug.Log("Player One assigned");
                _splashScreenManager.StartLogIn();
                ReInput.configuration.autoAssignJoysticks = true;
                enabled = false; // disable this script
            }
        }
    }
}