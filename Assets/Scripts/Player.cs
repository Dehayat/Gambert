using System;
using System.Collections;
using System.Collections.Generic;
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

    private Rigidbody2D rb;
    private BoxCollider2D boxCol;
    private GPlayerInputActions input;
    private Animator anim;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCol = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        input = new GPlayerInputActions();
        input.Enable();
    }
    private void OnEnable()
    {
        input.Player.Jump.performed += Jump_performed;
        input.Player.Jump.canceled += Jump_canceled;
        input.Player.Dash.performed += Dash_performed;
        input.Player.Attack.performed += Attack_performed;
    }

    private void OnDisable()
    {
        input.Player.Jump.performed -= Jump_performed;
        input.Player.Jump.canceled -= Jump_canceled;
        input.Player.Dash.performed -= Dash_performed;
        input.Player.Attack.performed -= Attack_performed;
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

    private int moveDir = 0;
    private void Update()
    {
        float moveDirFloat = input.Player.Move.ReadValue<Vector2>().x;
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

    private void FixedUpdate()
    {
        CheckIsOnGround();
        //Dash
        if ((wantToDash || dashBufferedTimer > Mathf.Epsilon) && !isDashing && !isAttacking && dashCoolDownTimer <= Mathf.Epsilon)
        {
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
                jumpTimer = 0;
                endJumpOnMin = false;
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
        if ((wantToAttack || attackBufferedTimer > Mathf.Epsilon) && !isAttacking && !isDashing && attackCoolDownTimer <= Mathf.Epsilon)
        {
            anim.Play("Attack");
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
                    attackPrefab.SetActive(true);
                }
            }
            else if (attackTimer >= preAttackLength + attackLength && attackTimer < preAttackLength + attackLength + postAttackLength)
            {
                if (!attackDone)
                {
                    attackDone = true;
                    attackPrefab.SetActive(false);
                }
            }
            else
            {
                attackPrefab.SetActive(false);
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
        if ((isOnGround || jumpForgiveTimer > Mathf.Epsilon) && (wantToJump || jumpBufferedTimer > Mathf.Epsilon) && !isJumping && !isDashing && !jumpWasUsed)
        {
            if (endNextJumpOnMin)
            {
                endNextJumpOnMin = false;
                endJumpOnMin = true;
            }
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
            if (jumpTimer >= maxJumpLength || (jumpTimer >= minJumpLength && endJumpOnMin))
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
        //General
        if (rb.velocity.y < -maxFallSpeed)
        {
            rb.velocity = new Vector2(rb.velocity.x, -maxFallSpeed);
        }
        if (!isDashing)
        {
            rb.velocity = new Vector2(moveDir * moveSpeed, rb.velocity.y);
        }
        if (!isRunning && moveDir != 0)
        {
            if (!isAttacking && !isJumping && isOnGround && !isDashing)
            {
                isRunning = true;
                anim.Play("Walk");
            }
        }
        else if (moveDir == 0)
        {
            isRunning = false;
        }
        if (!isRunning && !isDashing && !isAttacking && !isJumping)
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
        if (!isDashing && !isAttacking)
        {
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

    private bool isOnGround = false;
    private void CheckIsOnGround()
    {
        Vector2 origin = boxCol.bounds.center + (Vector3.down * (boxCol.bounds.extents.y * 0.75f));
        Vector2 size = new Vector2(boxCol.bounds.size.x * 0.8f, boxCol.bounds.extents.y * 0.55f);
        Collider2D colliderHit = Physics2D.OverlapBox(origin, size, 0, groundLayer);
        if (colliderHit != null)
        {
            isOnGround = true;
        }
        else
        {
            isOnGround = false;
        }
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        Vector2 origin = col.bounds.center + (Vector3.down * (col.bounds.extents.y * 0.75f));
        Vector2 size = new Vector2(col.bounds.size.x * 0.8f, col.bounds.extents.y * 0.55f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(origin, size);
    }
#endif
}
