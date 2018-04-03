using System;
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
                case EPowerUp.Shrink:
                    PlayEffect(this.systems.shrink, play);
                    break;
            }
        }

        #endregion

        #region Private Methods

        private void PlayEffect(ParticleSystem shrink, bool play)
        {
            shrink.Stop();
            if (!play)
            {
                return;
            }
            shrink.Play();
        }

        private void PlayFreezeEffect(int version, bool shouldPlay)
        {
            switch (version)
            {
                case 1:
                    //this.systems.freezeCube.Stop();
                    this.systems.freezeCube.transform.parent.gameObject.SetActive(shouldPlay);
                    if (!shouldPlay)
                    {
                        return;
                    }
                    this.systems.freezeCube.Play();
                    break;
                default:
                    this.systems.freezeOrb.SetActive(shouldPlay);
                    break;
            }
        }

        #endregion

        #region Structs

        [System.Serializable]
        struct PowerupParticleSystems
        {
            public ParticleSystem multiball;
            public ParticleSystem catchNThrow;
            public GameObject freezeOrb;
            public ParticleSystem freezeCube;
            public ParticleSystem shield;
            public ParticleSystem shrink;
        }

        #endregion
    }
}
