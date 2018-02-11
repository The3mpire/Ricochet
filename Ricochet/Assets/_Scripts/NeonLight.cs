using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enumerables;

public class NeonLight : MonoBehaviour {

    [SerializeField] Vector2[] nodes;
    [SerializeField] float speed;

    private TrailRenderer tr;
    private ParticleSystem ps;
    private ParticleSystem.MainModule psm;
    private Color neonBlue;
    private Color sparkBlue;
    private Color neonRed;
    private Color sparkRed;

    void Start()
    {
        transform.position = nodes[0];
        tr = GetComponent<TrailRenderer>();
        ps = GetComponent<ParticleSystem>();
        psm = ps.main;
    }

    public void Initialize(Color[] colors)
    {
        transform.position = nodes[0];
        tr = GetComponent<TrailRenderer>();
        ps = GetComponent<ParticleSystem>();
        psm = ps.main;
        neonBlue = colors[0];
        sparkBlue = colors[1];
        neonRed = colors[2];
        sparkRed = colors[3];
    }

    public void HitTheLights()
    {
        StopCoroutine("StartTheShow");
        StartCoroutine("StartTheShow");
    }

    private IEnumerator StartTheShow()
    {
        tr.enabled = false;
        transform.position = nodes[0];
        yield return new WaitForEndOfFrame();
        tr.enabled = true;
        float s = speed;
        int i = 0;
        Vector2 position = nodes[0];
        while (true)
        {
            Vector2 newTarget;

            Vector2 target = nodes[i + 1];
            Vector2 difference = target - position;
            Vector2 translation;
            if (difference.magnitude <= s * Time.fixedDeltaTime)
            {
                float rem = speed - difference.magnitude;
                i++;
                if (i == nodes.Length - 1)
                    break;
                newTarget = nodes[i + 1];
                translation = (newTarget - target).normalized * (rem * Time.fixedDeltaTime);
                translation = target + translation;
                translation = translation - position;
                target = newTarget;
            }
            else
            {
                translation = difference.normalized * (speed * Time.fixedDeltaTime);
            }
            transform.Translate(translation);
            position = transform.position;
            yield return new WaitForFixedUpdate();
        }
        ParticleSystem.EmissionModule emitter = ps.emission;
        emitter.rateOverTime = 0f;
        yield return new WaitForSeconds(psm.startLifetime.constant + ps.trails.lifetime.constant);
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        for (int i = 0; i < nodes.Length - 1; i++)
            Gizmos.DrawLine(nodes[i], nodes[i+1]);
    }

    public void SetColor(ETeam t)
    {
        switch (t)
        {
            case ETeam.RedTeam:
                tr.startColor = neonBlue;
                tr.endColor = neonBlue;
                psm.startColor = sparkBlue;
                break;
            case ETeam.BlueTeam:
                tr.startColor = neonRed;
                tr.endColor = neonRed;
                psm.startColor = sparkRed;
                break;
        }
    }
}
