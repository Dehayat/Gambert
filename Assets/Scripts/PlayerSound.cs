using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    public void PauseSound(bool pause)
    {
        AudioListener.pause = pause;
    }

    public AudioClip step;
    public AudioClip step2;
    public AudioClip jump;
    public AudioClip land;
    public AudioClip dash;
    public AudioClip attack;
    public AudioClip hitSomething;
    public AudioClip getHit;
    public AudioClip rally;

    private AudioSource source;
    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }
    private bool playStep2 = false;
    public void Step()
    {
        source.pitch = Random.Range(0.8f, 1.2f);
        if (playStep2)
        {
            source.PlayOneShot(step2, source.volume);
        }
        else
        {
            source.PlayOneShot(step, source.volume);
        }
        source.pitch = 1f;
        playStep2 = !playStep2;
    }
    public void Jump()
    {
        source.PlayOneShot(jump, source.volume);
    }
    public void Land()
    {
        source.PlayOneShot(land, source.volume + 0.3f);
    }
    public void Dash()
    {
        source.PlayOneShot(dash, source.volume);
    }
    public void Attack()
    {
        source.PlayOneShot(attack, source.volume);
    }
    public void HitSomething()
    {
        source.PlayOneShot(hitSomething, source.volume);
    }
    public void GetHit()
    {
        source.PlayOneShot(getHit, source.volume+0.2f);
    }
    public void Rally()
    {
        source.PlayOneShot(rally, source.volume - 0.1f);
    }

}
