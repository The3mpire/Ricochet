using Enumerables;
using System;
using UnityEngine;

public class BasePlayerSetup : MonoBehaviour {

    #region Inspector Variables

    [SerializeField]
    [Tooltip("The sprite animator for the player.")]
    private Animator spriteAnimator;

    [SerializeField]
    [Tooltip("The sprite renderer for the player.")]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    [Tooltip("Animator and sprite setup data for the cop")]
    private CharacterSetup cop;

    [SerializeField]
    [Tooltip("Animator and sprite setup data for the cat")]
    private CharacterSetup cat;

    [SerializeField]
    [Tooltip("Animator and sprite setup data for the computer")]
    private CharacterSetup computer;

    [SerializeField]
    [Tooltip("Animator and sprite setup data for the sushi man")]
    private CharacterSetup sushi;

    #endregion

    #region Private Helpers

    private void SetupCharacter(CharacterSetup setup, int version)
    {
        //if the setup has unimplimented sprites, use cop instead
        if (setup.controllers.Length == 0)
        {
            Debug.LogError("Could not find animation controller for player, defaulting to cop", gameObject);
            setup = this.cop;
        }
        else if (setup.placeholderSprites.Length == 0)
        {
            Debug.LogError("Could not find default sprite(s) for player, defaulting to cop", gameObject);
            setup = this.cop;
        }
        RuntimeAnimatorController[] controllers = setup.controllers;
        Sprite[] sprites = setup.placeholderSprites;
        this.spriteRenderer.flipX = setup.flipSpriteX;
        if(version < 0 || version >= controllers.Length)
        {
            Debug.LogError(string.Format("Sprite Animator version: {0} is out of range for {1}", version, gameObject.name));
        }
        else
        {
            this.spriteAnimator.runtimeAnimatorController = controllers[version];
        }

        if (version >= sprites.Length)
        {
            Debug.LogError(string.Format("Sprite Placeholder version: {0} is out of range for {1}", version, gameObject.name));
        }
        else
        {
            this.spriteRenderer.sprite = sprites[version];
        }
    }

    #endregion

    #region External Functions

    public void SetupCharacter(ECharacter e, int version)
    {
        switch(e)
        {
            case ECharacter.MallCop:
                SetupCharacter(this.cop, version);
                break;
            case ECharacter.Computer:
                SetupCharacter(this.computer, version);
                break;
            case ECharacter.Cat:
                SetupCharacter(this.cat, version);
                break;
            case ECharacter.Sushi:
                SetupCharacter(this.sushi, version);
                break;
        }
    }

    #endregion

    #region Structs

    [Serializable]
    private struct CharacterSetup
    {
        [SerializeField]
        public RuntimeAnimatorController[] controllers;

        [SerializeField]
        public Sprite[] placeholderSprites;

        public bool flipSpriteX;
    }

    #endregion

}
