using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStartTimer : MonoBehaviour {
    #region Serialized
    [SerializeField]
    private GameDataSO _gameData;
    [Tooltip("Number of seconds until the level loads")]
    [SerializeField]
    private float _waitTime = 3;
    #endregion

    #region Private  
    private Text _loadingText;
    private float _clock;
    private int _dots;
    #endregion

    #region MonoBehaviour
    private void Start () {
        _loadingText = gameObject.GetComponent<Text>();
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
