using System;
using System.Collections;
using UnityEngine;

namespace CCShaders
{
    [DisallowMultipleComponent]
    public sealed class ChromaticAberrationEffect : CameraShaderEffect
    {
        #region Inspector Variables

        [SerializeField]
        [Tooltip("Direction of the aberration")]
        private Vector4 direction = new Vector4(1, 0, 0, 0);

        [SerializeField]
        [Tooltip("Channel Intensity Modifier")]
        private Vector4 channelIntensity = new Vector4(1, 1, 1, 1);

        [SerializeField]
        [Range(0, 1)]
        [Tooltip("The intensity of the aberration (Distance of RGB channel split)")]
        private float intensity = .025f;

        [SerializeField]
        [Range(1, 10)]
        [Tooltip("How many times to cycle through the effect")]
        private int cycles = 1;

        [SerializeField]
        [Range(0, 1)]
        [Tooltip("Progress through the entire effect")]
        private float progress = 0f;

        [SerializeField]
        [Range(0, 1)]
        [Tooltip("Direction of the aberration")]
        private float defaultValue = 0.0025f;

        #endregion

        #region Properties

        public int Cycles { get { return this.cycles; } set { this.cycles = value; } }

        public float Progress { get { return this.progress; } set { this.progress = value; } }

        #endregion

        #region Coroutines

        public IEnumerator PlayEffect(float duration)
        {
            Material mat = this.EffectMaterial;
            float currentTime = Time.time;
            float endTime = Time.time + duration;
            while (currentTime <= endTime)
            {
                try
                {
                    float progress = currentTime / endTime;
                    mat.SetFloat("_Progress", progress);
                    currentTime = Time.time;
                }
                catch (NullReferenceException)
                {

                }
                yield return new WaitForEndOfFrame();
            }
            mat.SetFloat("_Progress", 0);
            

        }

        #endregion

        #region Private Helpers

        protected override void SetEffectValues(Material effectMaterial)
        {
            if (effectMaterial)
            {
                effectMaterial.SetFloat("_Intensity", this.intensity);
                effectMaterial.SetFloat("_Cycles", this.cycles);
                effectMaterial.SetFloat("_Progress", this.progress);
                effectMaterial.SetVector("_Direction", this.direction);
                effectMaterial.SetVector("_ChannelIntensity", this.channelIntensity);
                effectMaterial.SetFloat("_DefaultValue", this.defaultValue);
            }
        }

        #endregion
    }
}
