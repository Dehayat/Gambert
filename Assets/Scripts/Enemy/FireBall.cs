using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    public AttackBox attackBox;

    private void OnEnable()
    {
        attackBox.OnHit += AttackBox_OnHit;
    }
    private void OnDisable()
    {
        attackBox.OnHit -= AttackBox_OnHit;
    }

    private void AttackBox_OnHit(HitBox other)
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }
}
