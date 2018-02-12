using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTagManager : MonoBehaviour
{
    [SerializeField] private GameEvent OnShowPlayerTags;
    [SerializeField] private GameEvent OnHidePlayerTags;

    private IList<Player> players;

    public void Start()
    {
        players = ReInput.players.AllPlayers;
    }

    public void Update()
    {
        foreach (Player p in players)
        {
            if (p.GetButtonDown("ShowTags"))
            {
                OnShowPlayerTags.Raise();
            }

            if (p.GetButtonUp("ShowTags"))
            {
                bool hideTags = true;

                foreach (Player p2 in players)
                {
                    if (p2.GetButton("ShowTags"))
                    {
                        hideTags = false;
                    }
                }
                
                if (hideTags)
                {
                    OnHidePlayerTags.Raise();
                }
            }
        }
    }
}
