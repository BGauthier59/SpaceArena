using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

public class PlayerController : MonoBehaviour
{
    #region Variables

    [Header("Id")] [Space(5)] [Header("DATA")]
    public int playerIndex;

    public string playerName;

    [Space(3)] [Header("Components")] public PlayerManager manager;
    public PlayerInput playerInput;
    public Renderer rd;
    public CapsuleCollider col;
    public Rigidbody rb;
    public GamepadData dataGamepad;
    public RageScript rageCollider;

    [Space(3)] [Header("UI")] public PartyManager.PlayerUI playerUI;

    [SerializeField] private Sprite defaultPowerUpImage;
    [SerializeField] private ParticleSystem playerFire;

    [Space(3)] [Header("Renderer")] public SpriteRenderer directionArrow;
    [SerializeField] private ParticleSystemRenderer particleSystem;
    [SerializeField] private TrailRenderer trail;
    public Light playerLight;
    [SerializeField] private ParticleSystem shootingParticles;
    public GameObject crown;
    private PartyManager partyManager;
    [SerializeField] private VentingPlayerMesh ventingPlayerMesh;

    [Serializable]
    public struct VentingPlayerMesh
    {
        public GameObject obj;
        public TrailRenderer trailRd;
        public MeshRenderer rd;
    }

    [Space(3)] [Header("Feedbacks")] [SerializeField]
    private CameraShakeScriptable shootingShake;

    private RigidbodyConstraints defaultConstraints;
    private float baseSpeed;
    private float baseShootCooldown;
    private int bulletAmount;

    [Space(3)] [Header("Booleans")] [SerializeField]
    private bool isActive;

    [SerializeField] public bool isActiveVent;
    [SerializeField] public bool isActiveControllableTurret;
    [Space(3)] private bool isAttacking;
    private bool aiming;
    private bool isDashing;
    private bool grounded;
    private bool reloading;
    private bool isAutoReloading;
    public bool helpingAimSet;
    private bool isVentingOut;
    public bool detectInputConduit;
    private bool leavingTurret;
    public bool powerUpIsActive;
    private bool canUsePowerUp;

    [Space(3)] [Header("Joysticks inputs")] [SerializeField]
    private Vector2 leftJoystickInput;

    [SerializeField] private Vector2 rightJoystickInput;

    [Space(3)] [Header("References")] public Vector3 initPos;
    public Vector3 helpingAimDirection;
    private PowerUpManager currentPowerUp;
    private ReparationArea reparationArea;
    private NewVent lastTakenNewVent;
    private NewVent accessibleNewVent;
    private NewConduit currentConduit;
    private ControllableTurret accessibleControllableTurret;
    private ControllableTurret currentControllableTurret;

    [Space(5)] [Header("Durations")] [Header("PARAMETERS")] [SerializeField]
    private float dashDuration;

    public float shootCooldownDuration;
    [SerializeField] private float reloadDuration;
    [SerializeField] private float autoReloadDuration;
    [SerializeField] private float ventingCooldownDuration;
    [SerializeField] private float turretCooldownDuration;

    private float dashTimer;
    private float shootCooldownTimer;
    private float reloadTimer;
    private float autoReloadTimer;
    private float ventingCooldownTimer;
    private float turretCooldownTimer;

    [Space(3)] [Header("Move parameters")] [SerializeField] [Range(0f, 1f)]
    public float moveTolerance;

    [SerializeField] private float speed;

    [Space(3)] [Header("Aim parameters")] [Range(0f, 1f)] [SerializeField]
    private float aimTolerance;

    [SerializeField] private float rotateSpeed;
    [SerializeField] private float helpingAimMaxDistance;
    [SerializeField] private LayerMask enemyTargetLayer;

    [Space(3)] [Header("Dash parameters")] [SerializeField]
    private AnimationCurve dashFactor;

    [Space(3)] [Header("Attack parameters")] [SerializeField]
    private int maxBulletAmount;

