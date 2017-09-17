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
		isPlayerDead = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		playerController = other.gameObject.GetComponent<PlayerController> ();
		Debug.Log (playerController.playerNumber);
		playerNumber = playerController.playerNumber;
		Destroy (other.gameObject);
		StartCoroutine (respawnPlayer (playerNumber));
	}

	private IEnumerator respawnPlayer(int playerNum)
	{
		yield return new WaitForSeconds (2f);
		playerController = player.GetComponent<PlayerController> ();
		playerController.playerNumber = playerNum;
		Instantiate(player, respawnPoint.position, respawnPoint.rotation);

	}
}
