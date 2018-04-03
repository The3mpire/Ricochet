using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Rewired;
using UnityEngine.EventSystems;

public class SplashScreenManager : MonoBehaviour
{
    [SerializeField] private GameObject _titlePanel;
    [SerializeField] private GameObject _xboxLiveGameObject;
    [SerializeField] private GameObject _mainMenuPanel;
    [SerializeField] private GameObject _characterArtPanel;
    [SerializeField] private MainMenuFunctions _mainMenuFunctions;
    [SerializeField] private EventSystem es;
    private Image _panel;
    private UserProfile _userProfile;
    

    // Use this for initialization
    void Start ()
	{
	    es.enabled = false;
        _panel = GetComponentInChildren<Image>();
	    if (ReInput.players.GetPlayer(0).controllers.joystickCount > 0)
	    {
	        _panel.gameObject.SetActive(false);
	        _titlePanel.SetActive(false);
            SlideInMainMenu(1);
        }
#if UNITY_WSA_10_0 || UNITY_XBOXONE
        _userProfile = _xboxLiveGameObject.GetComponent<UserProfile>();
#endif
    }

    public void StartLogIn()
    {
        
#if UNITY_WSA_10_0 || UNITY_XBOXONE
        if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork) {
            Destroy(_panel.transform.Find("ButtonImage").gameObject);
            _xboxLiveGameObject.SetActive(true);
            _userProfile = _xboxLiveGameObject.GetComponent<UserProfile>();
            GameObject signinButton = _userProfile.signInPanel.transform.Find("SignInButton").gameObject;
            es.SetSelectedGameObject(signinButton);
            StartCoroutine(WaitForSignIn());
        }
#else
        Destroy(_panel.transform.Find("ButtonImage").gameObject);
        StartCoroutine(BeginSplashFadeOut());
        SlideInMainMenu(3f);
        _mainMenuFunctions.SelectDefaultOption();
#endif


    }

    IEnumerator BeginSplashFadeOut()
    {
        _panel.DOFade(0.0f, 2.0f);
        _titlePanel.GetComponent<Image>().DOFade(0.0f, 2.0f);

        yield return new WaitUntil(() => _panel.color.a == 0);

        _panel.gameObject.SetActive(false);
    }

#if UNITY_WSA_10_0 || UNITY_XBOXONE
    IEnumerator WaitForSignIn()
    {
        yield return new WaitUntil(() => !_userProfile.signInPanel.activeSelf);
        StartCoroutine(BeginSplashFadeOut());
        SlideInMainMenu(3f);

    }
#endif
    public void SlideInMainMenu(float duration)
    {
        es.enabled = true;
        _mainMenuFunctions.SelectDefaultOption();
        _mainMenuPanel.GetComponent<PanelSlide>().ExecuteMoveTo(duration);
        _characterArtPanel.GetComponent<PanelSlide>().ExecuteMoveTo(duration);
        
    }
}
