using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerTag : MonoBehaviour
{
    private Text text;
    private SFXManager sfxManager;

    public void Awake()
    {
        text = GetComponentInChildren<Text>();
        text.enabled = false;
        SFXManager.TryGetInstance(out sfxManager);
    }

    public void ShowTag()
    {
        if (sfxManager != null)
        {
            sfxManager.PlayPingSound();
        }
        else if (SFXManager.TryGetInstance(out sfxManager))
        {
            sfxManager.PlayPingSound();
        }
        text.enabled = true;
    }

    public void HideTag()
    {
        text.enabled = false;
    }
}
