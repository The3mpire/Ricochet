using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
