using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class UIScrollRectSnap : MonoBehaviour
{
    #region Reference Variables
    [Tooltip("How fast the buttons snap (bigger number is longer)")]
    [SerializeField]
    private float snapSpeed = 2f;
    [Tooltip("How far the buttons need to be before the wrap around")]
    [SerializeField]
    private int wrapDistance = 1000;

    [Header("Reference Variables)")]
    [Tooltip("Drag Scroll Panel here")]
    [SerializeField]
    private RectTransform scrollPanel;
    [Tooltip("Drag all the buttons here")]
    [SerializeField]
    public RectTransform[] buttons;
    [Tooltip("Drag the center Compare here")]
    [SerializeField]
    public RectTransform center;
    #endregion

    #region Hidden Variables
    private float[] distance;
    private float[] distReposition;
    private bool dragging = false;
    private int buttonDistance;
    private int minButtonNum;
    #endregion

    #region MonoBehaviour
    void Start()
    {
        Cursor.visible = true;
        distance = new float[buttons.Length];
        distReposition = new float[buttons.Length];

        buttonDistance = (int)Mathf.Abs(buttons[1].anchoredPosition.x - buttons[0].GetComponent<RectTransform>().anchoredPosition.x);
    }

    void Update()
    {
        float minDistance = Mathf.Min(distance);

        for (int i = 0; i < buttons.Length; i++)
        {
            distReposition[i] = center.position.x - buttons[i].position.x;
            distance[i] = Mathf.Abs(center.transform.position.x - buttons[i].transform.position.x);

            float curX = buttons[i].anchoredPosition.x;
            float curY = buttons[i].anchoredPosition.y;

            if (distReposition[i] > wrapDistance)
            {
                Vector2 newAnchoredPos = new Vector2(curX + (buttons.Length * buttonDistance), curY);
                buttons[i].anchoredPosition = newAnchoredPos;
            }
            else if (distReposition[i] < -wrapDistance)
            {
                Vector2 newAnchoredPos = new Vector2(curX - (buttons.Length * buttonDistance), curY);
                buttons[i].anchoredPosition = newAnchoredPos;
            }

            if (minDistance >= distance[i])
            {
                minButtonNum = i;
            }
        }

        if (!dragging)
        {
            LerpToAThing(-buttons[minButtonNum].anchoredPosition.x);
        }
    }

    private void LerpToAThing(float position)
    {
        float newX = Mathf.Lerp(scrollPanel.anchoredPosition.x, position, Time.deltaTime * snapSpeed);

        scrollPanel.anchoredPosition = new Vector2(newX, scrollPanel.anchoredPosition.y);
    }
    #endregion

    #region Event Functionalities
    public void StartDrag()
    {
        dragging = true;
    }

    public void EndDrag()
    {
        dragging = false;
    }
    #endregion
}
