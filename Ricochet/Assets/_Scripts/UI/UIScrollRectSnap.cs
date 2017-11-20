using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScrollRectSnap : MonoBehaviour
{
    #region Reference Variables
    [Tooltip("Drag Scroll Panel here")]
    [SerializeField]
    private RectTransform scrollPanel;
    [Tooltip("Drag all the buttons here")]
    [SerializeField]
    public Button[] buttons;
    [Tooltip("Drag the center Compare here")]
    [SerializeField]
    public RectTransform center;
    #endregion

    #region Hidden Variables
    private float[] distance;
    private bool dragging = false;
    private int buttonDistance;
    private int minButtonNum;
    #endregion

    #region MonoBehaviour
    void Start()
    {
        int buttonLength = buttons.Length;

    }
    
    void Update()
    {

    }
    #endregion
}
