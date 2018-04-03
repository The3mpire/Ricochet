using System.Runtime.CompilerServices;
using DG.Tweening;
using UnityEngine;

public class PanelSlide : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Position to move from.")]
    private Transform _from;
    [SerializeField]
    [Tooltip("Position to move to.")]
    private Transform _to;

    public Tween ExecuteMoveTo(float duration)
    {
        return transform.DOMove(_to.position, duration, true);
    }

    public Tween ExecuteMoveBack(float duration)
    {
        return transform.DOMove(_from.position, duration, true);
    }
}
