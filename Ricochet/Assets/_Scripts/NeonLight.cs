using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enumerables;

public class NeonLight : MonoBehaviour {

    [SerializeField] Vector2[] nodes;
    [SerializeField] float speed;

    private TrailRenderer tr;

    void Start()
    {
        transform.position = nodes[0];
        tr = GetComponent<TrailRenderer>();
    }

    public void Initialize()
    {
        transform.position = nodes[0];
        tr = GetComponent<TrailRenderer>();
    }

    void Update()
    {
        if (Input.anyKeyDown)
            HitTheLights();
    }

    public void HitTheLights()
    {
        StartCoroutine("StartTheShow");
    }

    private IEnumerator StartTheShow()
    {
        tr.enabled = false;
        transform.position = nodes[0];
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
        yield return new WaitForSeconds(tr.time);
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
                tr.startColor = Color.blue;
                tr.endColor = Color.blue;
                break;
            case ETeam.BlueTeam:
                tr.startColor = Color.red;
                tr.endColor = Color.red;
                break;
        }
    }
}
