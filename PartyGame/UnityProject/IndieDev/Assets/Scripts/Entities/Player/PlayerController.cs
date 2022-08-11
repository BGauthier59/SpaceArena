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

    public PlayerManager manager;

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
    [SerializeField] private float dashForce;
    [SerializeField] private float dashDuration;
    private float dashTimer;
    private bool isDashing;

    [Header("Attack")] 
    [SerializeField] private bool isAttacking;
    [SerializeField] private int maxBulletAmount;
    [SerializeField] private int bulletAmount;
    [SerializeField] private float bulletSpeed;
    private float timerBeforeNextShoot;
    [SerializeField] private float durationBeforeNextShoot;

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
        reloadBar.transform.parent.SetParent(GameManager.instance.mainCanvas.transform);
        rd.material.color = GameManager.instance.colors[playerIndex - 1];
        rd.material.SetColor("_EmissionColor", GameManager.instance.colors[playerIndex - 1] * 2);
    }

    #endregion


    private void Update()
    {
        Rotating();
        Reloading();
        Firing();
        Dashing();
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

        if (Mathf.Abs(rightJoystickInput.x) < aimTolerance && Mathf.Abs(rightJoystickInput.y) < aimTolerance)
        {
            aiming = false;
            rightJoystickInput = Vector2.zero;
        }
        else aiming = true;
    }

    public void OnFire(InputAction.CallbackContext ctx)
    {
        isAttacking = ctx.performed;
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

    public void OnDash(InputAction.CallbackContext ctx)
    {
        if (isDashing) return;
        Debug.Log("Je dash");
        isDashing = true;
        var dashVector = new Vector3(leftJoystickInput.x, 0, leftJoystickInput.y);
        dashVector.Normalize();
        rb.AddForce(dashVector * dashForce);
        //StartCoroutine(DashCooldown());
    }

    #endregion

    #region Actions

    private void Moving()
    {
        if (isDashing) return;
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
        else
        {
            var angle = Mathf.Atan2(rightJoystickInput.y, rightJoystickInput.x) * Mathf.Rad2Deg;
            transform.eulerAngles = new Vector3(0, -angle + 90, 0);
        }
    }

    private void Firing()
    {
        if (!isAttacking || reloading) return;

        if (timerBeforeNextShoot >= durationBeforeNextShoot)
        {
            bulletAmount--;
            if (bulletAmount <= 0) reloading = true;

            shootingParticles.Play();
            var newBullet =
                PoolOfObject.Instance.SpawnFromPool(PoolType.Bullet, transform.position, Quaternion.identity);
            newBullet.GetComponent<Rigidbody>().AddForce(transform.forward * bulletSpeed);
            GameManager.instance.cameraShake.AddShakeEvent(shootingShake);
            rb.AddForce(-transform.forward * recoil);
            timerBeforeNextShoot = 0f;
        }
        else
        {
            timerBeforeNextShoot += Time.deltaTime;
        }
    }

    private void Reloading()
    {
        if (reloading)
        {
            reloadBar.transform.parent.gameObject.SetActive(true);
            reloadTimer += Time.deltaTime;
            
            var nextPos = Camera.main.WorldToScreenPoint(transform.position) + new Vector3(0, -20);

            reloadBar.transform.parent.position = Vector3.Lerp(reloadBar.transform.parent.position, nextPos,
                reloadTimer / reloadDuration);
            reloadBar.fillAmount = reloadTimer / reloadDuration;
            
            if (reloadTimer > reloadDuration)
            {
                reloading = false;
                reloadTimer = 0;
                bulletAmount = maxBulletAmount;
                reloadBar.transform.parent.gameObject.SetActive(false);
            }
        }
    }

    private void Dashing()
    {
        if (dashTimer >= dashDuration)
        {
            isDashing = false;
            dashTimer = 0f;
        }
        else dashTimer += Time.deltaTime;
    }

    /*
    private System.Collections.IEnumerator DashCooldown()
    {
        yield return new WaitForSeconds(dashDuration);
        isDashing = false;
    }
    */

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