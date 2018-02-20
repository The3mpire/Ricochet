using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PulseSprite : MonoBehaviour {

    [SerializeField] private float growSize = 1.05f;
    [SerializeField] private float shrinkSize = 0.95f;
    [SerializeField] private float duration = 0.5f;

    // Use this for initialization
    void Start () {
        Grow();
	}

    void Grow ()
    {
        transform.DOScale(growSize, duration).OnComplete(()=>Shrink());
    }

    void Shrink()
    {
        transform.DOScale(shrinkSize, duration).OnComplete(()=>Grow());
    }
}
