using System.Runtime.CompilerServices;
using DG.Tweening;
using UnityEngine;

public class PanelSlide : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Position to move from.")]
    private Vector3 _from;
    [SerializeField]
    [Tooltip("Position to move to.")]
    private Vector3 _to = Vector3.zero;

    private void Start()
    {
        _from = transform.position;
    }

    public void ExecuteMoveTo(float duration)
    {
        Debug.Log("Panel sliding in");
        transform.DOMove(_to, duration);
    }

    public void ExecuteMoveBack(float duration)
    {
        Debug.Log("Panel sliding out");
        transform.DOMove(_from, duration);
    }
}
