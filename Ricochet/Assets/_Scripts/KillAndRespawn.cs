using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillAndRespawn : MonoBehaviour
{
    public Transform respawnPoint1;
    public Transform respawnPoint2;

    public ScoreManager ScoreUI;
    public GameObject ball;

    private PlayerController playerController;

    void OnCollisionEnter2D(Collision2D col)
    {
        Collider2D other = col.collider;
        if (other.gameObject.tag == "Player")
        {
            PlayerController pc = other.GetComponent<PlayerController>();

            pc.gameObject.SetActive(false);
            pc.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            ScoreUI.Score(pc.teamNumber == 1 ? 'b' : 'r', -1);
            StartCoroutine(RespawnPlayer(pc));
        }
        else if (other.name == "Shield")
        {
            PlayerController player = other.gameObject.GetComponentInParent<PlayerController>();
            //see if they have the power up
            if (player.hasPowerUp)
            {
                SpawnBalls();
                player.hasPowerUp = false;
                player.shield.color = Color.white;
            }
        }

    }

    private void SpawnBalls()
    {
        GameObject ball1 = Instantiate(ball);
        GameObject ball2 = Instantiate(ball);

        ball1.GetComponent<BallMovement>().isTempBall = true;
        ball2.GetComponent<BallMovement>().isTempBall = true;
    }

    private IEnumerator RespawnPlayer(PlayerController player)
    {
        yield return new WaitForSeconds(2f);
        player.gameObject.SetActive(true);

        switch (player.teamNumber)
        {
            case 1:
                player.transform.position = respawnPoint1.position;
                player.transform.rotation = respawnPoint1.rotation;
                break;
            case 2:
                player.transform.position = respawnPoint2.position;
                player.transform.rotation = respawnPoint2.rotation;
                break;
        }
    }
}
