using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Rewired;
using UnityEngine.EventSystems;

public class SplashScreenManager : MonoBehaviour
{
    [Header("Title Panel")]
    [SerializeField] private Image _background;
    [SerializeField] private GameObject _titlePanel;
    [SerializeField] private PanelSlide _glitchPanelSlide;
    [SerializeField] private PanelSlide _ballPanelSlide;
    [SerializeField] private Flyby _flyby;
    [SerializeField] private GameObject _ball;
    [Header("Main Menu Panel")]
    [SerializeField] private GameObject _xboxLiveGameObject;
    [SerializeField] private GameObject _mainMenuPanel;
    [SerializeField] private GameObject _characterArtPanel;
    [SerializeField] private MainMenuFunctions _mainMenuFunctions;
    [SerializeField] private EventSystem es;
    [Header("Variables")]
    [SerializeField] private float _titleSlideDuration = 1f;
    [SerializeField]
    [Tooltip("Time in seconds that it takes to fade out the Splash Screen")]
    private float _fadeOutDuration = 2f;
    [SerializeField]
    [Tooltip("Time in seconds that it takes to slide in the main menu")]
    private float _menuSlideInDuration = 2f;
    
    private Image _panel;
    private GameObject _buttonImage;
    #if UNITY_WSA_10_0 || UNITY_XBOXONE
    private UserProfile _userProfile;
    #endif
    

    // Use this for initialization
    void Start ()
	{
	    es.enabled = false;
        _panel = transform.Find("SplashPanel").GetComponent<Image>();
        _buttonImage = _panel.transform.Find("ButtonImage").gameObject;

        if (ReInput.players.GetPlayer(0).controllers.joystickCount > 0)
        {
            DeactivateSplashScreen();
            SlideInMainMenu(1);
        }
#if UNITY_WSA_10_0 || UNITY_XBOXONE
        _userProfile = _xboxLiveGameObject.GetComponent<UserProfile>();
#endif
	    StartCoroutine(DoTitleFlyIn());
	}

    public void StartLogIn()
    {
        
#if UNITY_WSA_10_0 || UNITY_XBOXONE
        if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
        {
            Destroy(_buttonImage);
            _xboxLiveGameObject.SetActive(true);
            _userProfile = _xboxLiveGameObject.GetComponent<UserProfile>();
            GameObject signinButton = _userProfile.signInPanel.transform.Find("SignInButton").gameObject;
            es.SetSelectedGameObject(signinButton);
            StartCoroutine(WaitForSignIn());
            StartCoroutine(BeginSplashFadeOut());
            SlideInMainMenu(_menuSlideInDuration);
        }
        else
        {
            Destroy(_buttonImage);
            StartCoroutine(BeginSplashFadeOut());
            SlideInMainMenu(_menuSlideInDuration);
            _mainMenuFunctions.SelectDefaultOption();
            _xboxLiveGameObject.SetActive(true);
            _userProfile = _xboxLiveGameObject.GetComponent<UserProfile>();
        }
#else
        Destroy(_buttonImage);
        StartCoroutine(BeginSplashFadeOut());
        SlideInMainMenu(_menuSlideInDuration);
        _mainMenuFunctions.SelectDefaultOption();
#endif


    }

    public void SlideInMainMenu(float duration)
    {
        es.enabled = true;
        _mainMenuFunctions.SelectDefaultOption();
        _mainMenuPanel.GetComponent<PanelSlide>().ExecuteMoveTo(duration);
        _characterArtPanel.GetComponent<PanelSlide>().ExecuteMoveTo(duration);

    }

    private void DeactivateSplashScreen()
    {
        _background.gameObject.SetActive(false);
        _panel.gameObject.SetActive(false);
        _flyby.gameObject.SetActive(false);
        _ball.SetActive(false);
    }

#region Coroutines

    IEnumerator DoTitleFlyIn()
    {
        Tween glitchTween = _glitchPanelSlide.ExecuteMoveTo(_titleSlideDuration);
        yield return glitchTween.WaitForCompletion();
        Tween ballTween = _ballPanelSlide.ExecuteMoveTo(_titleSlideDuration);
        yield return ballTween.WaitForCompletion();
        try
        {
            _buttonImage.GetComponent<Image>().DOFade(1f, 2f);
            _flyby.StartFlyby();
        }
        catch (MissingReferenceException e)
        {
            
        }
    }

    IEnumerator BeginSplashFadeOut()
    {
        _panel.DOFade(0.0f, _fadeOutDuration);
        _background.DOFade(0.0f, _fadeOutDuration);
        _glitchPanelSlide.gameObject.GetComponent<Image>().DOFade(0.0f, _fadeOutDuration);
        _ballPanelSlide.gameObject.GetComponent<Image>().DOFade(0.0f, _fadeOutDuration);
        _flyby.gameObject.transform.DOScale(Vector3.zero, _fadeOutDuration);
        _ball.transform.DOScale(Vector3.zero, _fadeOutDuration);

        yield return new WaitUntil(() => _panel.color.a == 0);

        DeactivateSplashScreen();
    }

#if UNITY_WSA_10_0 || UNITY_XBOXONE
    IEnumerator WaitForSignIn()
    {
        yield return new WaitUntil(() => !_userProfile.signInPanel.activeSelf);
    }
#endif
#endregion
}
