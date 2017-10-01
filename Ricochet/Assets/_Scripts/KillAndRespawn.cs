﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillAndRespawn : MonoBehaviour
{
    public Transform respawnPoint1;
    public Transform respawnPoint2;

    public ScoreManager ScoreUI;

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
            StartCoroutine(respawnPlayer(pc));
        }
    }

    private IEnumerator respawnPlayer(PlayerController player)
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
