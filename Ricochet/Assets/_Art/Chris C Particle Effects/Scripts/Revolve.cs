using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CCParticles
{
    [ExecuteInEditMode]
    public class Revolve : MonoBehaviour
    {

        [SerializeField]
        private Transform center;

        [SerializeField]
        private float speed;

        [SerializeField]
        private Vector3 axis;

        [SerializeField]
        private Vector3 startPosition;


        private void Start()
        {
            gameObject.transform.position = center.position + startPosition;
        }

        // Update is called once per frame
        void Update()
        {
            gameObject.transform.RotateAround(this.center.position, this.axis, Time.deltaTime * speed);
        }
    }

}