using UnityEngine;
using Rewired;

[AddComponentMenu("")]
public class InitPlayerOne : MonoBehaviour
{
    [SerializeField] private SplashScreenManager _splashScreenManager;

    private bool hadKeyboard;
    private Player playerOne;
    private Joystick joystickOne;
    private Keyboard keyboard;
    private SFXManager sfx;

    // called when main menu loads to undo character select mappings
    void Awake()
    {
        hadKeyboard = false;

        // see if we've already assigned the keyboard, and can thus
        //   skip the splash screen when returning to the main meu
        playerOne = ReInput.players.GetPlayer(0);
        if (playerOne.controllers.hasKeyboard)
            hadKeyboard = true;

        // unmap all controllers and keyboard from all players
        foreach (Player p in ReInput.players.Players)
        {
            p.controllers.ClearAllControllers();
            p.controllers.hasKeyboard = false;
        }

        // return the first controller to the first player
        if (ReInput.controllers.joystickCount > 0)
        {
            joystickOne = ReInput.controllers.GetJoystick(0);
            playerOne.controllers.AddController(joystickOne, true);
        }

        // return the keyboard to the first player
        keyboard = ReInput.controllers.Keyboard;
        playerOne.controllers.hasKeyboard = true;

        ReInput.ControllerConnectedEvent += OnControllerConnected;
    }

    private void Update()
    {
        if (!ReInput.isReady) return;
        if(!hadKeyboard)
            CheckForButtonPress();
    }

    private void CheckForButtonPress()
    {
        if(playerOne.GetAnyButtonDown())
        {
            if (SFXManager.TryGetInstance(out sfx))
                sfx.PlayMenuClickSound();
            _splashScreenManager.StartLogIn();
            hadKeyboard = true;
        }
    }

    // if we add a controller when there was none, give it to player one
    private void OnControllerConnected(ControllerStatusChangedEventArgs data)
    {
        if (playerOne != null && playerOne.controllers.joystickCount == 0)
        {
            joystickOne = ReInput.controllers.GetJoystick(data.controllerId);
            playerOne.controllers.AddController(joystickOne, true);
        }
    }

    public bool CanSkipSplash()
    {
        return hadKeyboard;
    }
}