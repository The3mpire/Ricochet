using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_MenuButton : MonoBehaviour, IDeselectHandler, ISelectHandler
{
    private Text text;

    public void Awake()
    {
        text = GetComponentInChildren<Text>();   
    }

    private void OnDisable()
    {
        text.color = Color.white;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        text.color = Color.white;
    }

    public void OnSelect(BaseEventData eventData)
    {
        text.color = Color.yellow;
    }

}
