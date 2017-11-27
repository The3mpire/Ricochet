using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CharSelect_PlayerController : MonoBehaviour
{
    #region Private
    private Player player;
    private CharacterController cc;

    [SerializeField]
    [Tooltip("Set the player number")]
    private int playerNumber;

    //[SerializeField]
    //[Tooltip("Drag character selection cursor here.")]
    //private GameObject cursor;

    //[SerializeField]
    //[Tooltip("Speed that the cursor moves")]
    //private int cursorSpeed;

    //[SerializeField]
    //[Tooltip("Drag UI Canvas here")]
    //private GameObject uiCanvas;

    [Tooltip("The button that is first highlighted")]
    [SerializeField]
    private GameObject defaultSelectedCharacter;

    #endregion
    // Use this for initialization
    void Awake()
    {
        player = ReInput.players.GetPlayer(playerNumber - 1);
        EventSystem.current.SetSelectedGameObject(defaultSelectedCharacter);
    }

    void OnEnable()
    {
        //cc = cursor.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        //Vector2 moveVector = new Vector2(player.GetAxis("MoveHorizontal"), player.GetAxis("MoveVertical")); // get input by name or action id
        //if (moveVector.x != 0.0f || moveVector.y != 0.0f)
        //{
        //    cc.Move(moveVector * cursorSpeed * Time.deltaTime);
        //}
        if (player.GetButtonDown("A Button"))
        {
            //var selectedCharacter = GetCursorTarget(cursor);
            var selectedCharacter = EventSystem.current.currentSelectedGameObject;
            var characterSelected = CheckSelection();
        }
    }

    //private Enumerables.ECharacter GetCursorTarget(GameObject curs)
    //{
    //    //Code to be place in a MonoBehaviour with a GraphicRaycaster component
    //    GraphicRaycaster gr = uiCanvas.GetComponent<GraphicRaycaster>();
    //    //Create the PointerEventData with null for the EventSystem
    //    PointerEventData ped = new PointerEventData(null);
    //    //Set required parameters, in this case, mouse position
    //    ped.position = cursor.transform.position;
    //    //Create list to receive all results
    //    List<RaycastResult> results = new List<RaycastResult>();
    //    //Raycast it
    //    gr.Raycast(ped, results);
    //    Debug.Log(results[0].gameObject.name);
    //    return Enumerables.ECharacter.None;
    //}

    private bool CheckSelection()
    {
        return false;
    }
}
