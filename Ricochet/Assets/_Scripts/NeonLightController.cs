using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enumerables;

public class NeonLightController : MonoBehaviour {

    [SerializeField] private Color neonBlue;
    [SerializeField] private Color sparkBlue;
    [SerializeField] private Color neonRed;
    [SerializeField] private Color sparkRed;

    Transform[] lights;
    Color[] colors;

	void Start () {
        int count = transform.childCount;
        lights = new Transform[count];
        for (int i = 0; i < count; i++)
        {
            Transform child = transform.GetChild(i);
            lights[i] = child;
            child.gameObject.SetActive(false);
        }
        colors = new Color[4];
        colors[0] = neonBlue;
        colors[1] = sparkBlue;
        colors[2] = neonRed;
        colors[3] = sparkRed;
    }

    public void HitTheLights(ETeam t)
    {
        foreach (Transform tr in lights)
        {
            Transform tra = Instantiate(tr);
            tra.gameObject.SetActive(true);
            NeonLight lightInstance = tra.GetComponent<NeonLight>();
            lightInstance.Initialize(colors);
            lightInstance.SetColor(t);
            lightInstance.HitTheLights();
        }
    }
}
