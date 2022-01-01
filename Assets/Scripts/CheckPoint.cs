using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public Transform checkPointSpawn;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerData.SetCheckPoint(checkPointSpawn.position);
    }
}
