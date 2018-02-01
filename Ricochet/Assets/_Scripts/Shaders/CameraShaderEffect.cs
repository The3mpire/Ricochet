using UnityEngine;

namespace CCShaders
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public abstract class CameraShaderEffect : MonoBehaviour
    {

        #region Inspector Variables

        [SerializeField]
        private Shader effectShader;

        #endregion

        #region Hidden Variables

        private Material effectMaterial;

        #endregion

        #region Monobehaviors

        private void OnEnable()
        {
            this.SetupEffect();
        }

        private void OnValidate()
        {
            this.SetupEffect();
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (this.effectMaterial)
            {
                Graphics.Blit(source, destination, this.effectMaterial);
            }
            else
            {
                Debug.LogError(string.Format("Material for {0} is null", this));
            }
        }

        #endregion

        #region Private Helpers

        private void SetupEffect()
        {
            if (this.effectShader)
            {
                if (!this.effectMaterial)
                {
                    this.effectMaterial = new Material(this.effectShader);
                    this.effectMaterial.hideFlags = HideFlags.DontSave;
                }
                if (this.effectMaterial)
                {
                    SetEffectValues(this.effectMaterial);
                }
            }
        }

        #endregion

        #region Abstract Methods

        protected abstract void SetEffectValues(Material effectMaterial);

        #endregion
    }
}
