using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UI_FuelGauge2 : MonoBehaviour
{
    [SerializeField] private Image fill;
    [SerializeField] private Image background;
    [SerializeField] private PlayerController player;

    [Range(0, 1)]
    [SerializeField] private float criticalFuelPercentage;
    [Range(0, 1)]
    [SerializeField] private float lowFuelPercentage;

    [SerializeField] private Color criticalFuelColor;
    [SerializeField] private Color lowFuelColor;
    [SerializeField] private Color plentyOfFuelColor;

    private bool pulsing = false;
    private bool growing = false;

    private void Start()
    {
        if (!player.gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
    }

    private void LateUpdate()
    {
        HandleBar();

        Pulse();
    }

    private void Pulse()
    {
        if (pulsing)
        {
            if (growing)
            {
                transform.DOScaleY(1.6f, .25f).OnComplete(() => growing = false);
            }
            else
            {
                transform.DOScaleY(1f, .25f).OnComplete(()=>growing=true);
            }
        }
        transform.DOScaleY(1f, .25f);
    }

    private void HandleBar()
    {
        if (fill == null || player == null)
        {
            Debug.LogError(name + " is missing reference variables.", gameObject);
            return;
        }

        float fillPerc = player.GetCurrentFuel() / player.GetMaxFuel();

        pulsing = false;
        if (fillPerc < criticalFuelPercentage)
        {
            background.color = criticalFuelColor;
            fill.color = criticalFuelColor;
            pulsing = true;
        }
        else if (fillPerc < lowFuelPercentage)
        {
            background.color = lowFuelColor;
            fill.color = lowFuelColor;
        }
        else
        {
            background.color = plentyOfFuelColor;
            fill.color = plentyOfFuelColor;
        }

        fill.rectTransform.localScale = new Vector3(fillPerc, 1, 1);

        
    }
}
