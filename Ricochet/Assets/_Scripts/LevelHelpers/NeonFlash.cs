using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enumerables;

public class NeonFlash : MonoBehaviour {

    [SerializeField] Vector3[] nodes;
    [SerializeField] float speed;
    [SerializeField] TrailRenderer tr;

    public void Initialize()
    {
        transform.position = nodes[0];
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
            Vector2 target = nodes[i + 1];
            Vector2 difference = target - position;
            Vector2 translation;
            if (difference.magnitude <= s * Time.fixedDeltaTime)
            {
                i++;
                if (i == nodes.Length - 1)
                    break;
                transform.position = target;
                target = nodes[i + 1];
            }
            else
            {
                translation = difference.normalized * (speed * Time.fixedDeltaTime);
                transform.Translate(translation);
            }
            position = transform.position;
            yield return new WaitForFixedUpdate();
        }
        yield return new WaitForSeconds(tr.time);
        Destroy(gameObject);
    }

    public void SetTrailGradient(Gradient grad)
    {
        tr.colorGradient = grad;
    }

    public void SetNodes(Vector3[] nodes)
    {
        this.nodes = nodes;
    }

    public void SetWidth(float width)
    {
        tr.widthMultiplier = width;
    }
}
