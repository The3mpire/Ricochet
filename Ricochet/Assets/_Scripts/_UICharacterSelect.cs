using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _UICharacterSelect : MonoBehaviour {





	private class Player
	{
		int team = 1;
		string character = "bean";
		int[] position = { 0, 0 };
	}

	// Use this for initialization
	void Start () {
		
	}



	
	// Update is called once per frame
	void Update () {
		if (Input.anyKeyDown) 
		{
			//Debug.Log (Input.ke);
		}
		
	}
}
