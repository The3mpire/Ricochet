﻿using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_MenuButton : MonoBehaviour, IDeselectHandler, ISelectHandler
{
    private Image image;

    [SerializeField] private int value;

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

    public int GetValue()
    {
        return value;
    }
}
