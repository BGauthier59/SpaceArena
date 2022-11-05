using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ControllableTurret : MonoBehaviour
{
    private List<PlayerController> players = new List<PlayerController>();
    private bool isPlayerInside;
    private PlayerController playerInside;
    [SerializeField] private Transform seat;
    [SerializeField] private float rotatingSpeed;
    [SerializeField] private Transform rotatingPart;
    [SerializeField] private Transform cannonOrigin;
    [SerializeField] private float shootCooldownDuration;
    [SerializeField] private int maxBullet;
    private int bulletAmount;
    [SerializeField] private int bulletSpeed;
    public bool needToReload;
    [SerializeField] private float reloadDuration;
    private float reloadTimer;
    [SerializeField] private Renderer blinkingMesh;
    [SerializeField] private Color enableColor;
    [SerializeField] private Color disableColor;
    [SerializeField] private float enableSpeed;
    [SerializeField] private float disableSpeed;
    private static readonly int BlinkColor = Shader.PropertyToID("_BlinkColor");
    private static readonly int BlinkSpeed = Shader.PropertyToID("_BlinkSpeed");
    [SerializeField] private TextMeshPro indicator;
    [SerializeField] private MeshRenderer[] colorMeshes;
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
    private static readonly int Color1 = Shader.PropertyToID("_Color");

    private void Start()
    {
        bulletAmount = maxBullet;
        SetText();
        SetColor();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isPlayerInside) return;
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<PlayerController>();
            players.Add(player);
            player.SetAccessibleTurret(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<PlayerController>();
            players.Remove(player);
            player.SetAccessibleTurret(null);
        }
    }

    public void OnPlayerEnters(PlayerController player)
    {
        isPlayerInside = true;
        playerInside = player;
        
        playerInside.SetAccessibleTurret(null);
        playerInside.SetControllableTurretPlayer(true);

        playerInside.transform.position = seat.position;

        playerInside.transform.SetParent(rotatingPart);
        playerInside.transform.localRotation = Quaternion.identity;

        playerInside.SetCurrentTurret(this);
        playerInside.shootCooldownDuration = shootCooldownDuration;
        
        SetColor();
    }

    public void OnPlayerExits()
    {
        playerInside.SetCurrentTurret(null);
        playerInside.transform.SetParent(null);
        DontDestroyOnLoad(playerInside.gameObject); // Il faut le refaire ?
        playerInside.SetControllableTurretPlayer(false);
        playerInside = null;
        isPlayerInside = false;
        
        SetColor();
    }

    public void SetColor()
    {
        if (playerInside && !needToReload)
        {
            foreach (var rd in colorMeshes)
            {
                rd.material.SetColor(EmissionColor, GameManager.instance.colors[playerInside.playerIndex - 1] * 2);
            }
        }
        else
        {
            foreach (var rd in colorMeshes)
            {
                rd.material.SetColor(Color1, Color.grey);
                rd.material.SetColor(EmissionColor, Color.grey * 0);

            }
        }
    }

    public void Rotating(Vector2 rotation)
    {
        var forward = new Vector3(-rotation.x, 0, -rotation.y);
        if (forward == Vector3.zero) return;
        rotatingPart.rotation = Quaternion.Lerp(rotatingPart.rotation, Quaternion.LookRotation(forward),
            Time.deltaTime * rotatingSpeed);
        rotatingPart.eulerAngles = new Vector3(0, rotatingPart.eulerAngles.y, 0);
    }

    public void Shoot()
    {
        bulletAmount--;
        SetText();
        var bullet = PoolOfObject.Instance
            .SpawnFromPool(PoolType.ControllableTurretProjectile, cannonOrigin.position, Quaternion.identity)
            .GetComponent<Rigidbody>();

        bullet.AddForce(transform.forward * bulletSpeed);

        if (bulletAmount <= 0)
        {
            Debug.Log("You can't shoot any more");
            // Feedbacks
            needToReload = true;
            blinkingMesh.material.SetColor(BlinkColor, disableColor * 1);
            blinkingMesh.material.SetFloat(BlinkSpeed, disableSpeed);
            SetColor();
        }
    }

    private void SetText()
    {
        indicator.text = $"{bulletAmount} / {maxBullet}";
    }

    private void Update()
    {
        if (needToReload) Reloading();
    }

    public void Reloading()
    {
        if (reloadTimer >= reloadDuration)
        {
            reloadTimer = 0f;
            needToReload = false;
            bulletAmount = maxBullet;
            blinkingMesh.material.SetColor(BlinkColor, enableColor * 1);
            blinkingMesh.material.SetFloat(BlinkSpeed, enableSpeed);
            SetColor();
            SetText();
        }
        else reloadTimer += Time.deltaTime;
    }
}