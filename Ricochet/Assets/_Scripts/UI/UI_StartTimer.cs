using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UI_StartTimer : MonoBehaviour
{
    private Text text;
    private GameManager manager;

    private bool fading = false;

    public void Awake()
    {
        text = GetComponentInChildren<Text>();

        GameManager.TryGetInstance(out manager);
    }

    private void OnEnable()
    {
        GameManager.OnStartTimerFinished += StartGameText;
    }

    private void OnDisable()
    {
        GameManager.OnStartTimerFinished -= StartGameText;
    }

    public void UpdateText()
    {
        if (!manager.GameRunning)
        {
            text.text = manager.GetTimeTillMatchStart().ToString();
            StartCoroutine(Timer());
        }
        else if (manager.MatchTimeLeft <= 5)
        {
            text.color = Color.red;
            text.text = manager.MatchTimeLeft.ToString();
            StartCoroutine(Timer());
        }
    }

    private void StartGameText()
    {
        text.text = "GO";
        StartCoroutine(Timer());
    }

    private IEnumerator Timer()
    {
        fading = true;
        text.DOFade(1f, .25f);
        yield return new WaitForSeconds(.5f);
        text.DOFade(0f, .25f).OnComplete(()=>fading = false);
    }
}
