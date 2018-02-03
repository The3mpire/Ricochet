using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelUIController : MonoBehaviour
{
    public GameObject fuelBubblePrefab;

    private List<GameObject> fuelBubbles;
    private PlayerController pc;

    public void Awake()
    {
    }
}
