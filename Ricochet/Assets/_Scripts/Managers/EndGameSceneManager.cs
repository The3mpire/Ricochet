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
        EndGameText.text = GameData.gameWinner == Enumerables.ETeam.None ? "Tie Game!" : "Congratulations "+ GameData.gameWinner.ToString() + "!";    
    }

    // Update is called once per frame
    void Update () {
        foreach (var player in ReInput.players.AllPlayers)
        {
            if (player.GetButtonDown("UISubmit"))
            {
                GameData.ResetGameStatistics();
                SceneManager.LoadSceneAsync("CharacterSelect");
            }
            if (player.GetButtonDown("UICancel"))
            {
                GameData.ResetGameStatistics();
                GameData.ResetGameSetup();
                SceneManager.LoadSceneAsync("MainMenu");
            }
        }
    }
    #endregion
}
