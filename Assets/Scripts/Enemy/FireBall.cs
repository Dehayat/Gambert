using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    public AttackBox attackBox;

    private Rigidbody2D rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate()
    {
        Utility.RotateTowards(transform.position + (Vector3)rb.velocity, transform);
    }

    private void OnEnable()
    {
        attackBox.OnHit += AttackBox_OnHit;
    }
    private void OnDisable()
    {
        attackBox.OnHit -= AttackBox_OnHit;
    }

    private void AttackBox_OnHit(HitInfo info)
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        SFX.instance.FireBallImpact();
        Destroy(gameObject);
    }
}
