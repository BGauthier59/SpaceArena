using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
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
    public Vector3 initPos;

    [Header("Components")] public Renderer rd;
    public Collider col;
    private Rigidbody rb;

    [Header("Controller & Parameters")] private bool isActive;
    [SerializeField] private Vector2 leftJoystickInput;
    [SerializeField] private Vector2 rightJoystickInput;

    [Range(0f, 1f)] public float moveTolerance;
    [Range(0f, 1f)] [SerializeField] private float aimTolerance;
    [SerializeField] private float speed;
    private float baseSpeed;
    [SerializeField] private float rotateSpeed;
    private bool aiming;
    [SerializeField] private AnimationCurve dashFactor;
    [SerializeField] private float dashDuration;
    private float dashTimer;
    private bool isDashing;

    [Header("Attack")] [SerializeField] private bool isAttacking;
    [SerializeField] private int maxBulletAmount;
    [SerializeField] private int bulletAmount;
    public float bulletSpeed;
    private float timerBeforeNextShoot;
    public float durationBeforeNextShoot;

    private float reloadTimer;
    private bool reloading;
    [SerializeField] private float reloadDuration;

    [SerializeField] private float autoReloadDuration;
    private float autoReloadTimer;
    private bool isAutoReloading;

    [SerializeField] private ParticleSystem shootingParticles;
    [SerializeField] private CameraShakeScriptable shootingShake;
    [SerializeField] private float recoil;
    [SerializeField] private Slider reloadGauge;

    [Header("Power-Up")] [SerializeField] private float setGaugeSpeed;
    public bool powerUpIsActive;
    [SerializeField] private Slider powerUpGauge;
    [SerializeField] private int powerUpMax;
    [SerializeField] private PowerUpManager currentPowerUp;
    public int powerUpScore;
    private bool canUsePowerUp = false;

    [Header("Reparation")] public ReparationArea reparationArea;

    [Header("Vent")] public NewVent lastTakenNewVent;
    public NewVent accessibleNewVent;
    public NewConduit currentConduit;
    public bool detectInputConduit;
    public bool isVentingOut;
    [SerializeField] private float afterVentingSecurityDuration;
    private float afterVentingSecurityTimer;

    [Header("Graph")] public SpriteRenderer directionArrow;
    [SerializeField] private ParticleSystemRenderer particleSystem;
    [SerializeField] private TrailRenderer trail;
    public Light playerLight;

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
        baseSpeed = speed;

        bulletAmount = maxBulletAmount;
        reloadGauge.maxValue = maxBulletAmount;
        reloadGauge.value = reloadGauge.maxValue;
        reloadGauge.transform.SetParent(GameManager.instance.partyManager.mainCanvas.transform);
        reloadGauge.transform.position = Camera.main.WorldToScreenPoint(transform.position)
                                         + new Vector3(0, -30);
        powerUpScore = 0;
        powerUpGauge.maxValue = powerUpMax;
        powerUpGauge.value = powerUpGauge.minValue;
        powerUpGauge.transform.SetParent(GameManager.instance.partyManager.mainCanvas.transform);
        powerUpGauge.transform.position = Camera.main.WorldToScreenPoint(transform.position)
                                          + new Vector3(50, 0);

        SetGaugesState(false);
        GraphInitialization();
    }

    public void GraphInitialization()
    {
        var material = rd.material;
        material.color = GameManager.instance.colors[playerIndex - 1];
        material.SetColor("_EmissionColor", GameManager.instance.colors[playerIndex - 1] * 2);
        material.EnableKeyword("_EMISSION");
        particleSystem.material = directionArrow.material = rd.material = trail.material = material;
    }

    public void ActivatePlayer()
    {
        if (GameManager.instance.partyManager.gameState == PartyManager.GameState.End)
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
        SetTimerAfterVenting();

        if (!isActive) return;

        Rotating();
        AutoReloading();
        Reloading();
        Firing();
        Dashing();
        PowerCheck();
    }

    private void FixedUpdate()
    {
        MovingInConduit();

        if (!isActive) return;

        Moving();
        SettingPowerUpGauge();
        SettingReloadGauge();
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
        else aiming = ctx.performed;
    }

    public void OnFire(InputAction.CallbackContext ctx)
    {
        if (!isActive) return;

        isAttacking = ctx.performed;
    }

    public void OnRepair(InputAction.CallbackContext ctx)
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

        isDashing = ctx.performed;
    }

    public void OnUsePowerUp(InputAction.CallbackContext ctx)
    {
        if (!isActive) return;
        if (!canUsePowerUp || powerUpIsActive) return;
        if (currentPowerUp != null)
        {
            powerUpIsActive = true;
            currentPowerUp.user = this;
            currentPowerUp.OnActivate();
        }

        //EndOfPowerUp(); // Pour le moment
    }

    #endregion

    #region Actions

    private void Moving()
    {
        var moveVector = new Vector3(leftJoystickInput.x, 0, leftJoystickInput.y);
        rb.velocity = moveVector * (speed * Time.fixedDeltaTime);
    }

    private void MovingInConduit()
    {
        if (currentConduit == null) return;
        if (!detectInputConduit) return;

        var moveVector = new Vector2(leftJoystickInput.x, leftJoystickInput.y);
        var dir = NewConduit.ConduitDirection.NA;

        if (moveVector.x > .5f) dir = NewConduit.ConduitDirection.Right;
        else if (moveVector.x < -.5f) dir = NewConduit.ConduitDirection.Left;
        else if (moveVector.y > .5f) dir = NewConduit.ConduitDirection.Up;
        else if (moveVector.y < -.5f) dir = NewConduit.ConduitDirection.Bottom;

        if (dir == NewConduit.ConduitDirection.NA) return;
        currentConduit.SetNextPoint(dir);
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
                if (!powerUpIsActive)
                {
                    bulletAmount--;

                    shootingParticles.Play();
                    var newBullet =
                        PoolOfObject.Instance.SpawnFromPool(PoolType.Bullet, transform.position, Quaternion.identity);
                    var bullet = newBullet.GetComponent<BulletScript>();
                    bullet.shooter = manager;
                    bullet.rb.AddForce(transform.forward * bulletSpeed);

                    GameManager.instance.partyManager.cameraShake.AddShakeEvent(shootingShake);
                    GameManager.instance.feedbacks.RumbleConstant(dataGamepad, VibrationsType.Shoot);

                    rb.AddForce(-transform.forward * recoil);
                    timerBeforeNextShoot = 0f;

                    reloadGauge.value = bulletAmount;

                    if (bulletAmount <= 0)
                    {
                        isAutoReloading = false;
                        reloading = true;
                    }
                    else
                    {
                        isAutoReloading = true;
                        autoReloadTimer = 0f;
                    }
                }
                else
                {
                    currentPowerUp.OnUse();
                    timerBeforeNextShoot = 0f;
                }
            }
            else
            {
                timerBeforeNextShoot += Time.deltaTime;
            }
        
    }

    private void Reloading()
    {
        if (!reloading) return;

        if (reloadTimer >= reloadDuration)
        {
            reloading = false;
            reloadTimer = 0f;
            bulletAmount = maxBulletAmount;
        }
        else
        {
            reloadGauge.value = (reloadTimer / reloadDuration) * reloadGauge.maxValue;
            reloadTimer += Time.deltaTime;
        }
    }

    private void AutoReloading()
    {
        if (!isAutoReloading) return;

        if (autoReloadTimer >= autoReloadDuration)
        {
            autoReloadTimer = 0f;
            reloading = true;
            reloadTimer = (bulletAmount / (float) maxBulletAmount) * reloadDuration;
            isAutoReloading = false;
        }
        else
        {
            autoReloadTimer += Time.deltaTime;
        }
    }

    private void Dashing()
    {
        if (!isDashing) return;

        if (accessibleNewVent)
        {
            if (isVentingOut && accessibleNewVent == lastTakenNewVent) return;

            CancelDash();
            accessibleNewVent.EntersVent(this);
            return;
        }

        if (dashTimer >= dashDuration)
        {
            CancelDash();
        }
        else
        {
            dashTimer += Time.deltaTime;
            var dashSpeed = dashFactor.Evaluate(dashTimer / dashDuration);
            ModifySpeed(dashSpeed);
        }
    }

    private void SetTimerAfterVenting()
    {
        if (!isVentingOut) return;

        if (afterVentingSecurityTimer > afterVentingSecurityDuration)
        {
            afterVentingSecurityTimer = 0f;
            isVentingOut = false;
        }
        else afterVentingSecurityTimer += Time.deltaTime;
    }

    private void SettingPowerUpGauge()
    {
        var nextPos = Camera.main.WorldToScreenPoint(transform.position)
                      + new Vector3(50, 0);
        powerUpGauge.transform.position =
            Vector3.Lerp(powerUpGauge.transform.position, nextPos, Time.fixedDeltaTime * setGaugeSpeed);
        //powerUpGauge.transform.position = nextPos;
    }

    private void SettingReloadGauge()
    {
        var nextPos = Camera.main.WorldToScreenPoint(transform.position)
                      + new Vector3(0, -30);

        reloadGauge.transform.position =
            Vector3.Lerp(reloadGauge.transform.position, nextPos, Time.fixedDeltaTime * setGaugeSpeed);
        //reloadGauge.transform.position = nextPos;
    }

    private void PowerCheck()
    {
        if (!powerUpIsActive) return;

        if (currentPowerUp.OnConditionCheck()) EndOfPowerUp();
    }

    #endregion

    #region Modification

    public void ModifySpeed(float factor)
    {
        var newSpeed = baseSpeed * factor;
        speed = newSpeed;
    }

    public void ResetSpeed()
    {
        speed = baseSpeed;
    }

    public void IncreasePowerUpGauge(int value)
    {
        Debug.Log("Le power up");
        if (canUsePowerUp) return;
        powerUpScore = Mathf.Min(powerUpMax, powerUpScore += value);
        powerUpGauge.value = powerUpScore;
        if (powerUpGauge.value >= powerUpMax)
        {
            GetPowerUp();
        }
    }

    private void GetPowerUp()
    {
        canUsePowerUp = true;
        currentPowerUp = PowerUpList.powerUpScript[6];
        powerUpGauge.value = 0;
        // Get power up
    }

    public void EndOfPowerUp()
    {
        powerUpScore = 0;
        powerUpGauge.value = powerUpScore;
        canUsePowerUp = false;
        powerUpIsActive = false;
    }

    public void CancelDash()
    {
        if (!isDashing) return;
        isDashing = false;
        ResetSpeed();
        dashTimer = 0f;
    }

    public void SetGaugesState(bool active)
    {
        reloadGauge.gameObject.SetActive(active);
        powerUpGauge.gameObject.SetActive(active);
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