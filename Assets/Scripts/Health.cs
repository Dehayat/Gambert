using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public delegate void Damaged(AttackBox attacker);
    public event Damaged OnDamaged;

    public int currentHealth = 3;
    public int maxHealth = 3;
    public bool canHit = true;

    public HitBox[] hitBoxes;

    public void SetCanHit(bool canHit)
    {
        this.canHit = canHit;
        if (hitBoxes == null) return;
        for (int i = 0; i < hitBoxes.Length; i++)
        {
            hitBoxes[i].canHit = canHit;
        }
    }

    private void OnEnable()
    {
        if (hitBoxes == null) return;
        for (int i = 0; i < hitBoxes.Length; i++)
        {
            hitBoxes[i].OnHit += Health_OnHit;
        }
    }

    private void OnDisable()
    {
        if (hitBoxes == null) return;
        for (int i = 0; i < hitBoxes.Length; i++)
        {
            hitBoxes[i].OnHit -= Health_OnHit;
        }
    }

    private void Health_OnHit(AttackBox other)
    {
        if (!canHit) return;
        currentHealth -= 1;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        if (currentHealth == 0)
        {
            SetCanHit(false);
        }
        Debug.Log(gameObject + " Got Hit");
        OnDamaged?.Invoke(other);
    }

}
