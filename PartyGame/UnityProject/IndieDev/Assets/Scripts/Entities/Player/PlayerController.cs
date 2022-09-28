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

    [Header("Input, Gamepad & Data")] 
    public int playerIndex;
    public string playerName;
    public PlayerInput playerInput;

    public GamepadData dataGamepad;

    public PlayerManager manager;

    [Header("Party Data")] 
    public int points;

    [Header("Components")] 
    public Renderer rd;
    public Collider col;
    private Rigidbody rb;

    [Header("Controller & Parameters")]
    private bool isActive;
    
    [SerializeField] private Vector2 leftJoystickInput;
    [SerializeField] private Vector2 rightJoystickInput;

    [Range(0f, 1f)] [SerializeField] private float moveTolerance;
    [Range(0f, 1f)] [SerializeField] private float aimTolerance;
    [SerializeField] private float speed;
    private float baseSpeed;
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
    public float bulletSpeed;
    private float timerBeforeNextShoot;
    [SerializeField] private float durationBeforeNextShoot;

    private float reloadTimer;
    private bool reloading;
    [SerializeField] private float reloadDuration;
    [SerializeField] private ParticleSystem shootingParticles;
    [SerializeField] private CameraShakeScriptable shootingShake;
    [SerializeField] private float recoil;
    [SerializeField] private Image reloadBar;

    [Header("Power-Up")]
    [SerializeField] private Slider powerUpGauge;
    [SerializeField] private int powerUpMax;
    private int powerUpScore;

    [Header("Reparation")] 
    public ReparationArea reparationArea;

    [Header("Vent")] 
    public Vent accessibleVent;

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
        rb = manager.rb;
        
        playerInput = GetComponent<PlayerInput>();
        dataGamepad = new GamepadData()
        {
            gamepad = playerInput.GetDevice<Gamepad>(),
            isMotorActive = false
        };

        GameManager.instance.feedbacks.RumbleConstant(dataGamepad, VibrationsType.Connection);
        rb.isKinematic = true;
        
        
        DeactivatePlayer();
    }

    #endregion

    #region Party Initialization

    public void PartyBegins()
    {
        rb.isKinematic = false;
        bulletAmount = maxBulletAmount;
        
        baseSpeed = speed;
        powerUpGauge.maxValue = powerUpMax;
        powerUpScore = 0;
        
        reloadBar.transform.parent.SetParent(GameManager.instance.mainCanvas.transform);
        powerUpGauge.transform.SetParent(GameManager.instance.mainCanvas.transform);

        rd.material.color = GameManager.instance.colors[playerIndex - 1];
        rd.material.SetColor("_EmissionColor", GameManager.instance.colors[playerIndex - 1] * 2);
    }

    public void ActivatePlayer()
    {
        if (GameManager.instance.partyManager.gameState == PartyManager.GameState.Finished || 
            GameManager.instance.partyManager.gameState == PartyManager.GameState.End)
        {
            Debug.LogWarning("Tried to active player after the end of game.");
            return;
        }
        isActive = true;
    }

    public void DeactivatePlayer()
    {
        rb.velocity = Vector3.zero;
        isActive = false;
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
        SettingPowerUpGauge();
    }
    
    #region Input Event

    public void OnMove(InputAction.CallbackContext ctx)
    {
        if (!isActive) return;
        
        leftJoystickInput = ctx.ReadValue<Vector2>();

        // Checking conditions
        if (Mathf.Abs(leftJoystickInput.x) < moveTolerance && Mathf.Abs(leftJoystickInput.y) < moveTolerance)
            leftJoystickInput = Vector2.zero;
    }

    public void OnPause(InputAction.CallbackContext ctx)
    {
        if (!isActive) return;

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
        if (!isActive) return;

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
        if (!isActive) return;

        isAttacking = ctx.performed;
    }

    public void OnReload(InputAction.CallbackContext ctx)
    {
        if (!isActive) return;

        reloading = true;
        reloadBar.transform.parent.position = Camera.main.WorldToScreenPoint(transform.position) 
                                              + new Vector3(0, -20);
        reloadBar.transform.parent.gameObject.SetActive(true);
    }

    public void OnRepairing(InputAction.CallbackContext ctx)
    {
        if (!isActive) return;

        if (reparationArea == null) return;
        if (!reparationArea.isWaitingForInput) return;
        
        GameManager.instance.feedbacks.RumbleConstant(dataGamepad, VibrationsType.Reparation);
        
        reparationArea.associatedElement.SetCheckingArea();
    }

    public void OnDash(InputAction.CallbackContext ctx)
    {
        if (!isActive) return;

        if (isDashing) return;
        Debug.Log("Je dash");
        isDashing = true;
        var dashVector = new Vector3(leftJoystickInput.x, 0, leftJoystickInput.y);
        dashVector.Normalize();
        rb.AddForce(dashVector * dashForce);
    }

    public void OnEnterVent(InputAction.CallbackContext ctx)
    {
        if (!isActive) return;
        if (!accessibleVent) return;
        
        accessibleVent.PlayerEnters(this);

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
            var bullet = newBullet.GetComponent<BulletScript>();
            bullet.shooter = manager;
            bullet.rb.AddForce(transform.forward * bulletSpeed);
            
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
            if (reloadTimer > reloadDuration)
            {
                reloading = false;
                reloadTimer = 0;
                bulletAmount = maxBulletAmount;
                reloadBar.transform.parent.gameObject.SetActive(false);
            }
            else
            {
                var nextPos = Camera.main.WorldToScreenPoint(transform.position) 
                              + new Vector3(0, -20);

                reloadBar.transform.parent.position = Vector3.Lerp(reloadBar.transform.parent.position, nextPos,
                    reloadTimer / reloadDuration);
                reloadBar.fillAmount = reloadTimer / reloadDuration;
                
                reloadTimer += Time.deltaTime;
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
    
    private void SettingPowerUpGauge()
    {
        var nextPos = Camera.main.WorldToScreenPoint(transform.position) 
                      + new Vector3(40, 0);
        //powerUpGauge.transform.position = Vector3.Lerp(powerUpGauge.transform.position, nextPos, Time.deltaTime);
        powerUpGauge.transform.position = nextPos;
    }

    #endregion

    #region Modification

    public void ModifySpeed(float factor)
    {
        var newSpeed = speed * factor;
        speed = newSpeed;
    }

    public void ResetSpeed()
    {
        speed = baseSpeed;
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