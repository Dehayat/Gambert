using System.Collections;
using UnityEngine;

public class dwagon : MonoBehaviour
{
    public enum dwagonState
    {
        flying,
        glideAttack,
        slamAttack
    }
    public int facingDirection = 1;
    public float floatSpeed = 2f;
    public float floatDistance = 0.5f;
    public float flyDuration = 2f;
    public float normalFlySpeed = 4f;
    public Transform flyPositionLeftRange;
    public Transform flyPositionRightRange;

    [Header("Glide Attack")]
    public Transform glideRightPosition;
    public Transform glideLeftPosition;
    public float waitBeforeGlideDuration = 1f;
    public float glideSpeed = 15f;
    public float waitAfterGlideDuration = 1.5f;

    [Header("Slam Attack")]
    public float groundY = 0f;
    public float waitBeforeSlamDuration = 0.5f;
    public GameObject slamShockwave;
    public float shockWaveDuration = 0.3f;
    public float waitAfterSlamDuration = 1f;
    public float slamSpeed = 18f;
    public float flyAwaySpeed = 18f;

    private Rigidbody2D rb;
    private Health health;
    private dwagonState state;
    private Transform target;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<Health>();
        GoToFlyingState();
    }
    private void Start()
    {
        Vector3 scale = transform.localScale;
        scale.x = facingDirection;
        transform.localScale = scale;
        startFloatPosition = transform.position;
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }
    private void OnEnable()
    {
        health.OnDamaged += Health_OnDamaged;
    }
    private void OnDisable()
    {
        health.OnDamaged -= Health_OnDamaged;
    }
    private void Health_OnDamaged(AttackBox attacker, Vector2 attackDir)
    {
        //stagerDone = false;
        //recoilDone = false;
        //state = bzState.gettingHit;
        //recoilVelocity = attackDir * recoilSpeed;
        //spriteRenderer.material = getHitMaterial;
        //invincibleTimer = invincibleDuration;
        //StartCoroutine(StaggerSequence());
        //StartCoroutine(RecoilSequence());
    }
    private void FixedUpdate()
    {
        switch (state)
        {
            case dwagonState.flying:
                FlyingState();
                break;
            case dwagonState.glideAttack:
                GlideAttackState();
                break;
            case dwagonState.slamAttack:
                SlamAttackState();
                break;
            default:
                break;
        }
    }

    private Vector3 startFloatPosition;
    private int floatDirection = -1;
    private float flyTimer = 0f;
    private void FlyingState()
    {
        if (Vector3.Distance(transform.position, startFloatPosition) >= floatDistance)
        {
            startFloatPosition = transform.position;
            floatDirection *= -1;
        }
        rb.velocity = floatSpeed * floatDirection * Vector2.up;

        if (flyTimer > Mathf.Epsilon)
        {
            flyTimer -= Time.fixedDeltaTime;
        }
        else
        {
            state = dwagonState.slamAttack;
        }
    }

    private Coroutine glideRoutine = null;
    private void GlideAttackState()
    {
        if (glideRoutine == null)
        {
            glideRoutine = StartCoroutine(GlideAttackSequence());
        }
    }
    IEnumerator GlideAttackSequence()
    {
        bool chooseRight = Random.Range(0f, 1f) < 0.5f;
        Vector3 startPosition;
        Vector3 endPosition;
        if (chooseRight)
        {
            startPosition = glideRightPosition.position;
            endPosition = glideLeftPosition.position;
        }
        else
        {
            startPosition = glideLeftPosition.position;
            endPosition = glideRightPosition.position;
        }
        while (Vector3.Distance(transform.position, startPosition) > 0.3f)
        {
            Vector3 moveDirection = startPosition - transform.position;
            Vector2 moveVelocity = moveDirection.normalized * normalFlySpeed;
            rb.velocity = moveVelocity;
            FaceVelocity();
            yield return new WaitForFixedUpdate();
        }
        float waitTimer = waitBeforeGlideDuration;
        startFloatPosition = transform.position;
        rb.velocity = Vector2.zero;
        FacePosition(endPosition);
        while (waitTimer > 0)
        {
            waitTimer -= Time.fixedDeltaTime;
            FlyingState();
            yield return new WaitForFixedUpdate();
        }
        rb.velocity = Vector2.zero;
        while (Vector3.Distance(transform.position, endPosition) > 0.3f)
        {
            Vector3 moveDirection = endPosition - transform.position;
            Vector2 moveVelocity = moveDirection.normalized * glideSpeed;
            rb.velocity = moveVelocity;
            FaceVelocity();
            yield return new WaitForFixedUpdate();
        }
        waitTimer = waitBeforeGlideDuration;
        startFloatPosition = transform.position;
        rb.velocity = Vector2.zero;
        while (waitTimer > 0)
        {
            waitTimer -= Time.fixedDeltaTime;
            FlyingState();
            yield return new WaitForFixedUpdate();
        }
        Vector3 targetPosition = flyPositionLeftRange.position;
        targetPosition.x = Random.Range(flyPositionLeftRange.position.x, flyPositionRightRange.position.x);
        while (Vector3.Distance(transform.position, targetPosition) > 0.3f)
        {
            Vector3 moveDirection = targetPosition - transform.position;
            Vector2 moveVelocity = moveDirection.normalized * normalFlySpeed;
            rb.velocity = moveVelocity;
            FaceVelocity();
            yield return new WaitForFixedUpdate();
        }
        rb.velocity = Vector2.zero;
        FacePosition(target.position);
        GoToFlyingState();
        glideRoutine = null;
    }

    private Coroutine slamRoutine = null;
    private void SlamAttackState()
    {
        if (slamRoutine == null)
        {
            slamRoutine = StartCoroutine(SlamAttackSequence());
        }
    }

    IEnumerator SlamAttackSequence()
    {
        float waitTimer = waitBeforeSlamDuration;
        rb.velocity = Vector2.zero;
        while (waitTimer > 0)
        {
            waitTimer -= Time.fixedDeltaTime;
            //FlyingState();
            yield return new WaitForFixedUpdate();
        }
        Vector3 slamPosition = target.position;
        slamPosition.y = groundY;
        while (Vector3.Distance(transform.position, slamPosition) > 0.3f)
        {
            Vector3 moveDirection = slamPosition - transform.position;
            Vector2 moveVelocity = moveDirection.normalized * slamSpeed;
            rb.velocity = moveVelocity;
            FaceVelocity();
            yield return new WaitForFixedUpdate();
        }
        waitTimer = shockWaveDuration;
        rb.velocity = Vector2.zero;
        slamShockwave.SetActive(true);
        while (waitTimer > 0)
        {
            waitTimer -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        slamShockwave.SetActive(false);
        waitTimer = waitAfterSlamDuration;
        startFloatPosition = transform.position;
        while (waitTimer > 0)
        {
            waitTimer -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        Vector3 targetPosition = flyPositionLeftRange.position;
        targetPosition.x = transform.position.x;
        waitTimer = shockWaveDuration;
        slamShockwave.SetActive(true);
        Vector3 shockwavePosition = slamShockwave.transform.localPosition;
        slamShockwave.transform.parent = null;
        while (Vector3.Distance(transform.position, targetPosition) > 0.3f)
        {
            Vector3 moveDirection = targetPosition - transform.position;
            Vector2 moveVelocity = moveDirection.normalized * flyAwaySpeed;
            rb.velocity = moveVelocity;
            if (waitTimer > 0 && waitTimer <= shockWaveDuration + Mathf.Epsilon)
            {
                waitTimer -= Time.fixedDeltaTime;
            }
            else
            {
                waitTimer = 999f;
                slamShockwave.SetActive(false);
                slamShockwave.transform.parent = transform;
                slamShockwave.transform.localPosition = shockwavePosition;
            }
            yield return new WaitForFixedUpdate();
        }
        if (waitTimer > 0 && waitTimer <= shockWaveDuration + Mathf.Epsilon)
        {
            while (waitTimer > 0)
            {
                waitTimer -= Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
            slamShockwave.SetActive(false);
            slamShockwave.transform.parent = transform;
            slamShockwave.transform.localPosition = shockwavePosition;
        }

        rb.velocity = Vector2.zero;
        FacePosition(target.position);
        GoToFlyingState();
        slamRoutine = null;
    }

    private void GoToFlyingState()
    {
        state = dwagonState.flying;
        flyTimer = flyDuration;
        startFloatPosition = transform.position;
    }

    private void FacePosition(Vector3 target)
    {
        if ((target - transform.position).x > 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = 1;
            transform.localScale = scale;
        }
        else
        {
            Vector3 scale = transform.localScale;
            scale.x = -1;
            transform.localScale = scale;
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
}
