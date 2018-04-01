using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CatMainMenuFlyby : MonoBehaviour
{
    [SerializeField]
    private Transform _startPoint;
    [SerializeField]
    private Transform _endPoint;
    [SerializeField]
    private float _duration = 10.0f;

    private Tween _moving;

	// Use this for initialization
	void Start ()
	{
	    transform.position = _startPoint.position;
	    _moving = transform.DOMove(_endPoint.position, _duration, true);
    }
	
	// Update is called once per frame
	void Update ()
	{
	    if (transform.position.x >= _endPoint.position.x)
	    {
	        _moving.Restart(true, 3f);
	    }
	}
}
