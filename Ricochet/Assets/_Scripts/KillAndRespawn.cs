using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillAndRespawn : MonoBehaviour
{

    private PlayerController playerController;
    //private int playerNumber;
    //private int teamNum;
    
    public Transform respawnPoint1;
    public Transform respawnPoint2;
    public GameObject player;

    void OnCollisionEnter2D(Collision2D col)
    {
        Collider2D other = col.collider;
        if (other.gameObject.tag == "Player")
        {
            PlayerController pc = other.GetComponent<PlayerController>();
            int playerNumber = pc.playerNumber;
            int teamNum = pc.teamNumber;
            Destroy(pc.gameObject);
            StartCoroutine(respawnPlayer(playerNumber, teamNum));
        }
    }

    private IEnumerator respawnPlayer(int playerNum, int teamNum)
    {
        yield return new WaitForSeconds(2f);
        PlayerController pc;
        switch (teamNum)
        {
            case 1:
                pc = Instantiate(player, respawnPoint1.position, respawnPoint1.rotation).GetComponent<PlayerController>();
                pc.playerNumber = playerNum;
                pc.teamNumber = teamNum;
                break;
            case 2:
                pc = Instantiate(player, respawnPoint2.position, respawnPoint2.rotation).GetComponent<PlayerController>();
                pc.playerNumber = playerNum;
                pc.teamNumber = teamNum;
                break;
        }
    }
}
