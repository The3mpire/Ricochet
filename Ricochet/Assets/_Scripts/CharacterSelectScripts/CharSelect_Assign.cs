using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class CharSelect_Assign : MonoBehaviour {

    // Static

    private static CharSelect_Assign instance;

    public static Rewired.Player GetRewiredPlayer(int gamePlayerId)
    {
        if (!Rewired.ReInput.isReady) return null;
        if (instance == null)
        {
            Debug.LogError("Not initialized. Do you have a PressStartToJoinPlayerSelector in your scene?");
            return null;
        }
        for (int i = 0; i < instance.playerMaps.Count; i++)
        {
            if (instance.playerMaps[i].gamePlayerId == gamePlayerId) return ReInput.players.GetPlayer(instance.playerMaps[i].rewiredPlayerId);
        }
        return null;
    }

    // Instance
    [Tooltip("Maximum number of players allowed")]
    [SerializeField]
    private int maxPlayers = 4;

    private List<PlayerMap> playerMaps; // Maps Rewired Player ids to game player ids
    private int gamePlayerIdCounter = 0;

    void Awake()
    {
        playerMaps = new List<PlayerMap>();
        instance = this; // set up the singleton
    }

    void Update()
    {
        
        // Watch for JoinGame action in each Player
        for (int i = 0; i < ReInput.players.playerCount; i++)
        {
            if (ReInput.players.GetPlayer(i).GetButtonDown("JoinGame"))
            {
                AssignNextPlayer(i);
            }
        }
    }

    void AssignNextPlayer(int rewiredPlayerId)
    {
        if (playerMaps.Count >= maxPlayers)
        {
            Debug.LogError("Max player limit already reached!");
            return;
        }

        int gamePlayerId = GetNextGamePlayerId();
        // Add the Rewired Player as the next open game player slot
        playerMaps.Add(new PlayerMap(rewiredPlayerId, gamePlayerId));

        Player rewiredPlayer = ReInput.players.GetPlayer(rewiredPlayerId);

        // Disable the Assignment map category in Player so no more JoinGame Actions return
        rewiredPlayer.controllers.maps.SetMapsEnabled(false, "Assignment");

        // Enable UI control for this Player now that he has joined
        rewiredPlayer.controllers.maps.SetMapsEnabled(true, "Default");

        Debug.Log("Added Rewired Player id " + rewiredPlayerId + " to game player " + gamePlayerId);
    }

    private int GetNextGamePlayerId()
    {
        return gamePlayerIdCounter++;
    }

    // This class is used to map the Rewired Player Id to your game player id
    private class PlayerMap
    {
        public int rewiredPlayerId;
        public int gamePlayerId;

        public PlayerMap(int rewiredPlayerId, int gamePlayerId)
        {
            this.rewiredPlayerId = rewiredPlayerId;
            this.gamePlayerId = gamePlayerId;
        }
    }
}
