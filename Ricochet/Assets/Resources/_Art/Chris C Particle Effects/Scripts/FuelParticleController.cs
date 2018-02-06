using UnityEngine;

namespace CCParticles
{
    public class FuelParticleController : MonoBehaviour
    {
        #region Inspector Variables

        [SerializeField]
        private ParticleSystem lowFuelParticle;
        
        private PlayerDashController dashController;

        #endregion

        #region Hidden Variables

        private float threshold;

        #endregion

        #region Monobehaviors

        public void Awake()
        {
            dashController = GetComponent<PlayerDashController>();
        }

        void Update()
        {
            int dashes = this.dashController.GetDashCount();
            if (dashes <= 0)
            {
                if (!this.lowFuelParticle.isPlaying)
                {
                    this.lowFuelParticle.Play();
                }
            }else
            {
                this.lowFuelParticle.Stop();
            }
        }
        #endregion
    }
}
