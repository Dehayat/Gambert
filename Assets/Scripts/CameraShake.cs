using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{

    public AnimationCurve shakeCurveX;
    public AnimationCurve shakeCurveY;
    public float shakePower;
    public float timeScale = 1f;
    public bool shake = false;

    public void Shake()
    {
        if (timeScale > 0 && shakePower > 0)
        {
            StartCoroutine(ShakeSequence());
        }
    }

    void Update()
    {
        if (shake)
        {
            shake = false;
        }
    }

    IEnumerator ShakeSequence()
    {
        float timer = 0f;
        transform.localPosition = Vector3.zero;
        Vector3 position = transform.position;
        while (timer * timeScale < shakeCurveX.keys[shakeCurveX.keys.Length - 1].time)
        {
            timer += Time.fixedDeltaTime;
            position = new Vector3(shakeCurveX.Evaluate(timer * timeScale), shakeCurveY.Evaluate(timer * timeScale), 0);
            position *= shakePower;
            transform.localPosition = position;
            yield return new WaitForEndOfFrame();
        }
        transform.localPosition = Vector3.zero;
    }
}
