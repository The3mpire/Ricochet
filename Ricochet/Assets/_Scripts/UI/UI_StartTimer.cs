using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UI_StartTimer : MonoBehaviour
{
    private Text text;
    private GameManager manager;

    public void Awake()
    {
        text = GetComponentInChildren<Text>();

        GameManager.TryGetInstance(out manager);
    }

    public void UpdateText()
    {
        text.text = manager.GetTimeTillMatchStart().ToString();
        StartCoroutine(Timer());
    }

    private IEnumerator Timer()
    {
        text.DOFade(1f, .25f);
        yield return new WaitForSeconds(.5f);
        text.DOFade(0f, .25f);
    }
}
