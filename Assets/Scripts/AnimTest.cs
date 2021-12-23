using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimTest : MonoBehaviour
{
    [SerializeField]
    private bool walk;

    private Animator anim;
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    private void Update()
    {
        if (walk)
        {
            walk = false;
            anim.Play("Walk", 0, 0.5f);
        }
    }
}
