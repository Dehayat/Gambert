using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager gm;

    public void BeatBoss()
    {
        beatBossUI.SetActive(true);
    }

    private void Awake()
    {
        gm = this;
    }

    public GameObject player;
    public GameObject beatBossUI;

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
