using UnityEngine;

[ExecuteInEditMode]
public class PaletteSwapShader : MonoBehaviour {

    #region Inspector Variables

    [SerializeField]
    private Color[] colors;

    #endregion

    #region Private Variables

    private Texture2D swapTex;
    private Material paletteMat;

    #endregion

    #region Monobehaviors

    private void Awake()
    {
        this.paletteMat = this.GetComponent<SpriteRenderer>().sharedMaterial;
    }

    private void Start()
    {
        ApplyTexture();
    }

    private void OnValidate()
    {
        ApplyTexture();
    }

    #endregion

    #region Private Methods

    void ApplyTexture () {
        this.swapTex = new Texture2D(256, 1, TextureFormat.RGBA32, false, false);
        this.swapTex.filterMode = FilterMode.Point;

        for(int x = 0; x < swapTex.width; x++)
        {
            this.swapTex.SetPixel(x, 0, new Color(0, 0, 0, 0));
        }
        swapTex.Apply();

        for (int x = 0; x < colors.Length; x++)
        {
            this.swapTex.SetPixel(x, 0, colors[x]);
        }

        swapTex.Apply();

        this.paletteMat.SetFloat("_SwapSize", (float)this.colors.Length);
        this.paletteMat.SetTexture("_SwapTex", this.swapTex);

        Debug.Log("Finished generating texture of size: " + this.swapTex.width);
	}

    #endregion
}
