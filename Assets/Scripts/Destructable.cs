using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructable : MonoBehaviour
{
    private Health health;

    private void Awake()
    {
        health = GetComponent<Health>();
    }

    private void OnEnable()
    {
        health.OnDamaged += OnDamaged;
    }
    private void OnDisable()
    {
        health.OnDamaged += OnDamaged;
    }

    private void OnDamaged(AttackBox other)
    {
        if (health.currentHealth == 0)
        {
            Destroy(gameObject);
        }
    }

}
