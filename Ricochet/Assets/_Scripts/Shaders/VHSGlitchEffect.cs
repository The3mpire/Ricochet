using UnityEngine;

namespace CCShaders
{
    [DisallowMultipleComponent]
    public class VHSGlitchEffect : CameraShaderEffect
    {
        #region Inspector Variables

        [SerializeField]
        private Color bleedColor = new Color(0, 0, 0, 0);

        [SerializeField]
        private float inverseBleedIntensity = 15f;

        [SerializeField]
        MovieTexture vhsClip;

        #endregion

        #region Monobehaviors

        void Start()
        {
            vhsClip.loop = true;
            vhsClip.Play();
        }

        #endregion

        #region Override Functions

        protected override void SetEffectValues(Material effectMaterial)
        {
            if (effectMaterial)
            {
                effectMaterial.SetTexture("_VHSTex", vhsClip);
                effectMaterial.SetVector("_BleedColor", bleedColor);
                effectMaterial.SetFloat("_BleedIntensity", inverseBleedIntensity);
            }
        }

        #endregion
    }

}