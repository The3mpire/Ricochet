using System.Collections;
using UnityEngine;
using Enumerables;

public class Neon : MonoBehaviour {

    #region Serialized Fields

    [Header("Neon Options")]
    [SerializeField] private ETeam team;
    [SerializeField] private float pulseFrequency;

    [Space]

    [Header("Line Renderer Options")]
    [SerializeField] private Vector3[] nodes;
    [SerializeField] private float width;
    [SerializeField] private int cornerVertices;
    [SerializeField] private int endCapVertices;
    [SerializeField] private bool loop;
    [SerializeField] private float emission;

    [Space]

    [Header("Renderer References")]
    [SerializeField] private LineRenderer innerBand;
    [SerializeField] private LineRenderer outerBand;
    [SerializeField] private LineRenderer auraBand;
    [SerializeField] private NeonFlash neonFlash;
    #endregion

    #region Private Fields
    private int emissionID;
    private Material innerMaterial;
    private Color innerColor;
    private Material outerMaterial;
    private Color outerColor;

    private IEnumerator tcf;
    private bool linearPulse;
    private float ogF;
    private float x;
    #endregion

    #region Monobehaviour

    void Start()
    {
        emissionID = Shader.PropertyToID("_EmissionColor");
        innerMaterial = innerBand.material;
        innerColor = innerMaterial.GetColor(emissionID);
        outerMaterial = outerBand.material;
        outerColor = outerMaterial.GetColor(emissionID);
        linearPulse = false;
        ogF = pulseFrequency;

        UpdateLineRenderers();
    }

    void Update()
    {
        EmissionPulse();
    }

    #endregion

    #region Helpers

    public void ReInitialize()
    {
        emissionID = Shader.PropertyToID("_EmissionColor");
        innerMaterial = innerBand.material;
        innerColor = innerMaterial.GetColor(emissionID);
        outerMaterial = outerBand.material;
        outerColor = outerMaterial.GetColor(emissionID);
        linearPulse = false;
        ogF = pulseFrequency;

        UpdateLineRenderers();
    }

    public void UpdateLineRenderers()
    {
        innerBand.numCornerVertices = cornerVertices;
        innerBand.numCapVertices = endCapVertices;
        innerBand.positionCount = nodes.Length;
        innerBand.SetPositions(nodes);
        innerBand.startWidth = width;
        innerBand.endWidth = width;
        innerBand.loop = loop;

        outerBand.numCornerVertices = cornerVertices;
        outerBand.numCapVertices = endCapVertices;
        outerBand.positionCount = nodes.Length;
        outerBand.SetPositions(nodes);
        outerBand.startWidth = width;
        outerBand.endWidth = width;
        outerBand.loop = loop;

        auraBand.numCornerVertices = cornerVertices;
        auraBand.numCapVertices = endCapVertices;
        auraBand.positionCount = nodes.Length;
        auraBand.SetPositions(nodes);
        auraBand.startWidth = width;
        auraBand.endWidth = width;
        auraBand.loop = loop;

        neonFlash.SetNodes(nodes);
        neonFlash.SetWidth(width);
    }

    public void Flash()
    {
        NeonFlash nf = Instantiate(neonFlash);
        nf.gameObject.SetActive(true);
        nf.Initialize();
        nf.HitTheLights();

        //StopCoroutine("TempChangeFrequency");
        //IEnumerator coroutine = TempChangeFrequency(9f, 2.5f);
        if (tcf != null)
            StopCoroutine(tcf);
        tcf = TempChangeFrequency(9f, 2.5f);
        StartCoroutine(tcf);
        StartCoroutine("AuraPulse");
    }

    private void EmissionPulse()
    {
        float emissionMagnitude;
        float emissionWidth;

        if (!linearPulse)
        {
            float y = -(Mathf.Sin(x / 2)) - (Mathf.Cos(x) / 4) + 0.75f;
            emissionMagnitude = 0.9f + y * 2;
            emissionWidth = 1 + y / 2;
        }
        else
        {
            float y = (Mathf.Sin(x) + 1) * 0.5f;
            emissionMagnitude = 0.9f + y * 2;
            emissionWidth = 1 + y / 2;
        }

        x += Time.deltaTime * pulseFrequency;
        if (x >= 2 * Mathf.PI)
            x = 0;

        Color emissionColor = innerColor * Mathf.LinearToGammaSpace(emissionMagnitude);
        innerMaterial.SetColor(emissionID, emissionColor);
        innerBand.widthMultiplier = emissionWidth;

        emissionColor = outerColor * Mathf.LinearToGammaSpace(emissionMagnitude);
        outerMaterial.SetColor(emissionID, emissionColor);
        outerBand.widthMultiplier = emissionWidth;
    }

    private IEnumerator TempChangeFrequency(float frequency, float duration)
    {
        pulseFrequency = frequency;
        linearPulse = true;
        yield return new WaitForSeconds(duration);

        while (!Approximately(x, (1.5f * Mathf.PI), 0.1f))
            yield return new WaitForEndOfFrame();

        x = Mathf.PI;
        linearPulse = false;
        pulseFrequency = ogF;
        tcf = null;
    }

    private IEnumerator AuraPulse()
    {
        LineRenderer aura = Instantiate(auraBand);
        aura.widthMultiplier = 0.1f;
        aura.gameObject.SetActive(true);
        float w = 0f;
        while (w < 3f)
        {
            w += Time.fixedDeltaTime;
            float y = (Mathf.Log(w) + 4f) * 0.5f;
            aura.widthMultiplier = y * 2f;
            aura.startColor = new Color(aura.startColor.r, aura.startColor.g, aura.startColor.b, 2.3f - y);
            aura.endColor = new Color(aura.startColor.r, aura.startColor.g, aura.startColor.b, 2.3f - y);
            yield return new WaitForEndOfFrame();
        }
        Destroy(aura.gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        for (int i = 0; i <= nodes.Length - 2; i++)
            Gizmos.DrawLine(nodes[i], nodes[i + 1]);
    }

    bool Approximately(float a, float b, float error)
    {
        return (Mathf.Abs(a - b) <= error);
    }

    #endregion

    #region Getters & Setters

    public ETeam GetTeam()
    {
        return team;
    }

    public void SetOuterBandMaterial(Material mat)
    {
        outerBand.material = mat;
    }

    public void SetInnerBandMaterial(Material mat)
    {
        innerBand.material = mat;
    }

    public void SetAuraBandGradient(Gradient grad)
    {
        auraBand.colorGradient = grad;
    }

    public void SetFlashGradient(Gradient grad)
    {
        neonFlash.SetTrailGradient(grad);
    }

    #endregion
}
