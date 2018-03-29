using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enumerables;

public class NeonLight : MonoBehaviour {

    private struct LightColor
    {
        public Color neon;
        public Color spark;
    }

    [SerializeField] Vector2[] nodes;
    [SerializeField] ETeam team;

    private NeonFlash neonFlash;
    private LineRenderer light;
    private LightColor blue;
    private LightColor red;
    
    private void Start()
    {
        neonFlash = GetComponentInChildren<NeonFlash>();
        neonFlash.gameObject.SetActive(false);
    }

    #region Public Methods
    public void InitializeColors(Color[] colors)
    {
        blue.neon = colors[0];
        blue.spark = colors[1];
        red.neon = colors[2];
        red.spark = colors[3];
    }

    public void HitTheLights()
    {
        NeonFlash nf = Instantiate(neonFlash);
        nf.gameObject.SetActive(true);
        nf.Initialize();
        nf.HitTheLights();
    }

    #endregion

    #region Helpers

    //void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.black;
    //    for (int i = 0; i < nodes.Length - 1; i++)
    //        Gizmos.DrawLine(nodes[i], nodes[i + 1]);
    //}
    #endregion
}
