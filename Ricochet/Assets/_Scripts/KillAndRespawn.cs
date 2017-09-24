using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillAndRespawn : MonoBehaviour
{

    private PlayerController playerController;
    private int playerNumber;
    private int teamNum;

    public bool isPlayerDead;
    public Transform respawnPoint1;
    public Transform respawnPoint2;
    public GameObject player;
    public SpriteRenderer sprite;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerNumber = other.transform.parent.gameObject.GetComponent<PlayerController>().playerNumber;
            teamNum = other.transform.parent.gameObject.GetComponent<PlayerController>().teamNumber;
            Destroy(other.transform.parent.gameObject);
            StartCoroutine(respawnPlayer(playerNumber));
        }
    }

    private IEnumerator respawnPlayer(int playerNum)
    {
        yield return new WaitForSeconds(2f);
        player.GetComponent<PlayerController>().playerNumber = playerNum;
        player.GetComponent<PlayerController>().teamNumber = teamNum;

        switch (teamNum)
        {
            case 1:
                Instantiate(player, respawnPoint1.position, respawnPoint1.rotation);
                break;
            case 2:
                Instantiate(player, respawnPoint2.position, respawnPoint2.rotation);
                break;
        }
    }
}
