using UnityEngine;

namespace CCShaders
{
    [DisallowMultipleComponent]
    public sealed class ChromaticAberrationEffect : CameraShaderEffect
    {

        #region Inspector Variables

        [SerializeField]
        [Range(0, 1)]
        [Tooltip("The intensity of the aberration (Distance of RGB channel split)")]
        private float intensity = .025f;

        [SerializeField]
        [Range(0, 100)]
        [Tooltip("Frequency of the aberration effect")]
        private float frequency = 10f;

        [SerializeField]
        [Range(-5, 5)]
        [Tooltip("Evolution speed of the effect")]
        private float travelRate = 0f;

        [SerializeField]
        [Range(1, 100)]
        [Tooltip("Frequency of the masking wave for the effect")]
        private float maskFrequency = 10f;

        [SerializeField]
        [Range(-5, 5)]
        [Tooltip("Evolution speed of the masking frequency")]
        private float maskTravelRate = 2f;

        [SerializeField]
        [Range(1, 20)]
        [Tooltip("How quickly the effect should simulate")]
        private float simulationSpeed = 1f;

        #endregion

        #region Private Helpers

        protected override void SetEffectValues(Material effectMaterial)
        {
            if (effectMaterial)
            {
                effectMaterial.SetFloat("_Intensity", this.intensity);
                effectMaterial.SetFloat("_Frequency", this.frequency);
                effectMaterial.SetFloat("_TravelRate", this.travelRate);
                effectMaterial.SetFloat("_MaskFrequency", this.maskFrequency);
                effectMaterial.SetFloat("_MaskTravelMultiplier", this.maskTravelRate);
                effectMaterial.SetFloat("_SimulationSpeed", this.simulationSpeed);
            }
        }

        #endregion
    }
}
