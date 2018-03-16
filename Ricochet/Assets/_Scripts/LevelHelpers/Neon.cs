using System.Collections;
using System.Collections.Generic;
using System;
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
    [SerializeField] private LineRenderer auraBand;
    [SerializeField] private NeonFlash neonFlash;
    #endregion

    #region Private Fields
    private int emissionID;
    private Material innerMaterial;
    private Color innerColor;
    private Material outerMaterial;
    private Color outerColor;

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

    public void EmissionPulse()
    {
        float emissionMagnitude;
        float emissionWidth;

        if (!linearPulse)
        {
            float y = -(Mathf.Sin(x / 2)) - (Mathf.Cos(x) / 4) + 0.75f;
            //float y = (float)Y;
            //Debug.Log(y);
            emissionMagnitude = 0.9f + y * 2;
            emissionWidth = 1 + y / 2;
        }
        else
        {
            float y = (Mathf.Sin(x) + 1) * 0.5f;
            //double Y = (Math.Sin(x) + 1) * 0.5;
            //float y = (float)Y;
            //Debug.Log(y);
            emissionMagnitude = 0.9f + y * 2;
            emissionWidth = 1 + y / 2;
        }

        x += Time.deltaTime * pulseFrequency;
        if (x >= 2 * Mathf.PI)
            x = 0;

        Color emissionColor = innerColor * Mathf.LinearToGammaSpace(emissionMagnitude);
        innerMaterial.SetColor(emissionID, emissionColor);
        //innerBand.startWidth = emissionWidth;
        //innerBand.endWidth = emissionWidth;
        innerBand.widthMultiplier = emissionWidth;

        emissionColor = outerColor * Mathf.LinearToGammaSpace(emissionMagnitude);
        outerMaterial.SetColor(emissionID, emissionColor);
        //outerBand.startWidth = emissionWidth;
        //outerBand.endWidth = emissionWidth;
        outerBand.widthMultiplier = emissionWidth;
    }

    public void Flash()
    {
        NeonFlash nf = Instantiate(neonFlash);
        nf.gameObject.SetActive(true);
        nf.Initialize();
        nf.HitTheLights();

        IEnumerator coroutine = TempChangeFrequency(9f, 2.5f);
        StopCoroutine("TempChangeFrequency");
        StartCoroutine(coroutine);
        StartCoroutine("AuraPulse");
    }

    private IEnumerator TempChangeFrequency(float frequency, float duration)
    {
        pulseFrequency = frequency;
        linearPulse = true;
        yield return new WaitForSeconds(duration);
        linearPulse = false;
        pulseFrequency = ogF;
    }

    private IEnumerator AuraPulse()
    {
        LineRenderer aura = Instantiate(auraBand);
        aura.widthMultiplier = 0.1f;
        aura.gameObject.SetActive(true);
        float w = 0.1f;
        while (w < 3f)
        {
            float y = (Mathf.Log10(w) + 4f) * 0.5f;
            aura.widthMultiplier = y * 2f;
            aura.startColor = new Color(aura.startColor.r, aura.startColor.g, aura.startColor.b, 2.3f - y);
            aura.endColor = new Color(aura.startColor.r, aura.startColor.g, aura.startColor.b, 2.3f - y);
            w += Time.fixedDeltaTime;
            yield return new WaitForEndOfFrame();
        }
        Destroy(aura.gameObject);
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
