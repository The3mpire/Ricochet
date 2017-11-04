using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class _UICharacterSelectView : MonoBehaviour {
	#region Inspector Variables
	[Header("Player Sprites")]
    [Tooltip("No selection sprite (should be enabled)")]
    [SerializeField]
    private GameObject noSelect;
    [Tooltip("Red Bean Sprite")]
	[SerializeField]
	private GameObject redBean;
	[Tooltip("Blue Bean Sprite")]
	[SerializeField]
	private GameObject blueBean;
	[Tooltip("Red Eggplant Sprite")]
	[SerializeField]
	private GameObject redEggplant;
	[Tooltip("Blue Eggplant Sprite")]
	[SerializeField]
	private GameObject blueEggplant;

	[Header("Cursor Sprites")]
	[Tooltip("Bean Cursor")]
	[SerializeField]
	private GameObject beanCursor;
	[Tooltip("Eggplant Cursor")]
	[SerializeField]
	private GameObject eggplantCursor;
    #endregion

    #region Private Variables
    private bool active = false;
    private int currentTeam = 1;
    private GameObject currentSprite;
    private GameObject currentCursor;
    #endregion

    #region MonoBenavior
    private void Awake()
    {
        
    }
    #endregion

    #region UI Behaviour
    public void UpdateCharacter(string newCharacter)
    {
        if (newCharacter == "bean")
        {
            if (currentTeam == 1)
            {
                UpdateCharacterImage(redBean);
            }
            else if (currentTeam == 2)
            {
                UpdateCharacterImage(blueBean);
            }
        }
        else if (newCharacter == "eggplant")
        {
            if (currentTeam == 1)
            {
                UpdateCharacterImage(redEggplant);
            }
            else if (currentTeam == 2)
            {
                UpdateCharacterImage(blueEggplant);
            }
        }
        else
        {
            Debug.LogError("Unrecognized character selected: " + newCharacter, gameObject);
        }
    }

	public void MoveCursor(string selection)
    {
        if (selection == "bean")
        {
            UpdateCursorImage(beanCursor);
        }
        else if (selection == "eggplant")
        {
            UpdateCursorImage(eggplantCursor);
        }
        else
        {
            Debug.LogError("Unrecognized cursor selected: " + selection, gameObject);
        }
    }

	public void SetReady(bool state)
    {
		
	}

    public void Enable()
    {
        noSelect.SetActive(false);

        currentSprite = redBean;
        redBean.SetActive(true);

        currentCursor = beanCursor;
        beanCursor.SetActive(true);
    }
    #endregion

    #region Private Helper Methods
    private void UpdateCharacterImage(GameObject newImage)
    {
        currentSprite.SetActive(false);
        currentSprite = newImage;
        newImage.SetActive(true);
    }
    private void UpdateCursorImage(GameObject newCursor)
    {
        currentCursor.SetActive(false);
        currentCursor = newCursor;
        newCursor.SetActive(true);
    }
    #endregion
}
