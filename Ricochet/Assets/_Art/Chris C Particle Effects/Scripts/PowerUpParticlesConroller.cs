using Enumerables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CCParticles
{
    [RequireComponent(typeof(Collider2D))]
    public class PowerUpParticlesConroller : MonoBehaviour
    {
        [SerializeField]
        private PowerupParticleSystems systems;

        /*
        private void OnTriggerEnter2D(Collider2D collision)
        {
            PowerUp power = collision.gameObject.GetComponent<PowerUp>();
            if (power)
            {
                EPowerUp type = power.GetPowerUpType();
                PlayPowerupEffect(type);
            }
        }
        */

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

        private void PlayFreezeEffect(int version, bool shouldPlay)
        {
            switch (version)
            {
                case 1:
                    PlayEffect(this.systems.freezeCube, shouldPlay);
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

        [System.Serializable]
        struct PowerupParticleSystems
        {
            public GameObject multiball;
            public GameObject catchNThrow;
            public GameObject freezeOrb;
            public GameObject freezeCube;
            public GameObject shield;
        }
    }
}
