using System.Collections.Generic;
using Enumerables;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectObjects : MonoBehaviour
{
    [SerializeField]
    [Tooltip("List of all Player 1 character tokens")]
    private List<GameObject> _tokens;
    [SerializeField]
    [Tooltip("Drag default active token for Player 1 here")]
    private GameObject _defaultToken;

    [SerializeField]
    [Tooltip("Drag player 1 Team panel here.")]
    private Image _teamPanel;

    private Image _teamImage;
    private GameObject _readyTag;
    private GameObject _joinIcon;

    public GameObject ActiveToken { get; private set; }
    public ETeam Team { get; private set; }

    private void Awake()
    {
        _readyTag = _teamPanel.GetComponent("ReadyText").gameObject;
        _teamImage = _teamPanel.GetComponent("SelectedCharImage") as Image;
        _joinIcon = _teamPanel.GetComponent("PressAToJoin").gameObject;
    }
}
