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

    public Enumerables.ECharacter getCharacterId()
    {
        return characterId;
    }

    public Sprite getCharacterImage()
    {
        return characterImage;
    }
}
