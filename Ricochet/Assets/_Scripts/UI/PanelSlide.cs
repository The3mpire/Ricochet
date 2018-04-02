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
        Debug.Log("Panel sliding in");
        transform.DOMove(_to.position, duration);
    }

    public void ExecuteMoveBack(float duration)
    {
        Debug.Log("Panel sliding out");
        transform.DOMove(_from.position, duration);
    }
}
