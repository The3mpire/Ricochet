using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillAndRespawn : MonoBehaviour
{

    private PlayerController playerController;
    private int playerNumber;
    private int teamNum;
    
    public Transform respawnPoint1;
    public Transform respawnPoint2;
    public GameObject player;

    void OnCollisionEnter2D(Collision2D col)
    {
        Collider2D other = col.collider;
        if (other.gameObject.tag == "Player")
        {
            PlayerController pc = other.GetComponent<PlayerController>();
            playerNumber = pc.playerNumber;
            teamNum = pc.teamNumber;
            Destroy(pc.gameObject);
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
