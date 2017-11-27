using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuSelectButton : MonoBehaviour, IDeselectHandler, ISelectHandler
{
    private Image image;

    public void Awake()
    {
        image = GetComponent<Image>();   
    }

    public void OnDeselect(BaseEventData eventData)
    {
        image.color = Color.white;
    }

    public void OnSelect(BaseEventData eventData)
    {
        image.color = Color.red;
    }
}
