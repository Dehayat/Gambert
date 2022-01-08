using System.Collections;
using UnityEngine;

public class bz : MonoBehaviour
{
    public enum bzState
    {
        wandering,
        goingBackToWanderArea,
        gettingHit
    }

    public float wanderSpeed = 2f;
    public Vector2 wanderDirection = Vector2.zero;
    public Collider2D wanderArea = null;
    public float recoilSpeed = 50f;
    public float recoilDuration = 0.1f;
    public float staggerDuration = 0.4f;
    public SpriteRenderer spriteRenderer;
    public Material getHitMaterial;
    public float invincibleDuration = 0.3f;
    public ParticleSystem deathEffect;

    private Rigidbody2D rb;
    private Health health;
    private bzState state;
    private Material normalMaterial;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<Health>();
        state = bzState.wandering;
        normalMaterial = spriteRenderer.material;
    }
    private void OnEnable()
    {
        health.OnDamaged += Health_OnDamaged;
    }
    private void OnDisable()
    {
        health.OnDamaged -= Health_OnDamaged;
    }

    private Vector2 recoilVelocity = Vector2.zero;
    private float invincibleTimer = 0f;
    private bool isDead = false;
    private void Health_OnDamaged(HitInfo info)
    {
        if (isDead) return;
        if (health.currentHealth == 0)
        {
            isDead = true;
            deathEffect.Play();
            deathEffect.transform.parent = null;
            deathEffect.transform.localScale = new Vector3(1f, 1f, 1f);
            SFX.instance.FlyDie();
            Destroy(deathEffect, 1f);
            Destroy(gameObject, 0.1f);
            rb.velocity = Vector2.zero;
            return;
        }
        stagerDone = false;
        recoilDone = false;
        state = bzState.gettingHit;
        if (info.direction.x > 0)
        {
            recoilVelocity.x = 1;
        }
        else
        {
            recoilVelocity.x = -1;
        }
        recoilVelocity.y = 0.5f;
        recoilVelocity *= recoilSpeed;
        spriteRenderer.material = getHitMaterial;
        invincibleTimer = invincibleDuration;
        StartCoroutine(StaggerSequence());
        StartCoroutine(RecoilSequence());
    }

    private bool stagerDone = false;
    IEnumerator StaggerSequence()
    {
        float staggerTimer = staggerDuration;
        while (staggerTimer > Mathf.Epsilon)
        {
            yield return new WaitForFixedUpdate();
            staggerTimer -= Time.fixedDeltaTime;
        }
        stagerDone = true;
    }
    private bool recoilDone = false;
    IEnumerator RecoilSequence()
    {
        float recoilTimer = recoilDuration;
        while (recoilTimer > Mathf.Epsilon)
        {
            yield return new WaitForFixedUpdate();
            recoilTimer -= Time.fixedDeltaTime;
        }
        recoilVelocity = Vector2.zero;
        recoilDone = true;
    }

    public bool dieNow = false;
    private void FixedUpdate()
    {
        if (isDead)
        {
            return;
        }
        if (dieNow)
        {
            health.currentHealth = 0;
            Health_OnDamaged(default);
        }
        switch (state)
        {
            case bzState.wandering:
                UpdateWander();
                break;
            case bzState.goingBackToWanderArea:
                UpdateGoBack();
                break;
            case bzState.gettingHit:
                UpdateGetHit();
                break;
            default:
                break;
        }
        if (invincibleTimer > 0)
        {
            invincibleTimer -= Time.fixedDeltaTime;
            if (invincibleTimer < Mathf.Epsilon)
            {
                spriteRenderer.material = normalMaterial;
                health.SetCanHit(true);
            }
        }
    }
    private void UpdateWander()
    {
        rb.velocity = wanderDirection * wanderSpeed;
        FaceVelocity();
        if (!isInWanderArea)
        {
            state = bzState.goingBackToWanderArea;
        }
    }

    private void FaceVelocity()
    {
        if (rb.velocity.x > Mathf.Epsilon)
        {
            Vector3 scale = transform.localScale;
            scale.x = 1;
            transform.localScale = scale;
        }
        else if (rb.velocity.x < -Mathf.Epsilon)
        {
            Vector3 scale = transform.localScale;
            scale.x = -1;
            transform.localScale = scale;
        }
    }

    private void UpdateGoBack()
    {
        Vector3 directionBackToArea = wanderArea.bounds.center - transform.position;
        directionBackToArea.z = 0;
        rb.velocity = directionBackToArea.normalized * wanderSpeed;
        FaceVelocity();
        if (isInWanderArea && (directionBackToArea.magnitude < 1f || Random.Range(0f, 1f) < 0.2f))
        {
            wanderDirection = new Vector2(directionBackToArea.x, 0);
            wanderDirection.Normalize();
            state = bzState.wandering;
        }
    }

    private void UpdateGetHit()
    {
        rb.velocity = recoilVelocity;
        if (recoilDone && stagerDone)
        {
            recoilDone = false;
            stagerDone = false;
            state = bzState.wandering;
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        wanderDirection = -wanderDirection;
    }

    private bool isInWanderArea = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == wanderArea)
        {
            isInWanderArea = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision == wanderArea)
        {
            isInWanderArea = false;
        }
    }
}
