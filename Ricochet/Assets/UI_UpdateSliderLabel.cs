using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_UpdateSliderLabel : MonoBehaviour, ISelectHandler, IDeselectHandler {

    [SerializeField] private Text disc;
    [SerializeField] private Text label;
    [SerializeField] private Slider slider;

    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color selectedColor = Color.red;

    private int lastValue;

    private void Awake()
    {
        lastValue = (int)slider.value;
    }

    public void UpdateText()
    {
        label.text = slider.value.ToString();
        
    }

    public void UpdateTimeText()
    {
        if (slider.value > lastValue)
        {
            lastValue += 5;
            slider.value = lastValue;
        }
        else if (slider.value < lastValue)
        {
            lastValue -= 5;
            slider.value = lastValue;
        }
        string minSec = string.Format("{0}:{1:00}", (int)slider.value / 60, (int)slider.value % 60);
        label.text = minSec;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        //label.color = defaultColor;
        disc.color = defaultColor;
    }

    public void OnSelect(BaseEventData eventData)
    {
        //label.color = Color.red;
        disc.color = selectedColor;
    }

}
