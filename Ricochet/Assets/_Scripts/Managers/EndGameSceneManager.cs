using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Rewired;

public class EndGameSceneManager : MonoBehaviour {
    #region Private
    [Tooltip("Drag the UI's EndGameText here")]
    [SerializeField]
    private Text EndGameText;
    #endregion
    #region MonoBehaviour
    private void Start()
    {
        EndGameText.text = "Congratulations "+ GameData.gameWinner.ToString() + "!";    
    }

    // Update is called once per frame
    void Update () {
        foreach (var player in ReInput.players.AllPlayers)
        {
            if (player.GetButtonDown("A Button"))
            {
                GameData.ResetGameStatistics();
                SceneManager.LoadSceneAsync("CharacterSelect");
            }
            if (player.GetButtonDown("B Button"))
            {
                GameData.ResetGameStatistics();
                SceneManager.LoadSceneAsync("MainMenu");
            }
        }
    }
    #endregion
}
