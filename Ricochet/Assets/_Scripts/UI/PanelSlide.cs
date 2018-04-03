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

    private void Start()
    {
        _from = transform;
    }

    public void ExecuteMoveTo(float duration)
    {
        transform.DOMove(_to.position, duration);
    }

    public void ExecuteMoveBack(float duration)
    {
        transform.DOMove(_from.position, duration);
    }
}
