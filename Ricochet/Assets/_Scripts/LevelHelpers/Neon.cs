using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Neon : MonoBehaviour {

    #region Serialized Fields
    [Header("Line Renderer Options")]
    [SerializeField] private Vector3[] nodes;
    [SerializeField] private float width;
    [SerializeField] private int cornerVertices;
    [SerializeField] private int endCapVertices;
    [SerializeField] private bool loop;
    [SerializeField] private float emission;

    [Space]

    [Header("Neon Options")]
    [SerializeField] private float pulseFrequency;

    [Space]

    [Header("Renderer References")]
    [SerializeField] private LineRenderer innerBand;
    [SerializeField] private LineRenderer outerBand;
    [SerializeField] private NeonFlash neonFlash;
    #endregion

    #region Private Fields
    private int emissionID;
    private Material innerMaterial;
    private Color innerColor;
    private Material outerMaterial;
    private Color outerColor;

    private float magMult;
    private float widMult;
    #endregion

    #region Monobehaviour

    void Start()
    {
        emissionID = Shader.PropertyToID("_EmissionColor");
        innerMaterial = innerBand.material;
        innerColor = innerMaterial.GetColor(emissionID);
        outerMaterial = outerBand.material;
        outerColor = outerMaterial.GetColor(emissionID);

        magMult = 1.2f / pulseFrequency;
        widMult = 1 / (pulseFrequency / 0.1f);
        UpdateLineRenderers();
    }

    void Update()
    {
        EmissionPulse();
        if (Input.GetKeyDown(KeyCode.Z))
            Flash();
    }

    #endregion

    #region Helpers

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

        neonFlash.SetNodes(nodes);
        neonFlash.SetWidth(width);
    }

    public void EmissionPulse()
    {
        float emissionMagnitude = .8f + Mathf.PingPong(Time.time * magMult, 2.0f - 0.8f);
        float emissionWidth = width + Mathf.PingPong(Time.time * widMult, 0.1f);

        Color emissionColor = innerColor * Mathf.LinearToGammaSpace(emissionMagnitude);
        innerMaterial.SetColor(emissionID, emissionColor);
        innerBand.startWidth = emissionWidth;
        innerBand.endWidth = emissionWidth;

        emissionColor = outerColor * Mathf.LinearToGammaSpace(emissionMagnitude);
        outerMaterial.SetColor(emissionID, emissionColor);
        outerBand.startWidth = emissionWidth;
        outerBand.endWidth = emissionWidth;
    }

    public void Flash()
    {
        NeonFlash nf = Instantiate(neonFlash);
        nf.gameObject.SetActive(true);
        nf.Initialize();
        nf.HitTheLights();

        IEnumerator coroutine = TempChangeFrequency(0.2f, 1.6f);
        StopAllCoroutines();
        StartCoroutine(coroutine);
    }

    private IEnumerator TempChangeFrequency(float frequency, float duration)
    {
        pulseFrequency = frequency;
        magMult = 1.2f / pulseFrequency;
        widMult = 1 / (pulseFrequency / 0.1f);
        yield return new WaitForSeconds(duration);
        pulseFrequency = 0.8f;
        magMult = 1.2f / pulseFrequency;
        widMult = 1 / (pulseFrequency / 0.1f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        for (int i = 0; i <= nodes.Length - 2; i++)
            Gizmos.DrawLine(nodes[i], nodes[i + 1]);
    }

    #endregion
}

#region Editor Operations

[CustomEditor(typeof(Neon))]
public class TMapInspector : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Update"))
        {
            Neon neon = (Neon)target;
            neon.UpdateLineRenderers();
        }
        if (GUILayout.Button("Flash"))
        {
            Neon neon = (Neon)target;
            neon.Flash();
        }
    }
}

#endregion
