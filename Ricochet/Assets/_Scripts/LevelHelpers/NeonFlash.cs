using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enumerables;

public class NeonFlash : MonoBehaviour {

    [SerializeField] Vector3[] nodes;
    [SerializeField] float speed;
    [SerializeField] TrailRenderer tr;

    //private ParticleSystem ps;
    //private ParticleSystem.MainModule psm;

    public void Initialize()
    {
        transform.position = nodes[0];
        //ps = GetComponent<ParticleSystem>();
        //psm = ps.main;
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
        //ParticleSystem.EmissionModule emitter = ps.emission;
        //emitter.rateOverTime = 0f;
        //yield return new WaitForSeconds(psm.startLifetime.constant + ps.trails.lifetime.constant);
        yield return new WaitForSeconds(tr.time);
        Destroy(gameObject);
    }

    public void SetNodes(Vector3[] nodes)
    {
        this.nodes = nodes;
    }

    public void SetWidth(float width)
    {
        //tr.startWidth = width;
        //tr.endWidth = width;
        tr.widthMultiplier = width;
    }
}
