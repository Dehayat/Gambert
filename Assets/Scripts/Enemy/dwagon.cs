using System.Collections;
using UnityEngine;

public class dwagon : MonoBehaviour
{
    public enum dwagonState
    {
        flying,
        glideAttack,
        slamAttack,
        FireBallAttack
    }
    public int facingDirection = 1;
    public float floatSpeed = 2f;
    public float floatDistance = 0.5f;
    public float flyDuration = 2f;
    public float normalFlySpeed = 4f;
    public Transform flyPositionLeftRange;
    public Transform flyPositionRightRange;
    [Range(0f, 100f)]
    public float GlideWeight = 40f;
    [Range(0f, 100f)]
    public float SlamWeight = 40f;
    [Range(0f, 100f)]
    public float FireWeight = 20f;

    [Header("Glide Attack")]
    public Transform glideRightPosition;
    public Transform glideLeftPosition;
    public float waitBeforeGlideDuration = 1f;
    public float glideSpeed = 15f;
    public float waitAfterGlideDuration = 1.5f;
    public float startYOffset = -0.8f;

    [Header("Slam Attack")]
    public float groundY = 0f;
    public float waitBeforeSlamDuration = 0.5f;
    public GameObject slamShockwave;
    public float shockWaveDuration = 0.3f;
    public float waitAfterSlamDuration = 1f;
    public float slamSpeed = 18f;
    public float flyAwaySpeed = 18f;
    public Transform dwagonBody;

    [Header("FireBall Attack")]
    public float waitBeforeFireBallDuration = 0.3f;
    public GameObject fireBall;
    public Transform[] fireballTargets = null;
    public Transform fireballSource;
    public float fireBallTimeToTarget = 1f;
    public AnimationCurve fireBallCurve;
    public float waitAfterFireBallDuration = 0.3f;
    public GameObject fireBallStub;

