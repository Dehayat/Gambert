using UnityEngine;

public class HitBox : MonoBehaviour
{
    public delegate void Hit(Collider2D other);
    public event Hit OnHit;

    private Collider2D hitBoxTrigger;

    private void Awake()
    {
        hitBoxTrigger = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<AttackBox>() != null)
        {
            collision.GetComponent<AttackBox>().HitTrigger(hitBoxTrigger);
        }
        OnHit?.Invoke(collision);
    }
}
