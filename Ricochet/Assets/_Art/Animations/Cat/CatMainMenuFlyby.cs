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
    private bool restarting = false;

	// Use this for initialization
	void Start ()
	{
	    transform.position = _startPoint.position;
	    _moving = transform.DOMove(_endPoint.position, _duration, true);
    }
	
	// Update is called once per frame
	void Update ()
	{
	    if (Math.Round(transform.position.x) >= Math.Round(_endPoint.position.x) && !restarting)
	    {
	        restarting = true;
	        _moving.Restart(true, 3f);
	    }
	    else
	    {
	        restarting = false;
	    }
	}
}
