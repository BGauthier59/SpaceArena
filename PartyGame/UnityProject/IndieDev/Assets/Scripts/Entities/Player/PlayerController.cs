using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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
    }

    private void FixedUpdate()
    {
        Moving();
    }

    #region Input Event

    public void OnMove(InputAction.CallbackContext ctx)
    {
        joystickInput = ctx.ReadValue<Vector2>();
    }

    public void OnPause(InputAction.CallbackContext ctx)
    {
        if (GameManager.instance.isPaused) return;
        SceneManager.LoadSceneAsync(GameManager.instance.pauseMenuIndex, LoadSceneMode.Additive);
        GameManager.instance.SetTimeScale();
    }

    #endregion

    #region Actions

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

    #endregion
    
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
