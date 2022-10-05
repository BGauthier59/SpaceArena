using System;
using UnityEditor.Experimental;
using UnityEngine;
using UnityEngine.InputSystem;

public class WormholeBehaviour : PowerUpManager
{
    [SerializeField] private GameObject teleportFX;
    [SerializeField] private float teleportRange;
    [SerializeField] private int damage;
    [SerializeField] private float impactRange;
    [SerializeField] private float moveSpeed;
    private PlayerInput playerInput;
    private Vector2 leftJoystickInput;
    [Range(0f, 1f)] private float moveTolerance;
    private Vector3 moveVector;

    public override void OnUse()
    {
        moveTolerance = 0.15f;
        playerInput = user.playerInput;
        user.DeactivatePlayer();
    }

    private void FixedUpdate()
    {
        Debug.Log("L'input "+ moveVector);
        Moving();
    }
    
    public void OnMove(InputAction.CallbackContext ctx)
    {
        leftJoystickInput = ctx.ReadValue<Vector2>();
        //if (!isActive) return;

        // Checking conditions
        if (Mathf.Abs(leftJoystickInput.x) < moveTolerance && Mathf.Abs(leftJoystickInput.y) < moveTolerance)
            leftJoystickInput = Vector2.zero;
        Debug.Log(leftJoystickInput);
        moveVector = new Vector3(leftJoystickInput.x, 0, leftJoystickInput.y);
    }
    private void Moving()
    {
        transform.Translate(moveVector * (moveSpeed * Time.fixedDeltaTime));
    }

    
}
