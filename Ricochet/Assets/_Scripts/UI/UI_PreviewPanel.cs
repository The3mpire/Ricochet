using System.Collections;
using System.Collections.Generic;
using Enumerables;
using UnityEngine;
using UnityEngine.UI;

public class UI_PreviewPanel : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Drag Preview Panel Image component")]
    private Image _previewImage;

    [SerializeField] private Sprite diagonalAlleySprite;
    [SerializeField] private Sprite ElevatorSprite;
    [SerializeField] private Sprite OverTheTopSprite;
    [SerializeField] private Sprite TheVoidSprite;

    public void ChangeSprite(BuildIndex level)
    {
        switch (level)
        {
            case BuildIndex.DIAGONAL_ALLEY:
                _previewImage.sprite = diagonalAlleySprite;
                break;
            case BuildIndex.ELEVATOR:
                _previewImage.sprite = ElevatorSprite;
                break;
            case BuildIndex.UP_N_OVER_WIDE:
                _previewImage.sprite = OverTheTopSprite;
                break;
            case BuildIndex.EMPTY_LEVEL:
                _previewImage.sprite = TheVoidSprite;
                break;
        }
    }
}
