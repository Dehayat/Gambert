using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructable : MonoBehaviour
{
    public HitBox hitBox;

    private void OnEnable()
    {
        hitBox.OnHit += HitBox_OnHit;
    }
    private void OnDisable()
    {
        hitBox.OnHit += HitBox_OnHit;
    }

    private void HitBox_OnHit(Collider2D other)
    {
        Destroy(gameObject);
    }

}
