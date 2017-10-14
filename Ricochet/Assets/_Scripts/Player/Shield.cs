using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    [SerializeField]
    private PlayerController player;

    public PlayerController GetPlayer()
    {
        return player;
    }

}
