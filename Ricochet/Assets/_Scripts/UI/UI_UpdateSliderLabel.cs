using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_UpdateSliderLabel : MonoBehaviour, ISelectHandler, IDeselectHandler
{

    #region Inspector Variables
    [SerializeField] private Text disc;
    [SerializeField] private Text label;
    [SerializeField] private Slider slider;

    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color selectedColor = Color.red;
    #endregion

    #region Private Variables
    private int lastValue;
    #endregion

    #region Monobehaviours
    private void Awake()
    {
        lastValue = (int)slider.value;
    }
    
    #endregion

    #region Public Functions
    public void UpdateText()
    {
        label.text = slider.value.ToString();
    }

    public void UpdateTimeText()
    {
        if (slider.value > lastValue)
        {
            lastValue = (int)slider.value;
            slider.value = Mathf.Ceil(lastValue / 5.0f) * 5;
        }
        else if (slider.value < lastValue)
        {
            lastValue = (int)slider.value;
            slider.value = Mathf.Floor(lastValue / 5.0f) * 5;
        }
        string minSec = string.Format("{0}:{1:00}", (int)slider.value / 60, (int)slider.value % 60);
        label.text = minSec;
    }
    #endregion

    #region ISelect and IDeselect Methods
    public void OnDeselect(BaseEventData eventData)
    {
        disc.color = defaultColor;
    }

    public void OnSelect(BaseEventData eventData)
    {
        disc.color = selectedColor;
    }
    #endregion

}
