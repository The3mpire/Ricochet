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

        for (int i=0; i<max; i++)
        {
            float count = (float)i/ max;
            float angle = count * Mathf.PI * 2;
            float x = Mathf.Sin(angle) * radius;
            float y = Mathf.Cos(angle) * radius;
            Vector3 pos = transform.position + new Vector3(x, y, 0);

            var sR = Instantiate(fuelBubblePrefab, pos, Quaternion.identity, transform).GetComponent<SpriteRenderer>();

            sR.color = (pc.GetTeamNumber() == Enumerables.ETeam.BlueTeam ? new Color32(0, 0, 255, 255) : new Color32(255, 0, 0, 255));

            fuelBubbles.Add(sR);
        }
    }

    public void Update()
    {
        transform.Rotate(0f, 0f, rotateSpeed);

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
