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
    #endregion
}
