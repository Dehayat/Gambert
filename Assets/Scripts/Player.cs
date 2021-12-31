using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float jumpSpeed = 5f;
    public float maxJumpLength = 0.5f;
    public float minJumpLength = 0.1f;
    public float dashSpeed = 15f;
    public float dashLength = 0.3f;
    public float dashCoolDown = 0.2f;
    public float preAttackLength = 0.1f;
    public float attackLength = 0.1f;
    public float postAttackLength = 0.1f;
    public GameObject attackPrefab = null;
    public float attackCoolDown = 0.1f;
    public LayerMask groundLayer = 1;
    public float attackBufferInputLength = 0.1f;
    public float dashBufferInputLength = 0.1f;
    public float jumpBufferInputLength = 0.2f;
    public float maxFallSpeed = 20f;
    public float jumpForgiveLength = 0.06f;
    public float attackKnockBackSpeed = 30f;
    public float attackKnockBackDuration = 0.04f;
    public float getHitDuration = 0.1f;
    public SpriteRenderer spriteRenderer;
    public Material getHitMaterial;
    public float invincibleDuration = 0.3f;
    public GameObject attackUpPrefab = null;
    public GameObject attackDownPrefab = null;
    public float bounceSpeed = 10f;
    public float bounceDuration = 0.2f;
    public GameObject hitEffectGO;
    public float hitEffectDuration = 0.1f;
    public float hitPauseDuration = 0.5f;
    public ParticleSystem getHitEffectParticles;
    public int maxRally = 1;
    public float deadFloatUpSpeed = 2f;
    public PlayerUI playerUI;
    public PlayerSound sound;
    public float stepInterval = 0.15f;

    private Rigidbody2D rb;
    private Collider2D boxCol;
    private GPlayerInputActions input;
    private Animator anim;
    private Health health;
    private Material normalMaterial;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCol = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        health = GetComponent<Health>();
        input = new GPlayerInputActions();
        normalMaterial = spriteRenderer.material;

        input.Enable();
    }
    private void Start()
    {
        playerUI.UpdateHealth(health.currentHealth, currentCanRally);
    }
    private void OnEnable()
    {
        input.Player.Jump.performed += Jump_performed;
        input.Player.Jump.canceled += Jump_canceled;
        input.Player.Dash.performed += Dash_performed;
        input.Player.Attack.performed += Attack_performed;
        input.Player.Pause.performed += Pause_performed; ;
        health.OnDamaged += Health_OnDamaged;
        attackDownPrefab.GetComponent<AttackBox>().OnHit += Player_OnHitDown;
        attackPrefab.GetComponent<AttackBox>().OnHit += Player_OnHit;
        attackUpPrefab.GetComponent<AttackBox>().OnHit += Player_OnHit;
    }

    private bool isPaused = false;
    private float savedTimeScale = 0f;
    private void Pause_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (isPaused)
        {
            var particlesMain = getHitEffectParticles.main;
            particlesMain.useUnscaledTime = true;
            isPaused = false;
            Time.timeScale = savedTimeScale;
        }
        else
        {
            var particlesMain = getHitEffectParticles.main;
            particlesMain.useUnscaledTime = false;
            savedTimeScale = Time.timeScale;
            Time.timeScale = 0f;
            isPaused = true;
        }
        playerUI.Pause(isPaused);
    }

    private bool isBouncing = false;
    private float bounceTimer = 0f;
    private void Player_OnHitDown(HitInfo info)
    {
        isBouncing = true;
        bounceTimer = bounceDuration;
        Player_OnHit(info);
    }
    private void Player_OnHit(HitInfo info)
    {
        sound.HitSomething();
        var enemyRB = info.target.GetComponent<Collider2D>().attachedRigidbody;
        if (enemyRB != null && enemyRB.CompareTag("Enemy") && currentCanRally > 0)
        {
            sound.Rally();
            health.currentHealth++;
            currentCanRally--;
            playerUI.UpdateHealth(health.currentHealth, currentCanRally);
            if (currentCanRally > maxRally)
            {
                currentCanRally = maxRally;
            }
            if (currentCanRally > health.maxHealth - health.currentHealth)
            {
                currentCanRally = health.maxHealth - health.currentHealth;
            }
        }
        StartCoroutine(HitEffect(info));
    }
    IEnumerator HitEffect(HitInfo info)
    {
        hitEffectGO.transform.position = info.point;
        Vector3 hitDirection = info.direction;
        Utility.RotateTowards(info.point + hitDirection * 5f, hitEffectGO.transform);
        hitEffectGO.SetActive(true);
        hitEffectGO.transform.parent = null;
        float timer = hitEffectDuration;
        while (timer > 0f)
        {
            timer -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        hitEffectGO.transform.parent = transform;
        hitEffectGO.SetActive(false);
    }

    private void OnDisable()
    {
        input.Player.Jump.performed -= Jump_performed;
        input.Player.Jump.canceled -= Jump_canceled;
        input.Player.Dash.performed -= Dash_performed;
        input.Player.Attack.performed -= Attack_performed;
        health.OnDamaged -= Health_OnDamaged;
        attackDownPrefab.GetComponent<AttackBox>().OnHit -= Player_OnHitDown;
        attackPrefab.GetComponent<AttackBox>().OnHit -= Player_OnHit;
        attackUpPrefab.GetComponent<AttackBox>().OnHit -= Player_OnHit;
    }

    private bool wantToStopJump = false;
    private void Jump_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        wantToStopJump = true;
    }

    private bool wantToJump = false;
    private void Jump_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        wantToJump = true;
    }

    private bool wantToDash = false;
    private void Dash_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        wantToDash = true;
    }

    public bool wantToAttack = false;
    private void Attack_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        wantToAttack = true;
    }
    private float beingAttackedTimer = 0f;
    private Vector2 knockBackDirection = Vector2.zero;
    private float invincibleTimer = 0f;
    private int currentCanRally = 0;

    private bool isDead = false;
    private void Health_OnDamaged(HitInfo info)
    {
        sound.GetHit();
        if (health.currentHealth == 0)
        {
            rb.velocity = Vector2.up * deadFloatUpSpeed;
            rb.isKinematic = true;
            if (currentAttackPrefab != null)
            {
                currentAttackPrefab.SetActive(false);
            }
            isDead = true;
            health.SetCanHit(false);
            getHitEffectParticles.Play();
            anim.Play("Dead");
            playerUI.UpdateHealth(health.currentHealth, 0);
            return;
        }

        currentCanRally++;
        if (currentCanRally > maxRally)
        {
            currentCanRally = maxRally;
        }
        if (currentCanRally > health.maxHealth - health.currentHealth)
        {
            currentCanRally = health.maxHealth - health.currentHealth;
        }
        isBeingAttacked = true;
        beingAttackedTimer = 0f;
        knockBackDirection = info.direction;
        knockBackDirection.y = 0.5f;
        if (knockBackDirection.x > 0)
        {
            knockBackDirection.x = 1;
        }
        else
        {
            knockBackDirection.x = -1;
        }
        //knockBackDirection.Normalize();
        spriteRenderer.material = getHitMaterial;
        invincibleTimer = invincibleDuration;
        health.SetCanHit(false);
        playerUI.UpdateHealth(health.currentHealth, currentCanRally);
        if (isAttacking)
        {
            isAttacking = false;
            currentAttackPrefab.SetActive(false);
            attackCoolDownTimer = attackCoolDown;
            anim.Play("Idle");
        }
        if (isDashing)
        {
            anim.Play("Idle");
            isDashing = false;
        }
        else
        {
            savedGravity = rb.gravityScale;
        }
        if (isJumping)
        {
            anim.Play("Idle");
            isJumping = false;
            endJumpOnMin = false;
        }
        if (isBouncing)
        {
            isBouncing = false;
        }
        if (isRunning)
        {
            isRunning = false;
        }
        if (isFalling)
        {
            isFalling = false;
        }
        rb.gravityScale = 0f;
        StartCoroutine(PauseGameForHit());
    }
    IEnumerator PauseGameForHit()
    {
        Time.timeScale = 0f;
        getHitEffectParticles.Play();
        yield return new WaitForSecondsRealtime(hitPauseDuration);
        yield return new WaitWhile(() => isPaused);
        Time.timeScale = 1f;
    }

    private int moveDir = 0;
    private int lookYDir = 0;
    private float lookPower = 0f;
    private float movePower = 0f;
    private void Update()
    {
        if (isDead || isPaused)
        {
            return;
        }
        float moveDirFloat = input.Player.Move.ReadValue<float>();
        movePower = Mathf.Abs(moveDirFloat);
        float lookYDirFloat = input.Player.Look.ReadValue<float>();
        lookPower = Mathf.Abs(lookYDirFloat);
        if (moveDirFloat > Mathf.Epsilon)
        {
            moveDir = 1;
        }
        else if (moveDirFloat < -Mathf.Epsilon)
        {
            moveDir = -1;
        }
        else
        {
            moveDir = 0;
        }
        if (moveDir != 0)
        {
            lookDir = moveDir;
        }
        else
        {
            lookDir = facingDir;
        }
        if (lookYDirFloat > Mathf.Epsilon)
        {
            lookYDir = 1;
        }
        else if (lookYDirFloat < -Mathf.Epsilon)
        {
            lookYDir = -1;
        }
        else
        {
            lookYDir = 0;
        }
    }
    private bool isJumping = false;
    private bool endJumpOnMin = false;
    private float jumpTimer = 0f;
    private bool isDashing = false;
    private float dashTimer = 0f;
    private float savedGravity = 0f;
    private float dashCoolDownTimer = 0f;
    private int dashDir = 1;
    private int lookDir = 1;
    private bool isAttacking = false;
    private float attackTimer = 0f;
    private bool jumpWasUsed = false;
    private int facingDir = 1;
    private float attackCoolDownTimer = 0f;
    private float attackBufferedTimer = 0f;
    private float dashBufferedTimer = 0f;
    private float jumpBufferedTimer = 0f;
    private bool endNextJumpOnMin = false;
    private bool isRunning = false;
    private bool isIdle = false;
    private bool isFalling = false;
    private float jumpForgiveTimer = 0f;
    private bool preAttackDone = false;
    private bool attackDone = false;
    private bool isBeingAttacked = false;
    private GameObject currentAttackPrefab = null;
    private float stepTimer = 0f;

    private void FixedUpdate()
    {
        if (isDead)
        {
            return;
        }
        CheckIsOnGround();

        //Being Attacked
        if (isBeingAttacked)
        {
            if (beingAttackedTimer < attackKnockBackDuration)
            {
                rb.velocity = knockBackDirection * attackKnockBackSpeed;
            }
            else if (beingAttackedTimer < attackKnockBackDuration + getHitDuration)
            {
                rb.velocity = Vector2.zero;
                rb.gravityScale = savedGravity;
            }
            else
            {
                rb.gravityScale = savedGravity;
                isBeingAttacked = false;
            }
            anim.Play("Idle");
            beingAttackedTimer += Time.fixedDeltaTime;
        }

        //Dash
        if ((wantToDash || dashBufferedTimer > Mathf.Epsilon) && !isBeingAttacked && !isDashing && !isAttacking && dashCoolDownTimer <= Mathf.Epsilon)
        {
            sound.Dash();
            isDashing = true;
            dashTimer = 0;
            savedGravity = rb.gravityScale;
            rb.gravityScale = 0f;
            dashDir = lookDir;
            dashBufferedTimer = 0;
            anim.Play("Dash");
            if (isRunning)
            {
                isRunning = false;
            }
            if (isJumping)
            {
                isJumping = false;
                endJumpOnMin = false;
            }
            if (isBouncing)
            {
                isBouncing = false;
            }
        }
        else if (wantToDash)
        {
            dashBufferedTimer = dashBufferInputLength;
        }
        if (isDashing)
        {
            dashTimer += Time.fixedDeltaTime;
            if (dashTimer >= dashLength)
            {
                rb.velocity = Vector2.zero;
                rb.gravityScale = savedGravity;
                isDashing = false;
            }
            else
            {
                dashCoolDownTimer = dashCoolDown;
                rb.velocity = new Vector2(dashSpeed * dashDir, 0);
            }
        }
        else
        {
            if (dashCoolDownTimer > 0f)
            {
                dashCoolDownTimer -= Time.fixedDeltaTime;
            }
        }
        //Attack
        if ((wantToAttack || attackBufferedTimer > Mathf.Epsilon) && !isBeingAttacked && !isAttacking && !isDashing && attackCoolDownTimer <= Mathf.Epsilon)
        {
            if (lookYDir == 1 && lookPower > movePower)
            {
                anim.Play("AttackUp");
                currentAttackPrefab = attackUpPrefab;
            }
            else if (lookYDir == -1 && lookPower > movePower && !isOnGround)
            {
                anim.Play("AttackDown");
                currentAttackPrefab = attackDownPrefab;
            }
            else
            {
                currentAttackPrefab = attackPrefab;
                anim.Play("Attack");
            }
            sound.Attack();
            isAttacking = true;
            attackTimer = 0f;
            attackBufferedTimer = 0;
            preAttackDone = false;
            attackDone = false;
        }
        else if (wantToAttack)
        {
            attackBufferedTimer = attackBufferInputLength;
        }
        if (isAttacking)
        {
            if (attackTimer < preAttackLength + attackLength + postAttackLength)
            {
                attackTimer += Time.fixedDeltaTime;
            }

            if (attackTimer < preAttackLength)
            {

            }
            else if (attackTimer >= preAttackLength && attackTimer < preAttackLength + attackLength)
            {
                if (!preAttackDone)
                {
                    preAttackDone = true;
                    currentAttackPrefab.SetActive(true);
                }
            }
            else if (attackTimer >= preAttackLength + attackLength && attackTimer < preAttackLength + attackLength + postAttackLength)
            {
                if (!attackDone)
                {
                    attackDone = true;
                    currentAttackPrefab.SetActive(false);
                }
            }
            else
            {
                currentAttackPrefab.SetActive(false);
                isAttacking = false;
                attackCoolDownTimer = attackCoolDown;
                if (isJumping)
                {
                    anim.Play("Jump");
                }
                if (isRunning)
                {
                    anim.Play("Walk");
                }
            }
        }
        else
        {
            if (attackCoolDownTimer > 0f)
            {
                attackCoolDownTimer -= Time.fixedDeltaTime;
            }
        }
        //Jump
        if (isOnGround && jumpWasUsed)
        {
            jumpWasUsed = false;
        }
        if ((isOnGround || jumpForgiveTimer > Mathf.Epsilon) && (wantToJump || jumpBufferedTimer > Mathf.Epsilon) && !isBeingAttacked && !isJumping && !isDashing && !jumpWasUsed)
        {
            if (endNextJumpOnMin)
            {
                endNextJumpOnMin = false;
                endJumpOnMin = true;
            }
            sound.Jump();
            jumpForgiveTimer = 0f;
            anim.Play("Jump");
            isJumping = true;
            jumpWasUsed = true;
            jumpTimer = 0f;
            jumpBufferedTimer = 0f;
            if (isRunning)
            {
                isRunning = false;
            }
        }
        else if (wantToJump)
        {
            jumpBufferedTimer = jumpBufferInputLength;
        }
        if (wantToStopJump && isJumping)
        {
            if (jumpTimer < minJumpLength)
            {
                endJumpOnMin = true;
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
                isJumping = false;
            }
        }
        if (isJumping)
        {
            jumpTimer += Time.fixedDeltaTime;
            if (jumpTimer >= maxJumpLength || (jumpTimer >= minJumpLength && endJumpOnMin) || HasHitHead())
            {
                endJumpOnMin = false;
                rb.velocity = new Vector2(rb.velocity.x, 0);
                isJumping = false;
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
            }
        }
        if (isBouncing)
        {
            rb.velocity = new Vector2(rb.velocity.x, bounceSpeed);
            if (bounceTimer > 0f)
            {
                bounceTimer -= Time.fixedDeltaTime;
            }
            else
            {
                isBouncing = false;
                rb.velocity = new Vector2(rb.velocity.x, 0f);
            }
        }

        //General
        if (invincibleTimer > 0)
        {
            invincibleTimer -= Time.fixedDeltaTime;
            if (invincibleTimer < Mathf.Epsilon)
            {
                spriteRenderer.material = normalMaterial;
                health.SetCanHit(true);
            }
        }
        if (!isDashing && !isBeingAttacked && rb.velocity.y < -maxFallSpeed)
        {
            rb.velocity = new Vector2(rb.velocity.x, -maxFallSpeed);
        }
        if (!isDashing && !isBeingAttacked)
        {
            rb.velocity = new Vector2(moveDir * moveSpeed, rb.velocity.y);
        }
        if (!isRunning && moveDir != 0)
        {
            if (!isAttacking && !isJumping && isOnGround && !isDashing && !isBeingAttacked)
            {
                isRunning = true;
                anim.Play("Walk");
                stepTimer = stepInterval;
                sound.Step();
            }
        }
        else if (moveDir == 0)
        {
            isRunning = false;
        }

        if (isFalling && isOnGround)
        {
            sound.Land();
        }
        if (!isRunning && !isDashing && !isAttacking && !isJumping && !isBeingAttacked)
        {
            if (isOnGround)
            {
                if (!isIdle)
                {
                    isIdle = true;
                    anim.Play("Idle");
                }
            }
            else
            {
                if (!isFalling)
                {
                    isFalling = true;
                    anim.Play("Falling");
                }
            }
        }
        else
        {
            isIdle = false;
            isFalling = false;
        }
        if (isFalling)
        {
            isIdle = false;
        }
        if (isOnGround)
        {
            isFalling = false;
            jumpForgiveTimer = jumpForgiveLength;
        }
        if (!isDashing && !isAttacking && !isBeingAttacked)
        {
            if (isRunning)
            {
                if (stepTimer > 0f)
                {
                    stepTimer -= Time.fixedDeltaTime;
                }
                else
                {
                    stepTimer = stepInterval;
                    sound.Step();
                }
            }

            if (facingDir != lookDir)
            {
                Vector3 scale = transform.localScale;
                scale.x *= -1;
                transform.localScale = scale;
                facingDir = lookDir;
            }
        }
        if (attackBufferedTimer > 0)
        {
            attackBufferedTimer -= Time.fixedDeltaTime;
        }
        if (dashBufferedTimer > 0)
        {
            dashBufferedTimer -= Time.fixedDeltaTime;
        }
        if (jumpBufferedTimer > 0)
        {
            if (wantToStopJump)
            {
                endNextJumpOnMin = true;
            }
            jumpBufferedTimer -= Time.fixedDeltaTime;
            if (jumpBufferedTimer < Mathf.Epsilon)
            {
                endNextJumpOnMin = false;
            }
        }
        if (jumpForgiveTimer > 0)
        {
            jumpForgiveTimer -= Time.fixedDeltaTime;
        }
        wantToJump = false;
        wantToStopJump = false;
        wantToDash = false;
        wantToAttack = false;
    }

    private bool HasHitHead()
    {
        Vector2 origin = boxCol.bounds.center + (Vector3.up * (boxCol.bounds.extents.y * 0.75f));
        Vector2 size = new Vector2(boxCol.bounds.size.x * 0.6f, boxCol.bounds.extents.y * 0.45f);
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.NoFilter();
        contactFilter.useTriggers = false;
        contactFilter.SetLayerMask(groundLayer);
        int collisionCount = Physics2D.OverlapBox(origin, size, 0, contactFilter, collisionResult);
        return collisionCount != 0;
    }

    private bool isOnGround = false;
    private Collider2D[] collisionResult = new Collider2D[1];
    private void CheckIsOnGround()
    {
        Vector2 origin = boxCol.bounds.center + (Vector3.down * (boxCol.bounds.extents.y * 0.75f));
        Vector2 size = new Vector2(boxCol.bounds.size.x * 0.8f, boxCol.bounds.extents.y * 0.75f);
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.NoFilter();
        contactFilter.useTriggers = false;
        contactFilter.SetLayerMask(groundLayer);
        int collisionCount = Physics2D.OverlapBox(origin, size, 0, contactFilter, collisionResult);
        if (collisionCount != 0)
        {
            isOnGround = true;
        }
        else
        {
            isOnGround = false;
        }
    }
    private void OnDrawGizmos()
    {
        Collider2D col = GetComponent<Collider2D>();
        Vector2 origin = col.bounds.center + (Vector3.down * (col.bounds.extents.y * 0.75f));
        Vector2 size = new Vector2(col.bounds.size.x * 0.8f, col.bounds.extents.y * 0.75f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(origin, size);

        origin = col.bounds.center + (Vector3.up * (col.bounds.extents.y * 0.75f));
        size = new Vector2(col.bounds.size.x * 0.6f, col.bounds.extents.y * 0.45f);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(origin, size);
    }
}
