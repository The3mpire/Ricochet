using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region MonoBehaviour
public class _UICharacterDisplay : MonoBehaviour {
	#region Inspector Variables
	[Header("Player Sprites")]
	[Tooltip("Red Bean Sprite")]
	[SerializeField]
	private SpriteRenderer redBean1;
	[Tooltip("Blue Bean Sprite")]
	[SerializeField]
	private SpriteRenderer blueBean1;
	[Tooltip("Red Eggplant Sprite")]
	[SerializeField]
	private SpriteRenderer redEggplant1;
	[Tooltip("Blue EggPlant Sprite")]
	[SerializeField]
	private SpriteRenderer blueEggplant1;

	[Header("Cursor Sprites")]
	[Tooltip("Bean Cursor")]
	[SerializeField]
	private SpriteRenderer beanCursor;
	[Tooltip("Eggbplant Cursor")]
	[SerializeField]
	private SpriteRenderer eggplantCursor;
	#region

	public void UpdateCharacter(string newCharacter) {
		
	}
	public void MoveCursor(string selection) {
		
	}
	public void UpdateTeam(int team) {
		
	}
	public void SetReady(bool state) {
		
	}
}
#endregion