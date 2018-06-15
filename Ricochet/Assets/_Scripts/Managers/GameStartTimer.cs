using Rewired;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStartTimer : MonoBehaviour {
    #region Inspector Variables
    [Tooltip("Drag GameData scriptable object here")]
    [SerializeField] private GameDataSO _gameData;
    [Tooltip("Number of seconds until the level loads")]
    [SerializeField] private float _waitTime = 3;
    [Tooltip("Drag ControllerMap here")]
    [SerializeField] private GameObject controllerMap;
    [Tooltip("Drag KeyboardMap here")]
    [SerializeField] private GameObject keyboardMap;
    #endregion

    #region Private  
    private Text _loadingText;
    private float _clock;
    private int _dots;
    #endregion

    #region MonoBehaviour
    private void Start () {
        _loadingText = gameObject.GetComponent<Text>();
        controllerMap.SetActive(false);
        keyboardMap.SetActive(false);
        if (ReInput.players.GetPlayer(0).controllers.hasKeyboard)
        {
            keyboardMap.SetActive(true);
            _waitTime *= 2;
            StartCoroutine(SwitchMap());
        }
        else
        {
            controllerMap.SetActive(true);
        }
	    StartCoroutine(LoadSelectedScene());
	}

    private void Update()
    {
        Tick();
    }
    #endregion

    #region Private Functions
    /// <summary>
    /// Wait for set amount of time, then load scene stored in GameData
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoadSelectedScene()
    {
        yield return new WaitForSeconds(_waitTime);
        SceneManager.LoadScene((int)_gameData.GetGameLevel());
    }

    private IEnumerator SwitchMap()
    {
        yield return new WaitForSeconds(_waitTime/2);
        keyboardMap.SetActive(false);
        controllerMap.SetActive(true);
    }

    /// <summary>
    /// Tick periods after Loading string every .5 seconds.
    /// </summary>
    private void Tick()
    {
        _clock += Time.deltaTime;
        if (_clock >= 0.5)
        {
            switch (_dots)
            {
                case 0:
                    _loadingText.text = "Loading ";
                    _dots++;
                    break;
                case 1:
                    _dots++;
                    _loadingText.text = "Loading. ";
                    break;
                case 2:
                    _dots++;
                    _loadingText.text = "Loading.. ";
                    break;
                case 3:
                    _dots = 0;
                    _loadingText.text = "Loading... ";
                    break;
                default:
                    break;
            }
            _clock = 0;
        } 
    }
    #endregion
}
