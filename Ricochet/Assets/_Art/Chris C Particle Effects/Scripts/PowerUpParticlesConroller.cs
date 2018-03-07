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

        private void OnTriggerEnter2D(Collider2D collision)
        {
            PowerUp power = collision.gameObject.GetComponent<PowerUp>();
            if (power)
            {
                EPowerUp type = power.GetPowerUpType();
                PlayPowerupEffect(type);
            }
        }

        public void PlayPowerupEffect(EPowerUp powerup, int version = 0)
        {
            switch (powerup)
            {
                case EPowerUp.Multiball:
                    PlayEffect(this.systems.multiball);
                    break;
                case EPowerUp.CatchNThrow:
                    PlayEffect(this.systems.catchNThrow);
                    break;
                case EPowerUp.Freeze:
                    PlayFreezeEffect(version);
                    break;
                case EPowerUp.CircleShield:
                    PlayEffect(this.systems.shield);
                    break;
            }
        }

        private void PlayFreezeEffect(int version)
        {
            switch (version)
            {
                case 1:
                    PlayEffect(this.systems.freezeCube);
                    break;
                default:
                    PlayEffect(this.systems.freezeOrb);
                    break;
            }
        }

        private IEnumerable PlayEffect(GameObject system)
        {
            system.SetActive(true);
            yield return null;
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
