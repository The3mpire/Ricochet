using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UI_GameOverMenu : MonoBehaviour
{
    [SerializeField] private Button defaultSelectedButton;

    #region MonoBehaviours
    private void Start()
    {
        

    }

    private void Update()
    {

    }
    #endregion

    public void ShowGameOverMenu()
    {
        EventSystem.current.SetSelectedGameObject(defaultSelectedButton);
    }
}
