using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillandRespawnScript : MonoBehaviour {

	private PlayerController playerController;
	private int playerNumber;

	public bool isPlayerDead;
	public Transform respawnPoint;
	public GameObject player;

	// Use this for initialization
	void Start () {
		//isPlayerDead = false;
	}

	// Update is called once per frame
	void Update () {

	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "Player") {

			playerNumber = other.transform.parent.gameObject.GetComponent<PlayerController> ().playerNumber;
			Destroy(other.transform.parent.gameObject);
			StartCoroutine (respawnPlayer (playerNumber));
		}
	}

	private IEnumerator respawnPlayer(int playerNum)
	{
		yield return new WaitForSeconds (2f);
		player.GetComponent<PlayerController> ().playerNumber = playerNum;
		Instantiate (player, respawnPoint.position, respawnPoint.rotation);
	}
}
