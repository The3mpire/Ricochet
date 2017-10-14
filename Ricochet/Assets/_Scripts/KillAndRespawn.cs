using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillAndRespawn : MonoBehaviour
{
    public Transform respawnPoint1;
    public Transform respawnPoint2;

    public ScoreManager ScoreUI;
    public GameObject ball;

    //private void SpawnBalls()
    //{
    //    GameObject ball1 = Instantiate(ball);
    //    GameObject ball2 = Instantiate(ball);

    //    ball1.GetComponent<Ball>().isTempBall = true;
    //    ball2.GetComponent<Ball>().isTempBall = true;
    //}

    //private IEnumerator RespawnPlayer(PlayerController player)
    //{
    //    yield return new WaitForSeconds(2f);
    //    player.gameObject.SetActive(true);

    //    switch (player.teamNumber)
    //    {
    //        case 1:
    //            player.transform.position = respawnPoint1.position;
    //            player.transform.rotation = respawnPoint1.rotation;
    //            break;
    //        case 2:
    //            player.transform.position = respawnPoint2.position;
    //            player.transform.rotation = respawnPoint2.rotation;
    //            break;
    //    }
    //}
}
