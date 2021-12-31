using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    public GameObject boss;
    public Transform bossStartPos;
    public Transform bossEndPos;
    public float bossMoveDuration = 2f;
    public Transform exitBlocker;
    public float exitBlockDuration = 0.3f;
    public float exitEndYPosition = 2.12f;
    public float timeBeforeBoss = 1f;

    private bool bossStarted = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (bossStarted)
        {
            return;
        }
        bossStarted = true;
        StartCoroutine(StartBoss());
    }

    IEnumerator StartBoss()
    {
        float timer = 0;
        Vector3 startPosition = exitBlocker.position;
        Vector3 endPosition = exitBlocker.position;
        endPosition.y = exitEndYPosition;
        SFX.instance.SlamDoor();
        while (timer < exitBlockDuration)
        {
            timer += Time.deltaTime;
            exitBlocker.position = Vector3.Lerp(startPosition, endPosition, timer / exitBlockDuration);
            yield return null;
        }
        yield return new WaitForSeconds(timeBeforeBoss);
        boss.SetActive(true);
        while (timer < bossMoveDuration)
        {
            timer += Time.deltaTime;
            boss.transform.position = Vector3.Lerp(bossStartPos.position, bossEndPos.position, timer / bossMoveDuration);
            yield return null;
        }
        boss.GetComponent<dwagon>().canAttack = true;
    }
}
