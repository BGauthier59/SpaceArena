using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    #region Variables

    [Header("Input, Gamepad & Data")] public int playerIndex;
    public string playerName;
    public PlayerInput playerInput;

    public GamepadData dataGamepad;

    [SerializeField] private PlayerManager manager;

    [Header("Party Data")] public int points;

    [Header("Components")] public Renderer rd;
    [SerializeField] private Rigidbody rb;

    [Header("Controller & Parameters")] [SerializeField]
    private Vector2 leftJoystickInput;

    [SerializeField] private Vector2 rightJoystickInput;

    [Range(0f, 1f)] [SerializeField] private float moveTolerance;
    [Range(0f, 1f)] [SerializeField] private float aimTolerance;
    [SerializeField] private float speed;
    [SerializeField] private float rotateSpeed;
    private bool aiming;

    [Header("Attack")] [SerializeField] private int maxBulletAmount;
    [SerializeField] private int bulletAmount;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float fireRate;
    private float lastFire;
    private float reloadTimer;
    private bool reloading;
    [SerializeField] private float reloadDuration;
    [SerializeField] private ParticleSystem shootingParticles;
    [SerializeField] private CameraShakeScriptable shootingShake;
    [SerializeField] private float recoil;
    [SerializeField] private Image reloadBar;

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
        bulletAmount = maxBulletAmount;
        reloadBar.transform.parent = GameManager.instance.mainCanvas.transform;
    }

    #endregion


    private void Update()
    {
        Rotating();
        Aiming();
        Reloading();
    }

    private void FixedUpdate()
    {
        Moving();
    }

    #region Input Event

    public void OnMove(InputAction.CallbackContext ctx)
    {
        leftJoystickInput = ctx.ReadValue<Vector2>();

        // Checking conditions
        if (Mathf.Abs(leftJoystickInput.x) < moveTolerance && Mathf.Abs(leftJoystickInput.y) < moveTolerance)
            leftJoystickInput = Vector2.zero;
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

    public void OnAim(InputAction.CallbackContext ctx)
    {
        rightJoystickInput = ctx.ReadValue<Vector2>();

        if (Mathf.Abs(leftJoystickInput.x) < aimTolerance && Mathf.Abs(leftJoystickInput.y) < aimTolerance)
        {
            aiming = false;
            leftJoystickInput = Vector2.zero;
        }
        else
        {
            aiming = true;
        }
    }

    public void OnAttack(InputAction.CallbackContext ctx)
    {
        // Checking conditions
        if (Time.time >= lastFire + fireRate && !reloading)
        {
            bulletAmount--;
            if (bulletAmount == 0)
            {
                reloadTimer = 0;
                reloading = true;
            }

            shootingParticles.Play();
            lastFire = Time.time;
            var newBullet = PoolOfObject.Instance.SpawnFromPool("Bullets", transform.position, Quaternion.identity);
            newBullet.GetComponent<Rigidbody>().AddForce(transform.forward * bulletSpeed);
            GameManager.instance.cameraShake.AddShakeEvent(shootingShake);
            rb.AddForce(-transform.forward * recoil);
        }
    }

    public void OnReload(InputAction.CallbackContext ctx)
    {
        reloading = true;
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
        var moveVector = new Vector3(leftJoystickInput.x, 0, leftJoystickInput.y);
        rb.velocity = moveVector * speed * Time.fixedDeltaTime;
    }

    private void Rotating()
    {
        if (!aiming)
        {
            var forward = new Vector3(leftJoystickInput.x, 0, leftJoystickInput.y);
            if (forward == Vector3.zero) return;
            transform.rotation =
                Quaternion.Lerp(rb.rotation, Quaternion.LookRotation(forward), Time.deltaTime * rotateSpeed);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }
    }

    private void Aiming()
    {
        float angle = Mathf.Atan2(rightJoystickInput.y, rightJoystickInput.x) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(0, -angle + 90, 0);
    }

    private void Reloading()
    {
        if (reloading)
        {
            reloadBar.transform.parent.gameObject.SetActive(true);
            reloadTimer += Time.deltaTime;
            reloadBar.transform.parent.position =
                Camera.main.WorldToScreenPoint(transform.position) + new Vector3(0, -20);
            reloadBar.fillAmount = reloadTimer / reloadDuration;
            if (reloadTimer > reloadDuration)
            {
                reloading = false;
                bulletAmount = maxBulletAmount;
                reloadBar.transform.parent.gameObject.SetActive(false);
            }
        }
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