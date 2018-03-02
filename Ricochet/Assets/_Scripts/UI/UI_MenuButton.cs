using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_MenuButton : MonoBehaviour, IDeselectHandler, ISelectHandler
{
    protected Text text;

    public void Awake()
    {
        text = GetComponentInChildren<Text>();   
    }

    private void OnDisable()
    {
        text.color = Color.white;
    }

    public virtual void OnDeselect(BaseEventData eventData)
    {
        text.color = Color.white;
    }

    public virtual void OnSelect(BaseEventData eventData)
    {
        text.color = Color.yellow;
    }

}
