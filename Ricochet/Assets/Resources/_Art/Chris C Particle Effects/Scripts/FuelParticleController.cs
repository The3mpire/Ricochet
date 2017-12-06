using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CCParticles
{
    public class FuelParticleController : MonoBehaviour
    {
        #region Inspector Variables

        [SerializeField]
        private ParticleSystem lowFuelParticle;

        [SerializeField]
        private PlayerController playerController;

        [SerializeField]
        private float threshold;

        #endregion

        #region Monobehaviors
        void Update()
        {
            float percentage = this.playerController.GetCurrentFuel() / this.playerController.GetMaxFuel();
            if (percentage <= threshold)
            {
                if (!this.lowFuelParticle.isPlaying)
                {
                    this.lowFuelParticle.Play();
                }
            } else
            {
                this.lowFuelParticle.Stop();
            }
        }
        #endregion
    }
}
