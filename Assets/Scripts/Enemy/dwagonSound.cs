using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dwagonSound : MonoBehaviour
{
    public AudioClip glide;
    public AudioClip whoosh;
    public AudioClip slam;
    public AudioClip fireBall;
    public AudioClip getHit;
    public AudioClip die;
    public AudioClip fall;

    private AudioSource source;
    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    public void Glide()
    {
        source.PlayOneShot(glide, source.volume - 0.2f);
    }
    public void Whoosh()
    {
        source.PlayOneShot(whoosh);
    }
    public void Slam()
    {
        source.PlayOneShot(slam, source.volume - 0.2f);
    }
    public void FireBall()
    {
        source.PlayOneShot(fireBall);
    }
    public void GetHit()
    {
        source.PlayOneShot(getHit, source.volume - 0.1f);
    }
    public void Die()
    {
        source.PlayOneShot(die);
    }
    public void Fall()
    {
        source.PlayOneShot(fall);
    }
}
