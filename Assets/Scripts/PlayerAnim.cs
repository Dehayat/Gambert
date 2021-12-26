using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum animation
{
    idle,
    walk
}

public class PlayerAnim : MonoBehaviour
{
    public void PlayAnimation(animation animId)
    {
        switch (animId)
        {
            case global::animation.idle:
                anim.Play("Idle");
                break;
            case global::animation.walk:
                anim.Play("Walk");
                break;
            default:
                break;
        }
    }

    private Animator anim;
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

}
