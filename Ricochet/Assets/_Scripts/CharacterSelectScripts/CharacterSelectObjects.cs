using System.Collections.Generic;
using Enumerables;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectObjects : MonoBehaviour
{
    #region Serialized
    [SerializeField]
    [Tooltip("List of all Player 1 character tokens")]
    private List<GameObject> _tokens;
    [SerializeField]
    [Tooltip("Drag default active token for Player 1 here")]
    private GameObject _defaultToken;
    [SerializeField]
    [Tooltip("Drag player 1 ready tag here.")]
    private GameObject _readyTag;
    #endregion

    #region Private
    private GameObject _activeToken;
    private ETeam _team;
    #endregion

    #region Public Getters and Setters
    public List<GameObject> Tokens
    {
        get { return _tokens; }
    }
    public GameObject DefaultToken
    {
        get { return _defaultToken; }
        set { _defaultToken = value; }
    }
    public GameObject ReadyTag
    {
        get { return _readyTag; }
    }
    public GameObject ActiveToken
    {
        get { return _activeToken; }
        set { _activeToken = value; }
    }
    public ETeam Team
    {
        get { return _team; }
        set { _team = value; }
    }
    #endregion

    public GameObject SwitchTokensToNext(GameObject activeToken, int direction)
    {
        activeToken.SetActive(false);
        var currentIndex = _tokens.FindIndex(t => (t == activeToken));
        if (currentIndex == 0)
        {
            if (direction > 0)
            {
                activeToken = _tokens[currentIndex + direction];
            }
            else
            {
                activeToken = _tokens[_tokens.Count - 1];
            }
        }
        else if (currentIndex == _tokens.Count - 1)
        {
            if (direction < 0)
            {
                activeToken = _tokens[currentIndex + direction];
            }
            else
            {
                activeToken = _tokens[0];
            }
        }
        else
        {
            activeToken = _tokens[currentIndex + direction];
        }
        activeToken.SetActive(true);
        return activeToken;
    }
}
