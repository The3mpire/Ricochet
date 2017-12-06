using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CCParticles
{
    [ExecuteInEditMode]
    public class ParticleTrigger: MonoBehaviour
    {
        #region Inspector Variables

        [SerializeField]
        [Tooltip("The particle system that will be triggered to play")]
        private ParticleSystem system;

        [SerializeField]
        [Tooltip("Whether or not this particle should start playing on awake")]
        private bool playOnAwake;

        #endregion

        #region Monobehaviors

        private void OnValidate()
        {
            ParticleSystem.MainModule m = this.system.main;
            m.playOnAwake = this.playOnAwake;
        }

        #endregion

        #region External Functions

        public void PlaySystem(bool play)
        {
            if (this.system.gameObject.activeInHierarchy)
            {
                if (play)
                {
                    this.system.Play();
                }
                else
                {
                    this.system.Stop();
                }
            }
        }

        public bool IsPlaying()
        {
            return this.system.isPlaying;
        }

        #endregion

    }
}
