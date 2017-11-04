using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class _UICharacterSelectControler : MonoBehaviour {
    #region Private Variables
    Player playerInput1;
    Player playerInput2;
    Player playerInput3;
    Player playerInput4;
    #endregion

    #region MonoBehaviour
    void Awake()
    {
        playerInput1 = ReInput.players.GetPlayer(0);
        playerInput2 = ReInput.players.GetPlayer(1);
        playerInput3 = ReInput.players.GetPlayer(2);
        playerInput4 = ReInput.players.GetPlayer(3);

    }

    void Update()
    {

    }
    #endregion
}
