using UnityEngine;
using UnityEngine.UI;

public class UI_FuelGauge : MonoBehaviour
{
    [SerializeField] private Image fill;
    [SerializeField] private PlayerController player;

    private void Update ()
    {
        HandleBar();
	}

    private void HandleBar ()
    {
        if (fill == null || player == null)
        {
            Debug.LogWarning(name + " is missing reference variables.", gameObject);
        }

        fill.rectTransform.localScale = new Vector3(1, player.GetCurrentFuel() / player.GetMaxFuel(), 1);
    }
}
