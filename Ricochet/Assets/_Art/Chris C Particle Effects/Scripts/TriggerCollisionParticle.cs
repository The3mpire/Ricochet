using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CCParticles
{
    public class TriggerCollisionParticle : MonoBehaviour
    {
        #region Inspector Variables

        [SerializeField]
        private ParticleSystem particleSystem;

        [SerializeField]
        private LayerMask collisionLayer;

        #endregion

        private void OnCollisionEnter2D(Collision2D collision)
        {
            CheckAndPlay(collision.collider);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            CheckAndPlay(collision);
        }

        private void OnDisable()
        {
            this.particleSystem.gameObject.SetActive(false);
        }

        private void CheckAndPlay(Collider2D collision)
        {
            if (this.particleSystem != null)
            {
                if (((1 << collision.gameObject.layer) & this.collisionLayer) != 0)
                {
                    StartCoroutine(PlayParticleSystem(this.particleSystem));
                }
            }
        }

        IEnumerator PlayParticleSystem(ParticleSystem system)
        {
            particleSystem.gameObject.SetActive(true);
            particleSystem.Play();
            while (particleSystem.isPlaying)
            {
                yield return null;
            }
            particleSystem.gameObject.SetActive(false);
        }

    }
}
