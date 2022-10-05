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
    private float moveTolerance;

    public override void OnUse()
    {
        moveTolerance = user.moveTolerance;
        playerInput = user.playerInput;
        user.DeactivatePlayer();
        Debug.Log("wormhole");
    }
    
    public void OnMove(InputAction.CallbackContext ctx)
    {
        leftJoystickInput = ctx.ReadValue<Vector2>();
        //if (!isActive) return;

        // Checking conditions
        if (Mathf.Abs(leftJoystickInput.x) < moveTolerance && Mathf.Abs(leftJoystickInput.y) < moveTolerance)
            leftJoystickInput = Vector2.zero;
    }
}
