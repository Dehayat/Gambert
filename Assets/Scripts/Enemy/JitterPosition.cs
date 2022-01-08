using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JitterPosition : MonoBehaviour
{
    public float jitterAmount = 0.2f;
    public float jitterDuration = 0.1f;

    private void Start()
    {
        StartCoroutine(JitterSequence());
    }
    IEnumerator JitterSequence()
    {
        while (true)
        {
            Vector3 dir = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
            dir.Normalize();
            dir *= jitterAmount;
            float timer = 0;
            while (timer <= jitterDuration)
            {
                Vector3 cur = Vector3.Lerp(Vector3.zero, dir, timer / jitterDuration);
                transform.localPosition = cur;
                timer += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
            timer = 0f;
            while (timer <= jitterDuration)
            {
                Vector3 cur = Vector3.Lerp(dir, Vector3.zero, timer / jitterDuration);
                transform.localPosition= cur;
                timer += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
        }
    }
}
