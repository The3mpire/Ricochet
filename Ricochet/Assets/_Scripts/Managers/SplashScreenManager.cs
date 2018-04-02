using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Rewired;
using UnityEngine.EventSystems;

public class SplashScreenManager : MonoBehaviour
{
    [SerializeField] private GameObject _titlePanel;
    [SerializeField]
    private GameObject _mainMenuPanel;
    [SerializeField]
    private MainMenuFunctions _mainMenuFunctions;
    private Image _panel;
    [SerializeField]
    private EventSystem es;

    // Use this for initialization
    void Start ()
	{
	    es.enabled = false;
        _panel = GetComponentInChildren<Image>();
	    if (ReInput.players.GetPlayer(0).controllers.joystickCount > 0)
	    {
	        _panel.gameObject.SetActive(false);
	        _titlePanel.GetComponent<PanelSlide>().ExecuteMoveTo(0.0f);
	        _mainMenuPanel.GetComponent<PanelSlide>().ExecuteMoveTo(1);
        }

	}
	
	// Update is called once per frame
	void Update () {
	}

    public void StartLogIn()
    {
        //TODO: Check platform
        //TODO: Show log in prompt
        Debug.Log("Log In");
        Destroy(_panel.transform.Find("ButtonImage").gameObject);
        StartCoroutine(BeginSplashFadeOut());
        es.enabled = true;
        SlideInMainMenu();
        _mainMenuFunctions.SelectDefaultOption();
    }

    IEnumerator BeginSplashFadeOut()
    {
        _panel.DOFade(0.0f, 2.0f);
        _titlePanel.GetComponent<PanelSlide>().ExecuteMoveTo(2.0f);

        yield return new WaitUntil(() => _panel.color.a == 0);

        _panel.gameObject.SetActive(false);
    }

    public void SlideInMainMenu()
    {
        _mainMenuPanel.GetComponent<PanelSlide>().ExecuteMoveTo(3);
    }
}
