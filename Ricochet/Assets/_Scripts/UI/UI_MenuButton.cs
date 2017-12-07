using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_MenuButton : MonoBehaviour, IDeselectHandler, ISelectHandler
{
    private Image image;

    public void Awake()
    {
        image = GetComponent<Image>();   
    }

    private void OnDisable()
    {
        image.color = Color.white;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        image.color = Color.white;
    }

    public void OnSelect(BaseEventData eventData)
    {
        image.color = Color.magenta;
    }

}
