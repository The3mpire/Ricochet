using System;
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
        private bool useRotation = false;

        [SerializeField]
        private float radius = 5;

        float angle = 0;

        private Vector3 startPosition;

        private void Start()
        {
            //speed = (2 * Mathf.PI) / 5;
            startPosition = gameObject.transform.position;
        }

        void Update()
        {
            if (this.useRotation)
            {
                gameObject.transform.RotateAround(this.center.position, this.axis, Time.deltaTime * speed);
            }
            else
            {
                angle += speed * Time.deltaTime;
                float x = Mathf.Cos(angle) * radius;
                float y = Mathf.Sin(angle) * radius;
                transform.position = new Vector3(x, transform.position.y, y);
            }
        }

    }

}