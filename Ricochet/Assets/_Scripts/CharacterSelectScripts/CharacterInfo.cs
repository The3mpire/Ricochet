using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInfo : MonoBehaviour {
    [SerializeField]
    [Tooltip("Enumerator value of this character")]
    private Enumerables.ECharacter characterId;

    [SerializeField]
    [Tooltip("Image for this character")]
    private Sprite characterImage;

    private bool isSelectable;

    private void Awake()
    {
        isSelectable = true;
    }

    public Enumerables.ECharacter getCharacterId()
    {
        return characterId;
    }

    public Sprite getCharacterImage()
    {
        return characterImage;
    }

    public bool GetIsSelectable()
    {
        return isSelectable;
    }

    public void SetIsSelectable(bool value)
    {
        isSelectable = value;
    }
}