    public float bulletSpeed;
    [SerializeField] private float recoil;
    public Transform bulletOrigin;

    [Space(3)] [Header("Gravity parameters")] [SerializeField]
    private float fallSpeed;

    [SerializeField] private LayerMask groundLayer;

    [Space(3)] [Header("Power-up parameters")] [SerializeField]
    private float powerUpMax;

    private int lastPowerUpIndex;

    public float powerUpScore;

    [Space(3)] [Header("GUI parameters")] [SerializeField]
    private float setGaugeSpeed;

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
        defaultConstraints = rb.constraints;
        trail.enabled = false;
        lastPowerUpIndex = -1;

        DeactivatePlayer();
    }

    private void LinkReferences()
    {
        partyManager = GameManager.instance.partyManager;
        manager.LinkReferences();
    }

    #endregion

    #region Game Initialization

    public void PartyBegins()
    {
        LinkReferences();

        rb.isKinematic = false;
        baseSpeed = speed;
        baseShootCooldown = shootCooldownDuration;

        //reloadGauge.transform.SetParent(partyManager.mainCanvas.transform);
        //powerUpGauge.transform.SetParent(partyManager.mainCanvas.transform);

        ResetGauges();
        SetGaugesState(false);
        GraphInitialization();
        ResetPlayer();
        ResetPlayerGraphsAndCollisions();
    }

    public void GraphInitialization()
    {
        var material = rd.material;
        var color = GameManager.instance.colors[playerIndex - 1];
        playerUI.powerUpFire.startColor = playerUI.powerUpSlider.color = playerUI.powerUpSparks.startColor =
            playerUI.lifeSliderColor.color =
                playerUI.reloadSliderColor.color = playerUI.powerUpBurst.startColor = color;

        material.color = color;
        material.SetColor("_EmissionColor", GameManager.instance.colors[playerIndex - 1] * 1);
        material.EnableKeyword("_EMISSION");
        particleSystem.material = directionArrow.material = rd.material =
            trail.material = ventingPlayerMesh.rd.material = ventingPlayerMesh.trailRd.material = material;
        crown.SetActive(false);
        ventingPlayerMesh.rd.gameObject.SetActive(false);
        ventingPlayerMesh.trailRd.enabled = false;
    }

    #endregion

    #region Updates & FixedUpdates

    private void Update()
    {
        if (!isActive) return;
        UpdateClassic();
        UpdateControllableTurret();
    } // Main Update

    private void UpdateClassic()
    {
        if (isActiveVent || isActiveControllableTurret) return;

        SetTimerAfterVenting();
        SetTimerAfterLeavingControllableTurret();
        Grounding();
        Rotating();
        AutoReloading();
        Reloading();
        Firing();
        FireReload();
        Dashing();
        PowerCheck();
    } // Calls methods when the player is on a classic state

    private void UpdateControllableTurret()
    {
        if (!isActiveControllableTurret) return;
        MovingInTurret();
        Firing();
        FireReload();
    } // Calls methods when the player is controlling a turret

    private void FixedUpdate()
    {
        if (!isActive) return;
        FixedUpdateClassic();
        FixedUpdateVent();
    } // Main FixedUpdate

    private void FixedUpdateClassic()
    {
        if (isActiveVent || isActiveControllableTurret) return;

        Moving();
        //SettingPowerUpGauge();
        //SettingReloadGauge();
    } // Calls methods when the player is on a classic state

    private void FixedUpdateVent()
    {
        if (!isActiveVent) return;
        MovingInConduit();
    } // Calls methods when the player is venting

    #endregion

    #region Input Event

    public void OnMove(InputAction.CallbackContext ctx)
    {
        // Can't be used if is not active
        if (!isActive) return;

        leftJoystickInput = ctx.ReadValue<Vector2>();

        if (Mathf.Abs(leftJoystickInput.x) < moveTolerance && Mathf.Abs(leftJoystickInput.y) < moveTolerance)
            leftJoystickInput = Vector2.zero;
    }

    public void OnPause(InputAction.CallbackContext ctx)
    {
        // Can't be used if is not active
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
        // Can't be used if is not active, or if player is venting or controlling a turret
        if (!isActive) return;
        if (isActiveVent || isActiveControllableTurret) return;

        rightJoystickInput = ctx.ReadValue<Vector2>();

        // Checking conditions
        if (Mathf.Abs(rightJoystickInput.x) < aimTolerance && Mathf.Abs(rightJoystickInput.y) < aimTolerance)
        {
            aiming = false;
            rightJoystickInput = Vector2.zero;
        }
        else aiming = ctx.performed;
    }

    public void OnFire(InputAction.CallbackContext ctx)
    {
        // Can't be used if is not active, or if player is venting
        if (!isActive) return;
        if (isActiveVent) return;

        isAttacking = ctx.performed;
    }

    public void OnRepair(InputAction.CallbackContext ctx)
    {
        // Can't be used if is not active, or if player is venting or controlling a turret
        if (!isActive) return;
        if (isActiveVent || isActiveControllableTurret) return;

        if (!ctx.performed) return;
        if (reparationArea == null) return;
        if (!reparationArea.isWaitingForInput)
        {
            Debug.Log("You failed reparation");
            //reparationArea.associatedElement.CancelReparation();
            return;
        }

        GameManager.instance.feedbacks.RumbleConstant(dataGamepad, VibrationsType.Reparation);

        reparationArea.associatedElement.SetCheckingArea();
    }

    public void OnDash(InputAction.CallbackContext ctx)
    {
        // Can make a player leaves a turret if is inside
        if (isActiveControllableTurret && ctx.performed && currentControllableTurret)
        {
            leavingTurret = true;
            currentControllableTurret.OnPlayerExits();
            return;
        }

        // Can't be used if is not active, or if player is venting
        if (!isActive) return;
        if (isActiveVent) return;

        if (isDashing) return;

        isDashing = ctx.performed;
    }

    public void OnUsePowerUp(InputAction.CallbackContext ctx)
    {
        // Can't be used if is not active, or if player is venting or controlling a turret
        if (!isActive) return;
        if (isActiveVent || isActiveControllableTurret) return;

        // Can't be used if there's no power up available or already using one
        if (!canUsePowerUp || powerUpIsActive) return;
        if (currentPowerUp == null) return;

        // Feedbacks activation
        currentPowerUp.OnActivate(this);
    }

    #endregion

    #region Actions

    private void Moving()
    {
        if (!grounded) return;

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

    private void MovingInTurret()
    {
        if (currentControllableTurret == null) return;
        var moveVector = new Vector2(leftJoystickInput.x, leftJoystickInput.y);
        currentControllableTurret.Rotating(moveVector);
        HelpingAim();
    }

    private void Grounding()
    {
        if (Physics.Raycast(transform.position, Vector3.down, (col.height / 2 + 0.1f), groundLayer))
        {
            rb.constraints = defaultConstraints;
            grounded = true;
        }
        else
        {
            rb.constraints = RigidbodyConstraints.None;
            rb.position += Vector3.up * (-fallSpeed * Time.deltaTime);
            grounded = false;
        }
    }

    private void Rotating()
    {
        if (!aiming)
        {
            var forward = new Vector3(leftJoystickInput.x, 0, leftJoystickInput.y);
            if (forward == Vector3.zero) return;
            transform.rotation =
                Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(forward), Time.deltaTime * rotateSpeed);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }
        else
        {
            var angle = Mathf.Atan2(rightJoystickInput.y, rightJoystickInput.x) * Mathf.Rad2Deg;
            transform.eulerAngles = new Vector3(0, -angle + 90, 0);
            HelpingAim();
        }
    }

    private void HelpingAim()
    {
        RaycastHit hit;
        var aimDistance = isActiveControllableTurret ? currentControllableTurret.aimMaxDistance : helpingAimMaxDistance;
        if (Physics.Raycast(transform.position, transform.forward, out hit, aimDistance, enemyTargetLayer))
        {
            helpingAimDirection = hit.transform.position - transform.position;
            helpingAimSet = true;
        }
        else
        {
            helpingAimDirection = Vector3.zero;
            helpingAimSet = false;
        }
    }

    private void Firing()
    {
        if (!isAttacking || reloading) return;
        if (shootCooldownTimer >= shootCooldownDuration)
        {
            if (!powerUpIsActive)
            {
                if (isActiveControllableTurret)
                {
                    if (!currentControllableTurret)
                    {
                        Debug.LogWarning("No controllable turret found ?");
                        return;
                    }

                    if (currentControllableTurret.needToReload) return;

                    currentControllableTurret.Shoot();
                    partyManager.cameraShake.AddShakeEvent(shootingShake);
                    GameManager.instance.feedbacks.RumbleConstant(dataGamepad, VibrationsType.ControllableTurretShoot);
                    shootCooldownTimer = 0f;
                }
                else
                {
                    bulletAmount--;
                    shootingParticles.Play();
                    var bullet = PoolOfObject.Instance
                        .SpawnFromPool(PoolType.Bullet, bulletOrigin.position, transform.rotation)
                        .GetComponent<BulletScript>();
                    bullet.shooter = manager;

                    if (helpingAimSet) bullet.rb.AddForce(helpingAimDirection.normalized * bulletSpeed);
                    else bullet.rb.AddForce(transform.forward * bulletSpeed);

                    partyManager.cameraShake.AddShakeEvent(shootingShake);
                    GameManager.instance.feedbacks.RumbleConstant(dataGamepad, VibrationsType.Shoot);

                    rb.AddForce(-transform.forward * recoil);
                    shootCooldownTimer = 0f;
                    playerUI.reloadSlider.value = bulletAmount;

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
            }
            else
            {
                currentPowerUp.OnUse();
                shootCooldownTimer = 0f;
            }
        }
    }

    private void FireReload()
    {
        // We need to increase shootCooldownTimer even when we're not pressing any attack input
        if (shootCooldownTimer >= shootCooldownDuration) return;
        shootCooldownTimer += Time.deltaTime;
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
            playerUI.reloadSlider.value = (reloadTimer / reloadDuration) * playerUI.reloadSlider.maxValue;
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
            reloadTimer = (bulletAmount / (float)maxBulletAmount) * reloadDuration;
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

        // Tries to enter a vent
        if (accessibleNewVent && accessibleNewVent != lastTakenNewVent && !isVentingOut)
        {
            CancelDash();
            accessibleNewVent.EntersVent(this);
            return;
        }

        // Tries to enter a turret
        if (!leavingTurret && accessibleControllableTurret)
        {
            Debug.Log("Entering turret");
            CancelDash();
            accessibleControllableTurret.OnPlayerEnters(this);
            return;
        }

        // Just dashing
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

        if (ventingCooldownTimer > ventingCooldownDuration)
        {
            ventingCooldownTimer = 0f;
            lastTakenNewVent = null;
            isVentingOut = false;
        }
        else ventingCooldownTimer += Time.deltaTime;
    }

    private void SetTimerAfterLeavingControllableTurret()
    {
        if (!leavingTurret) return;

        if (turretCooldownTimer > turretCooldownDuration)
        {
            turretCooldownTimer = 0f;
            leavingTurret = false;
        }
        else turretCooldownTimer += Time.deltaTime;
    }

    private void PowerCheck()
    {
        if (!powerUpIsActive) return;

        if (currentPowerUp.OnConditionCheck()) EndOfPowerUp();

        // Que se passe-t-il si le joueur entre dans une vent ou dans une tourelle avec un power up ? Il s'annule ? Il s'interromp et reprend après avoir quitté ?
    }

    #endregion

    #region Reset

    public void ResetSpeed()
    {
        speed = baseSpeed;
    } // Resets player's speed

    public void ResetShootCooldown()
    {
        shootCooldownDuration = baseShootCooldown;
    }

    public void ResetDeath() // Called when the player dies
    {
        if (reparationArea != null && reparationArea.isEveryPlayerOn)
        {
            reparationArea.OnTriggerExit(col);
        }

        if (currentControllableTurret != null)
        {
            currentControllableTurret.OnPlayerExits();
        }

        SetAccessibleTurret(null);
        SetAccessibleNewVent(null);

        DeactivatePlayer();
        col.enabled = false;
        rd.gameObject.SetActive(false);
        directionArrow.enabled = false;
        playerLight.enabled = false;
        trail.enabled = false;

        ResetPlayer();
        SetGaugesState(false);
    }

    public void ResetRespawn() // Called when the player respawns
    {
        SetGaugesState(true);
        transform.position = initPos;
        ResetPlayerGraphsAndCollisions();
        ActivatePlayer();
    }

    private void ResetPlayer() // Resets the player's main variables when the game begins or when dying
    {
        if (isActiveVent) SetVentingPlayer(false);
        if (isActiveControllableTurret) SetControllableTurretPlayer(false);
        ResetSpeed();
        ResetShootCooldown();
        isAttacking = false;
        isVentingOut = false;
        isDashing = false;
        aiming = false;
        isAutoReloading = false;
        EndOfPowerUp();
    }

    private void ResetPlayerGraphsAndCollisions() // Resets the player's renderers and colliders
    {
        rd.gameObject.SetActive(true);
        trail.enabled = true;
        directionArrow.enabled = true;
        playerLight.enabled = true;
        col.enabled = true;
        ventingPlayerMesh.rd.gameObject.SetActive(false);
        ventingPlayerMesh.trailRd.enabled = false;
    }

    private void ResetGauges()
    {
        playerUI.lifeSlider.maxValue = manager.totalLife;
        playerUI.lifeSlider.value = manager.totalLife;
        bulletAmount = maxBulletAmount;
        playerUI.reloadSlider.maxValue = maxBulletAmount;
        playerUI.reloadSlider.value = playerUI.reloadSlider.maxValue;
        //reloadGauge.transform.position = Camera.main.WorldToScreenPoint(transform.position)
        //                                 + new Vector3(0, -30);
        powerUpScore = 0;
        //powerUpGauge.fill = powerUpMax;
        playerUI.powerUpSlider.fillAmount = 0;
        //powerUpGauge.transform.position = Camera.main.WorldToScreenPoint(transform.position)
        //                                  + new Vector3(50, 0);
    } // Resets gauges to their initial values

    #endregion

    #region Modification

    public void ModifySpeed(float factor)
    {
        var newSpeed = baseSpeed * factor;
        speed = newSpeed;
    } // Multiplies the speed by factor

    public void CancelDash()
    {
        if (!isDashing) return;
        isDashing = false;
        ResetSpeed();
        dashTimer = 0f;
    }

    #endregion

    #region Gauges

    public void SetGaugesState(bool active)
    {
        if (active && partyManager.gameState == PartyManager.GameState.End)
        {
            Debug.LogWarning("Tried to display gauges after end of game.");
            return;
        }

        playerUI.self.SetActive(active);
    } // Displays or hides gauges

    #endregion

    #region Power-Ups

    public void IncreasePowerUpGauge(int value)
    {
        if (canUsePowerUp) return;
        powerUpScore = Mathf.Min(powerUpMax, powerUpScore += value);
        playerUI.powerUpSlider.fillAmount = powerUpScore / powerUpMax;

        if (playerUI.powerUpSlider.fillAmount >= 1) GetPowerUp();
    } // Increases power up gauge value when the player hits an enemy

    private void GetPowerUp()
    {
        // Feedback get power up
        canUsePowerUp = true;
        playerUI.powerUpFire.Play();
        playerUI.powerUpSparks.Play();
        playerUI.powerUpUI.Play("GetPowerUpUI");
        playerFire.Play();
        int index;
        if (GameManager.instance.powerUps.Count < 2)
        {
            Debug.LogWarning("Not enough power ups!");
            index = 0;
        }
        else
        {
            do
            {
                index = UnityEngine.Random.Range(0, 3);
            } while (index == lastPowerUpIndex);
        }

        lastPowerUpIndex = index;
        
        currentPowerUp = GameManager.instance.powerUps[index];
        playerUI.powerUpImage.sprite = currentPowerUp.powerUpImage;
        playerUI.powerUpSlider.fillAmount = 0;
    } // Gives the player a new power up

    public void CancelPowerUp()
    {
        if (!canUsePowerUp || powerUpIsActive) return;

        playerUI.powerUpFire.Stop();
        playerUI.powerUpSparks.Stop();
        playerFire.Stop();
        playerUI.powerUpImage.sprite = defaultPowerUpImage;

        currentPowerUp = null;
    } // Prevents the player from bringing a power-up from a game to another

    public void EndOfPowerUp()
    {
        if (!powerUpIsActive) return;

        currentPowerUp.OnDeactivate();
        powerUpScore = 0;
        playerUI.powerUpSlider.fillAmount = powerUpScore / powerUpMax;
        playerUI.powerUpImage.sprite = defaultPowerUpImage;
        canUsePowerUp = false;
        playerUI.powerUpFire.Stop();
        playerFire.Stop();
        playerUI.powerUpSparks.Stop();
        currentPowerUp = null;
    } // Resets player variables when a power up ends

    #endregion

    #region Activation & Deactivation

    public void ActivatePlayer()
    {
        if (partyManager.gameState == PartyManager.GameState.End)
        {
            Debug.LogWarning("Tried to active player after the end of game.");
            return;
        }

        isActive = true;
    }

    public void DeactivatePlayer()
    {
        leftJoystickInput = Vector2.zero;
        rb.velocity = Vector3.zero;
        isActive = false;
    }

    public void SetVentingPlayer(bool goingIn)
    {
        isActiveVent = goingIn;
        //SetGaugesState(!goingIn);
        rb.velocity = Vector3.zero;

        if (goingIn)
        {
            // Feedbacks player goes in
            accessibleNewVent = null;
            rd.gameObject.SetActive(false);
            trail.enabled = false;
            directionArrow.enabled = false;
            playerLight.enabled = false;
            col.enabled = false;
            ventingPlayerMesh.rd.gameObject.SetActive(true);
            ventingPlayerMesh.trailRd.enabled = true;
        }
        else
        {
            // Feedbacks player goes out
            currentConduit = null;
            isVentingOut = true;
            detectInputConduit = false;
            ResetPlayerGraphsAndCollisions();
        }
    }

    public void SetControllableTurretPlayer(bool goingIn)
    {
        isActiveControllableTurret = goingIn;
        //SetGaugesState(!goingIn);
        rb.velocity = Vector3.zero;
        rb.isKinematic = goingIn;
        playerLight.enabled = !goingIn;
        //col.enabled = !goingIn;
        if (!goingIn) ResetShootCooldown();
    }

    #endregion

    #region Setter

    public void SetCurrentReparationArea(ReparationArea ra) => reparationArea = ra;
    public void SetAccessibleNewVent(NewVent v) => accessibleNewVent = v;
    public void SetLastTakenNewVent(NewVent v) => lastTakenNewVent = v;
    public void SetCurrentConduit(NewConduit c) => currentConduit = c;
    public void SetAccessibleTurret(ControllableTurret ct) => accessibleControllableTurret = ct;
    public void SetCurrentTurret(ControllableTurret ct) => currentControllableTurret = ct;

    #endregion

    private void OnDisable()
    {
        // Security
        dataGamepad.gamepad.SetMotorSpeeds(0, 0);
    }
}