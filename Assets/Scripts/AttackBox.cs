using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBox : MonoBehaviour
{
    public bool hitOnce = true;

    public delegate void Hit(HitBox other);
    public event Hit OnHit;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        TryHit(collision);
    }

    private void TryHit(Collider2D collision)
    {
        HitBox target = collision.GetComponent<HitBox>();
        if (target != null && target.canHit)
        {
            target.Attack(this);
            OnHit?.Invoke(target);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (hitOnce) return;
        TryHit(collision);
    }
}
