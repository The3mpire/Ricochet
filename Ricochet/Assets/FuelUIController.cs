using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelUIController : MonoBehaviour
{
    [SerializeField]
    private GameObject fuelBubblePrefab;

    [SerializeField]
    private float rotateSpeed = 5f;

    [SerializeField]
    private float radius = 2f;

    [SerializeField]
    private Sprite emptyFuelBubbleSprite;

    [SerializeField]
    private Sprite FuelBubbleSprite;

    private List<SpriteRenderer> fuelBubbles;
    private PlayerController pc;
    private PlayerDashController dashController;

    public void Awake()
    {
        dashController = GetComponentInParent<PlayerDashController>();
        fuelBubbles = new List<SpriteRenderer>();
        pc = GetComponentInParent<PlayerController>();
    }

    public void Start()
    {
        int max = dashController.GetMaxDashCount();

        Vector3[] positions = new Vector3[]
        {
            transform.position + Vector3.down * radius,
            transform.position + Vector3.down * radius + Vector3.right,
            transform.position + Vector3.down * radius + Vector3.left
        };

        for (int i = 0; i < max; i++)
        {
            var sR = Instantiate(fuelBubblePrefab, positions[i], Quaternion.identity, transform).GetComponent<SpriteRenderer>();
            
            fuelBubbles.Add(sR);
        }


    }

    public void Update()
    {
        int count = 0;

        foreach (SpriteRenderer go in fuelBubbles)
        {
            if (count < dashController.GetDashCount())
            {
                go.sprite = FuelBubbleSprite;
                count++;
            }
            else
            {
                go.sprite = emptyFuelBubbleSprite;
            }
            
        }
    }
}
