using Enumerables;
using UnityEngine;

namespace CCParticles
{
    public class PowerUpParticlesController : MonoBehaviour
    {
        #region Inspector Variables

        [SerializeField]
        private PowerupParticleSystems systems;

        #endregion

        #region Public Methods

        public void StopPowerupEffect(EPowerUp powerup, int version = 0)
        {
            PlayPowerupEffect(powerup, version, false);
        }

        public void PlayPowerupEffect(EPowerUp powerup, int version = 0, bool play = true)
        {
            switch (powerup)
            {
                case EPowerUp.Multiball:
                    PlayEffect(this.systems.multiball, play);
                    break;
                case EPowerUp.CatchNThrow:
                    PlayEffect(this.systems.catchNThrow, play);
                    break;
                case EPowerUp.Freeze:
                    PlayFreezeEffect(version, play);
                    break;
                case EPowerUp.CircleShield:
                    PlayEffect(this.systems.shield, play);
                    break;
            }
        }

        #endregion

        #region Private Methods

        private void PlayFreezeEffect(int version, bool shouldPlay)
        {
            switch (version)
            {
                case 1:
                    this.systems.freezeCube.Stop();
                    this.systems.freezeCube.transform.parent.gameObject.SetActive(shouldPlay);
                    if (!shouldPlay)
                    {
                        return;
                    }
                    this.systems.freezeCube.Play();
                    break;
                default:
                    PlayEffect(this.systems.freezeOrb, shouldPlay);
                    break;
            }
        }

        private void PlayEffect(GameObject system, bool shouldPlay)
        {
            system.SetActive(shouldPlay);
        }

        #endregion

        #region Structs

        [System.Serializable]
        struct PowerupParticleSystems
        {
            public GameObject multiball;
            public GameObject catchNThrow;
            public GameObject freezeOrb;
            public ParticleSystem freezeCube;
            public GameObject shield;
        }

        #endregion
    }
}
