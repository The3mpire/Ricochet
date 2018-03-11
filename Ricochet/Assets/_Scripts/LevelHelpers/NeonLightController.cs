using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enumerables;

public class NeonLightController : MonoBehaviour {

    [SerializeField] private Color neonBlue;
    [SerializeField] private Color sparkBlue;
    [SerializeField] private Color neonRed;
    [SerializeField] private Color sparkRed;

    NeonLight[] lights;
    Color[] colors;

	void Start () {
        colors = new Color[4];
        colors[0] = neonBlue;
        colors[1] = sparkBlue;
        colors[2] = neonRed;
        colors[3] = sparkRed;

        int count = transform.childCount;
        lights = new NeonLight[count];
        for (int i = 0; i < count; i++)
        {
            lights[i] = transform.GetChild(i).GetComponent<NeonLight>(); ;
            lights[i].InitializeColors(colors);
        }
    }

    public void HitTheLights(ETeam t)
    {
        foreach (NeonLight light in lights)
        {
            light.HitTheLights();
        }
    }
}
