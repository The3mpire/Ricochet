using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_MatchTimer : MonoBehaviour
{
    [SerializeField]
    private Text text;

    public void UpdateText(float timeInSeconds)
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
}
