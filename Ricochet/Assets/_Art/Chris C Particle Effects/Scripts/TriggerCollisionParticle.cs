using System.Collections;
using UnityEngine;

namespace CCParticles
{
    public class TriggerCollisionParticle : MonoBehaviour
    {
        #region Inspector Variables

        [SerializeField]
        [Tooltip("The particle system that will be played when a collision is detected")]
        private ParticleSystem system;

        [SerializeField]
        [Tooltip("The layer that this can collide with to trigger the particle")]
        private LayerMask collisionLayer;

        [SerializeField]
        [Tooltip("Enables OnCollisionEnter detection")]
        private bool enableStandardCollision = true;

        [SerializeField]
        [Tooltip("Enables OnTriggerEnter detection")]
        private bool enableTriggerColliision = true;

        #endregion

        #region Monobehaviour

        private void OnEnable()
        {
            if (this.system)
            {
                this.system.gameObject.SetActive(false);
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            CheckAndPlay(collision.collider, this.enableStandardCollision);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            CheckAndPlay(collision, this.enableTriggerColliision);
        }

        #endregion

        #region Private Helpers

        private void CheckAndPlay(Collider2D collider, bool collisionTypeEnabled)
        {
            if (this.system != null && collisionTypeEnabled && gameObject.activeSelf)
            {
                if (((1 << collider.gameObject.layer) & this.collisionLayer) != 0)
                {
                    StartCoroutine(PlayParticleSystem(this.system));
                }
            }
        }

        private IEnumerator PlayParticleSystem(ParticleSystem system)
        {
            this.system.gameObject.SetActive(true);
            this.system.Play();
            while (this.system.isPlaying)
            {
                yield return null;
            }
            this.system.gameObject.SetActive(false);
        }

        #endregion
    }
}
