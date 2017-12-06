using System.Collections;
using UnityEngine;

namespace CCParticles
{
    public class TriggerCollisionParticle : ParticleTrigger
    {
        #region Inspector Variables

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
            if (collisionTypeEnabled)
            {
                if (((1 << collider.gameObject.layer) & this.collisionLayer) != 0)
                {
                    base.PlaySystem(true);
                }
            }
        }
        #endregion
    }
}
