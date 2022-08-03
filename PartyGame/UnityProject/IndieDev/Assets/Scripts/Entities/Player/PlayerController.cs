using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    #region Variables

    [Header("Input, Gamepad & Data")] public int playerIndex;
    public string playerName;
    public PlayerInput playerInput;

    public GamepadData dataGamepad;

    [SerializeField] private PlayerManager manager;

    [Header("Party Data")] 
    public int points;

    [Header("Components")] public Renderer rd;
    [SerializeField] private Rigidbody rb;

    [Header("Controller & Parameters")] 
    [SerializeField] private Vector2 joystickInput;

    [Range(0f, 1f)] [SerializeField] private float moveTolerance;
    [SerializeField] private float speed;
    [SerializeField] private float rotateSpeed;

    #endregion

    #region Connection

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Initialization();
    }

    private void Initialization()
    {
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

    #endregion

    #region Party Initialization

    public void PartyBegins()
    {
        rb.isKinematic = false;
    }

    #endregion

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
        
        // Checking conditions
        if (Mathf.Abs(joystickInput.x) < moveTolerance && Mathf.Abs(joystickInput.y) < moveTolerance) joystickInput = Vector2.zero;
    }

    public void OnPause(InputAction.CallbackContext ctx)
    {
        // Checking conditions
        if (GameManager.instance.isPaused) return;
        
        // Pausing the game
        GameManager.instance.isPaused = true;
        GameManager.instance.SetMainGamepad(dataGamepad.gamepad);
        GameManager.instance.eventSystem.GetComponent<InputSystemUIInputModule>().actionsAsset = playerInput.actions;
        SceneManager.LoadSceneAsync(GameManager.instance.pauseMenuIndex, LoadSceneMode.Additive);
    }

    public void OnAttack(InputAction.CallbackContext ctx)
    {
        // Checking conditions
        
        // If true, attacks
    }

    public void OnRepairing(InputAction.CallbackContext ctx)
    {
        // Checking conditions
        
        // If true, repairs damages buildings
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
        transform.rotation =
            Quaternion.Lerp(rb.rotation, Quaternion.LookRotation(forward), Time.deltaTime * rotateSpeed);
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