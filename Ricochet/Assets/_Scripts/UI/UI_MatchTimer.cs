using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UI_MatchTimer : MonoBehaviour
{
    [SerializeField]
    private Text text;

    [SerializeField]
    private float yellowPulseTime;

    [SerializeField]
    private float yellowPulseSize;

    [SerializeField]
    private float redPulseTime;

    [SerializeField]
    private float redPulseSize;
    
    private Vector3 originalScale;

    private GameManager gm;

    public void Awake()
    {
        originalScale = transform.localScale;
        GameManager.TryGetInstance(out gm);
    }

    public void UpdateText()
    {
        SetText(gm.MatchTimeLeft);
        if (gm.MatchTimeLeft <= redPulseTime)
        {
            text.color = Color.red;
            Pulse(redPulseSize);
        }
        else if (gm.MatchTimeLeft <= yellowPulseTime)
        {
            text.color = Color.yellow;
            Pulse(yellowPulseSize);    
        }
    }

    public void SetText(float timeInSeconds)
    {
        string minutes = ((int)(timeInSeconds / 60)).ToString();
        int seconds_num = (int)(timeInSeconds % 60);
        string seconds;
        if (seconds_num < 10)
        {
            seconds = '0' + seconds_num.ToString();
        }
        else
        {
            seconds = seconds_num.ToString();
        }
        text.text = minutes + ':' + seconds;
    }

    private void Pulse(float pulseSize)
    {
        transform.DOScale(originalScale, .5f).SetEase(Ease.InFlash).OnComplete(
            () => transform.DOScale(pulseSize * Vector2.one, .5f).SetEase(Ease.InQuad));
    }

}
