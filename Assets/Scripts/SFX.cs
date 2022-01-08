using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFX : MonoBehaviour
{
    public static SFX instance;

    private float lastFireBall = 0f;
    public void FireBallImpact()
    {
        if (lastFireBall + delayBetweenFireBalls > Time.fixedTime)
        {
            return;
        }
        lastFireBall = Time.fixedTime;
        sound.pitch = 1f;
        sound.pitch += Random.Range(-0.3f, 0.3f);
        sound.PlayOneShot(fireBallImpact, 0.4f);
    }
    public void SlamDoor()
    {
        sound.PlayOneShot(slamDoorSound, 0.8f);
    }
    public void FlyDie()
    {
        sound.PlayOneShot(flyDead, 0.8f);
    }

    public AudioClip fireBallImpact;
    public float delayBetweenFireBalls = 0.1f;
    public AudioClip slamDoorSound;
    public AudioClip flyDead;

    private AudioSource sound;
    private void Awake()
    {
        if (instance != null)
        {
            DestroyImmediate(this);
        }
        else
        {
            instance = this;
        }
        sound = GetComponent<AudioSource>();
    }
}
