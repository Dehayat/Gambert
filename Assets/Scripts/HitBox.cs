using UnityEngine;

public class HitBox : MonoBehaviour
{
    public delegate void Hit(AttackBox other, Vector2 attackDir);
    public event Hit OnHit;

    public bool canHit = true;

    private Collider2D hitBoxTrigger;

    public void Attack(AttackBox attacker, Vector2 attackDir)
    {
        if (!canHit) return;
        OnHit?.Invoke(attacker, attackDir);
    }

    private void Awake()
    {
        hitBoxTrigger = GetComponent<Collider2D>();
    }
}
