using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject player;

    void Start()
    {
        if (PlayerData.hasCheckPoint)
        {
            player.transform.position = PlayerData.checkPoint;
            FindObjectOfType<FollowCamera>().SnapToTarget();
        }
        BlackScreen.instance.FadeFromBlack();
    }
}
