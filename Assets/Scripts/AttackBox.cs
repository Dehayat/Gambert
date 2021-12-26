using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBox : MonoBehaviour
{
    public delegate void Hit(Collider2D other);
    public event Hit OnHit;

    public void HitTrigger(Collider2D hitBox)
    {
        OnHit?.Invoke(hitBox);
    }
}
