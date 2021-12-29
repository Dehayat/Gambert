using UnityEngine;


public class HitBox : MonoBehaviour
{
    public event Hit OnHit;

    public bool canHit = true;

    private Collider2D hitBoxTrigger;

    public void Attack(HitInfo info)
    {
        if (!canHit) return;
        OnHit?.Invoke(info);
    }

    private void Awake()
    {
        hitBoxTrigger = GetComponent<Collider2D>();
    }
}
