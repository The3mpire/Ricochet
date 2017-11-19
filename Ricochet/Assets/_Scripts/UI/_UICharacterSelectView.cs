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
	[Tooltip("Red Carrot Sprite")]
	[SerializeField]
	private GameObject redCarrot;
	[Tooltip("Blue Carrot Sprite")]
	[SerializeField]
	private GameObject blueCarrot;

	[Header("Cursor Sprites")]
	[Tooltip("Bean Cursor")]
	[SerializeField]
	private GameObject beanCursor;
	[Tooltip("Carrot Cursor")]
	[SerializeField]
	private GameObject carrotCursor;
    #endregion

    #region Private Variables
    private bool active = false;
    private int currentTeam = 1;
	private string currentSpriteString;
    private GameObject currentSprite;
    private GameObject currentCursor;
    #endregion

    #region MonoBenavior
    private void Awake()
    {
		currentSpriteString = "bean";
		currentSprite = noSelect;
		currentCursor = beanCursor;
    }
    #endregion

    #region UI Behaviour
    public void UpdateCharacter(string newCharacter)
    {
		currentSpriteString = newCharacter;

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
        else if (newCharacter == "carrot")
        {
            if (currentTeam == 1)
            {
                UpdateCharacterImage(redCarrot);
            }
            else if (currentTeam == 2)
            {
                UpdateCharacterImage(blueCarrot);
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
        else if (selection == "carrot")
        {
            UpdateCursorImage(carrotCursor);
        }
        else
        {
            Debug.LogError("Unrecognized cursor selected: " + selection, gameObject);
        }
    }

	public void SetReady(bool state)
    {
        if (state)
        {
            Debug.Log("You are now ready!");
        }
        else
        {
            Debug.Log("No longer ready");
        }
	}

	public void UpdateTeam(int playerTeam)
	{
		currentTeam = playerTeam;
		UpdateCharacter (currentSpriteString);
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
