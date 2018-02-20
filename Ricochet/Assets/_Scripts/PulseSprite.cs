using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PulseSprite : MonoBehaviour {

    

	// Use this for initialization
	void Start () {
        Grow();
	}

    void Grow ()
    {
        transform.DOScale(1.05f, .5f).OnComplete(()=>Shrink());
    }

    void Shrink()
    {
        transform.DOScale(.95f, .5f).OnComplete(()=>Grow());
    }
}
