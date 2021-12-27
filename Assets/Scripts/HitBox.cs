using UnityEngine;

public class HitBox : MonoBehaviour
{
    public delegate void Hit(AttackBox other);
    public event Hit OnHit;

    public bool canHit = true;

    private Collider2D hitBoxTrigger;

    public void Attack(AttackBox attacker)
    {
        if (!canHit) return;
        OnHit?.Invoke(attacker);
    }

    private void Awake()
    {
        hitBoxTrigger = GetComponent<Collider2D>();
    }
}
