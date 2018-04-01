using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Rewired;

public class SplashScreenManager : MonoBehaviour
{
    [SerializeField] private GameObject _titlePanel;
    [SerializeField]
    private GameObject _mainMenuPanel;
    [SerializeField]
    private MainMenuFunctions _mainMenuFunctions;
    private Image _panel;

	// Use this for initialization
	void Start ()
	{
	    _panel = GetComponentInChildren<Image>();

	}
	
	// Update is called once per frame
	void Update () {
        //TODO:Start fade out of splash screen and main menu slide in
	}

    public void StartLogIn()
    {
        //TODO: Check platform
        //TODO: Show log in prompt
        Debug.Log("Log In");
        Destroy(_panel.transform.Find("ButtonImage").gameObject);
        StartCoroutine(BeginSplashFadeOut());
        SlideInMainMenu();
        _mainMenuFunctions.SelectDefaultOption();
    }

    IEnumerator BeginSplashFadeOut()
    {
        Debug.Log("Fading out");
        _panel.DOFade(0.0f, 2.0f);
        _titlePanel.GetComponent<PanelSlide>().ExecuteMoveTo(2.0f);

        yield return new WaitUntil(() => _panel.color.a == 0);

        Debug.Log("Fade complete");
        _panel.gameObject.SetActive(false);
    }

    public void SlideInMainMenu()
    {
        _mainMenuPanel.GetComponent<PanelSlide>().ExecuteMoveTo(3);
    }
}
