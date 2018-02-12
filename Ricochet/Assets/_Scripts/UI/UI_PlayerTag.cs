using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerTag : MonoBehaviour
{
    private Text text;

    public void Awake()
    {
        text = GetComponentInChildren<Text>();
        text.enabled = false;
    }

    public void ShowTag()
    {
        text.enabled = true;
    }

    public void HideTag()
    {
        text.enabled = false;
    }
}
