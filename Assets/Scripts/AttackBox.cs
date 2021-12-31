using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct HitInfo
{
    public AttackBox attacker;
    public HitBox target;
    public Vector2 direction;
    public Vector3 point;
    public int damageType;
}

public delegate void Hit(HitInfo info);
public class AttackBox : MonoBehaviour
{
    public bool hitOnce = true;
    public int damageType = 0;

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
            HitInfo hitInfo = default;
            hitInfo.attacker = this;
            hitInfo.target = target;
            hitInfo.direction = attackDir.normalized;
            hitInfo.point = collision.ClosestPoint(attackTrigger.bounds.center);
            hitInfo.damageType = damageType;
            target.Attack(hitInfo);
            OnHit?.Invoke(hitInfo);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (hitOnce) return;
        TryHit(collision);
    }
}
