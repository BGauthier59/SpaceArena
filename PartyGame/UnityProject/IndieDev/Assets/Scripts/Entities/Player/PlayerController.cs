using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public PlayerInput playerInput;
    private int playerIndex;
    public string playerName;
    public int points;
    public Renderer rd;
    [SerializeField] private Rigidbody rb;
    
    [SerializeField] private Vector2 joystickInput;
    [SerializeField] private float speed;
    [SerializeField] private float rotateSpeed;

    [SerializeField] private PlayerManager manager;

    public GamepadData dataGamepad;
    
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        GameManager.instance.allPlayers.Add(this);
        playerIndex = GameManager.instance.playerInputManager.playerCount;
        playerName = $"Player {playerIndex}";
        manager = GetComponent<PlayerManager>();
        playerInput = GetComponent<PlayerInput>();

        dataGamepad = new GamepadData()
        {
            gamepad = playerInput.GetDevice<Gamepad>(),
            isMotorActive = false
        };
        
        GameManager.instance.feedbacks.RumbleConstant(dataGamepad, VibrationsType.Connection);
        rb.isKinematic = true;
        
    }

    public void PartyBegins()
    {
        rb.isKinematic = false;
    }

    private void Update()
    {
        Rotating();
        Debug.DrawRay(transform.position, transform.forward * 2, Color.blue);
    }

    private void FixedUpdate()
    {
        Moving();
    }

    private void Moving()
    {
        var moveVector = new Vector3(joystickInput.x, 0, joystickInput.y);
        rb.velocity = moveVector * speed * Time.fixedDeltaTime;
    }

    private void Rotating()
    {
        var forward = new Vector3(joystickInput.x, 0, joystickInput.y);
        if (forward == Vector3.zero) return;
        transform.rotation = Quaternion.Lerp(rb.rotation, Quaternion.LookRotation(forward), Time.deltaTime * rotateSpeed);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        joystickInput = ctx.ReadValue<Vector2>();
    }
}

[Serializable]
public class GamepadData
{
    public Gamepad gamepad;
    public bool isRumbling;
    
    public RumblePattern activeRumblePattern;
    public float rumbleDuration;
    public float pulseDuration;
    public float lowA;
    public float highA;
    public float rumbleStep;
    public bool isMotorActive;
}