    private Rigidbody2D rb;
    private Health health;
    private Animator anim;
    private Animator shockWaveAnim;
    private Animator fireBallStubAnim;
    private dwagonState state;
    private Transform target;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<Health>();
        anim = GetComponent<Animator>();
        shockWaveAnim = slamShockwave.GetComponent<Animator>();
        fireBallStubAnim = fireBallStub.GetComponent<Animator>();
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
            case dwagonState.FireBallAttack:
                FireBallAttackState();
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
        Float();
        if (flyTimer > Mathf.Epsilon)
        {
            flyTimer -= Time.fixedDeltaTime;
        }
        else
        {
            float action = Random.Range(0f, GlideWeight + SlamWeight + FireWeight);
            if (action < GlideWeight)
            {
                state = dwagonState.glideAttack;
            }
            else if (action < GlideWeight + SlamWeight)
            {
                state = dwagonState.slamAttack;
            }
            else
            {
                state = dwagonState.FireBallAttack;
            }
        }
    }

    private void Float()
    {
        if (Vector3.Distance(transform.position, startFloatPosition) >= floatDistance)
        {
            startFloatPosition = transform.position;
            floatDirection *= -1;
        }
        rb.velocity = floatSpeed * floatDirection * Vector2.up;

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
        anim.Play("Fly");
        while (waitTimer > 0)
        {
            waitTimer -= Time.fixedDeltaTime;
            Float();
            yield return new WaitForFixedUpdate();
        }
        rb.velocity = Vector2.zero;
        Quaternion savedRotation = transform.rotation;
        transform.position += new Vector3(0, startYOffset, 0);
        endPosition += new Vector3(0, startYOffset, 0);
        RotateTowards(endPosition);
        anim.Play("Slam");
        while (Vector3.Distance(transform.position, endPosition) > 0.3f)
        {
            Vector3 moveDirection = endPosition - transform.position;
            Vector2 moveVelocity = moveDirection.normalized * glideSpeed;
            rb.velocity = moveVelocity;
            FaceVelocity();
            yield return new WaitForFixedUpdate();
        }
        transform.position -= new Vector3(0, startYOffset, 0);
        transform.rotation = savedRotation;
        waitTimer = waitBeforeGlideDuration;
        anim.Play("Fly");
        startFloatPosition = transform.position;
        rb.velocity = Vector2.zero;
        while (waitTimer > 0)
        {
            waitTimer -= Time.fixedDeltaTime;
            Float();
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
        slamPosition.x = Mathf.Clamp(slamPosition.x, flyPositionLeftRange.position.x, flyPositionRightRange.position.x);
        FacePosition(slamPosition);
        Quaternion savedRotation = transform.rotation;
        RotateTowards(slamPosition);
        anim.Play("Slam");
        while (Vector3.Distance(transform.position, slamPosition) > 0.3f)
        {
            Vector3 moveDirection = slamPosition - transform.position;
            Vector2 moveVelocity = moveDirection.normalized * slamSpeed;
            rb.velocity = moveVelocity;
            yield return new WaitForFixedUpdate();
        }
        anim.Play("Fly");
        transform.rotation = savedRotation;
        waitTimer = shockWaveDuration;
        rb.velocity = Vector2.zero;
        slamShockwave.SetActive(true);
        shockWaveAnim.Play("Slam");
        while (waitTimer > 0)
        {
            waitTimer -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        shockWaveAnim.Play("Nothing");
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
        shockWaveAnim.Play("FlyAway");
        anim.Play("Slam");
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
                shockWaveAnim.Play("Nothing");
                slamShockwave.SetActive(false);
                slamShockwave.transform.parent = transform;
                slamShockwave.transform.localPosition = shockwavePosition;
            }
            yield return new WaitForFixedUpdate();
        }
        anim.Play("Fly");
        if (waitTimer > 0 && waitTimer <= shockWaveDuration + Mathf.Epsilon)
        {
            while (waitTimer > 0)
            {
                waitTimer -= Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
            shockWaveAnim.Play("Nothing");
            slamShockwave.SetActive(false);
            slamShockwave.transform.parent = transform;
            slamShockwave.transform.localPosition = shockwavePosition;
        }

        rb.velocity = Vector2.zero;
        FacePosition(target.position);
        GoToFlyingState();
        slamRoutine = null;
    }

    private void RotateTowards(Vector3 targetPosition, Transform transformToMove = null)
    {
        if (transformToMove == null)
        {
            transformToMove = transform;
        }
        Vector3 lookDirection = targetPosition - transformToMove.position;
        lookDirection.z = 0;
        lookDirection.Normalize();
        float rotationAngle = Vector3.SignedAngle(Vector3.up, lookDirection, Vector3.forward);
        transformToMove.rotation = Quaternion.AngleAxis(rotationAngle, Vector3.forward);
    }

    private void FireBallAttackState()
    {
        if (fireBallRoutine == null)
        {
            fireBallRoutine = StartCoroutine(FireBallAttackSequence());
        }
    }

    private Coroutine fireBallRoutine = null;
    IEnumerator FireBallAttackSequence()
    {
        bool chooseRight = Random.Range(0f, 1f) < 0.5f;
        chooseRight = true;
        Vector3 startPosition;
        if (chooseRight)
        {
            startPosition = flyPositionRightRange.position;
        }
        else
        {
            startPosition = flyPositionLeftRange.position;
        }
        while (Vector3.Distance(transform.position, startPosition) > 0.3f)
        {
            Vector3 moveDirection = startPosition - transform.position;
            Vector2 moveVelocity = moveDirection.normalized * normalFlySpeed;
            rb.velocity = moveVelocity;
            FaceVelocity();
            yield return new WaitForFixedUpdate();
        }
        FacePosition(target.position);
        float waitTimer = waitBeforeFireBallDuration;
        rb.velocity = Vector2.zero;
        fireBallStub.SetActive(true);
        fireBallStubAnim.Play("Spawn");
        while (waitTimer > 0)
        {
            waitTimer -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        GameObject[] fireBalls = new GameObject[fireballTargets.Length];
        for (int i = 0; i < fireballTargets.Length; i++)
        {
            fireBalls[i] = Instantiate(fireBall, fireballSource.position, Quaternion.identity);
            RotateTowards(fireballTargets[i].position, fireBalls[i].transform);
        }
        fireBallStub.SetActive(false);
        waitTimer = 0f;
        while (waitTimer < fireBallTimeToTarget)
        {
            waitTimer += Time.fixedDeltaTime;
            for (int i = 0; i < fireballTargets.Length; i++)
            {
                if (fireBalls[i] == null) continue;
                Vector3 fireBallPosition = Vector3.Lerp(fireballSource.position, fireballTargets[i].position, waitTimer / fireBallTimeToTarget);
                float xlerp = fireBallCurve.Evaluate(waitTimer / fireBallTimeToTarget);
                fireBallPosition.x = Mathf.Lerp(fireballSource.position.x, fireballTargets[i].position.x, xlerp);
                fireBalls[i].transform.position = fireBallPosition;
            }
            yield return new WaitForFixedUpdate();
        }
        waitTimer = waitAfterFireBallDuration;
        startFloatPosition = transform.position;
        rb.velocity = Vector2.zero;
        anim.Play("Fly");
        while (waitTimer > 0)
        {
            waitTimer -= Time.fixedDeltaTime;
            Float();
            yield return new WaitForFixedUpdate();
        }
        GoToFlyingState();
        fireBallRoutine = null;
    }


    private void GoToFlyingState()
    {
        state = dwagonState.flying;
        flyTimer = flyDuration;
        startFloatPosition = transform.position;
        anim.Play("Fly");
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
