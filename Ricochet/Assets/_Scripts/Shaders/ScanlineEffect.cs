using UnityEngine;

namespace CCShaders
{
    public class ScanlineEffect : CameraShaderEffect
    {
        #region Inspector Variables

        [SerializeField]
        [Range(0, 1)]
        private float intensityMultiplier = .025f;

        [SerializeField]
        [Range(0, 10)]
        private float frequency = 3f;

        [SerializeField]
        [Range(1, 50)]
        private float speed = 8;

        #endregion

        #region Override Methods

        protected override void SetEffectValues(Material effectMaterial)
        {
            effectMaterial.SetFloat("_IntensityMultiplier", this.intensityMultiplier);
            effectMaterial.SetFloat("_Frequency", this.frequency);
            effectMaterial.SetFloat("_Speed", this.speed);
        }

        #endregion
    }
}
