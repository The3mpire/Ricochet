using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTaunt : MonoBehaviour
{
    private PlayerController playerController;
    private Player rewiredPlayer;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    private void Start()
    {
        
    }
}
