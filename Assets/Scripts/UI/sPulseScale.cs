using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sPulseScale : MonoBehaviour
{
    [SerializeField, Range(0.5f,1)] private float scaleDecreaseFactor;
    [SerializeField] private float pulseDur;
    [SerializeField] private AnimationCurve pulseCurve;
    private Vector3 initScale;
    private Vector3 decreasedScale;

    private void Start()
    {

    }

    private void OnEnable()
    {
        initScale = transform.localScale;
        decreasedScale = initScale * scaleDecreaseFactor;
        StartCoroutine(Pulse());
    }

    private void OnDisable()
    {
        transform.localScale = initScale;
        StopAllCoroutines();
    }

    IEnumerator Pulse()
    {
        float speed = 1 / pulseDur;
        while (true)
        {
            float t = 0f;
            
            while (t <= 1f)
            {
                t += Time.deltaTime * speed;
                Vector3 newScale = Vector3.Slerp(initScale, decreasedScale, pulseCurve.Evaluate(t));
                transform.localScale = newScale;

                yield return null;
            }
            yield return null;
        }

    }
}
