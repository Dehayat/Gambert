using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField]
    private bool takeInput = true;

    public void EnableInput()
    {
        takeInput = true;
    }
    public void DisableInput()
    {
        takeInput = false;
        ResetInput();
    }
    public void ResetInput()
    {
        moveDirection = 0;
    }

    public int MoveDirection
    {
        get
        {
            return moveDirection;
        }
    }

    private PlayerInputActions inputActions;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        inputActions.Enable();
    }

    private void ActionPerformed(InputAction.CallbackContext context)
    {
        if (!takeInput) return;
        Debug.Log(context.action.id == inputActions.Player.Fire.id);
    }

    private void OnEnable()
    {
        //inputActions.Player.Fire.performed += ActionPerformed;
    }
    private void OnDisable()
    {
        //inputActions.Player.Fire.performed -= ActionPerformed;
    }

    private int moveDirection = 0;

    void Update()
    {
        if (!takeInput) return;
        float moveDirectionFloat = inputActions.Player.Move.ReadValue<Vector2>().x;
        if (moveDirectionFloat > Mathf.Epsilon)
        {
            moveDirection = 1;
        }
        else if (moveDirectionFloat < -Mathf.Epsilon)
        {
            moveDirection = -1;
        }
        else
        {
            moveDirection = 0;
        }
    }
}
