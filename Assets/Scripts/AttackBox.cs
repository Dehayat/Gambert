using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBox : MonoBehaviour
{
    public bool hitOnce = true;

    public delegate void Hit(HitBox other);
    public event Hit OnHit;

    private Collider2D attackTrigger;
    private void Awake()
    {
        attackTrigger = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        TryHit(collision);
    }

    private void TryHit(Collider2D collision)
    {
        HitBox target = collision.GetComponent<HitBox>();
        if (target != null && target.canHit)
        {
            Vector2 hitBoxCenter = collision.bounds.center;
            Vector2 attackBoxCenter = attackTrigger.bounds.center;
            Vector2 attackDir = hitBoxCenter - attackBoxCenter;
            target.Attack(this, attackDir.normalized);
            OnHit?.Invoke(target);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (hitOnce) return;
        TryHit(collision);
    }
}
