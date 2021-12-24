using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float runSpeed = 10;

    private PlayerInputHandler input;
    private Rigidbody2D rb2d;

    private int direction = 1;

    private bool isRunning = false;

    private void Awake()
    {
        input = GetComponent<PlayerInputHandler>();
        rb2d = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        if (input.MoveDirection != 0)
        {
            direction = input.MoveDirection;
            isRunning = true;
        }
        else
        {
            isRunning = false;
        }
    }

    private void FixedUpdate()
    {
        if (isRunning)
        {
            float moveSpeed = direction * runSpeed;
            rb2d.velocity = new Vector2(moveSpeed, rb2d.velocity.y);
        }
        else
        {
            rb2d.velocity = new Vector2(0f, rb2d.velocity.y);
        }
    }
}
