using UnityEngine;

public class UI_Indicator : MonoBehaviour
{

    [SerializeField]
    private GameObject indicatorGameObject;

    public void ActivateIndicator()
    {
        indicatorGameObject.SetActive(true);
    }

    public void DeactivateIndicator()
    {
        indicatorGameObject.SetActive(false);
    }
}
