using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class _UICharacterDisplay : MonoBehaviour {
	#region Inspector Variables
	[Header("Player Sprites")]
    [Tooltip("No selection sprite (should be enabled)")]
    [SerializeField]
    private Image noSelect;
    [Tooltip("Red Bean Sprite")]
	[SerializeField]
	private Image redBean;
	[Tooltip("Blue Bean Sprite")]
	[SerializeField]
	private Image blueBean;
	[Tooltip("Red Eggplant Sprite")]
	[SerializeField]
	private Image redEggplant;
	[Tooltip("Blue Eggplant Sprite")]
	[SerializeField]
	private Image blueEggplant;

	[Header("Cursor Sprites")]
	[Tooltip("Bean Cursor")]
	[SerializeField]
	private Image beanCursor;
	[Tooltip("Eggplant Cursor")]
	[SerializeField]
	private Image eggplantCursor;
    #endregion

    #region UI Behaviour
    public void UpdateCharacter(string newCharacter) {
		
	}
	public void MoveCursor(string selection) {
		
	}
	public void UpdateTeam(int team) {
		
	}
	public void SetReady(bool state) {
		
	}
    #endregion
}
