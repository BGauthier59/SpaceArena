using System;
using System.Collections;
using System.Collections.Generic;
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
    private bool needToReload = false;
    [SerializeField] private float reloadDuration;
    private float reloadTimer;

    private void Start()
    {
        bulletAmount = maxBullet;
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
        playerInside.SetControllableTurretPlayer(true);

        playerInside.transform.position = seat.position;

        playerInside.transform.SetParent(rotatingPart);
        playerInside.transform.localRotation = Quaternion.identity;

        playerInside.SetCurrentTurret(this);
        playerInside.shootCooldownDuration = shootCooldownDuration;
    }

    public void OnPlayerExits()
    {
        playerInside.SetCurrentTurret(null);
        playerInside.transform.SetParent(null);
        DontDestroyOnLoad(playerInside.gameObject); // Il faut le refaire ?
        playerInside.SetControllableTurretPlayer(false);
        playerInside = null;
        isPlayerInside = false;
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
        if (bulletAmount <= 0)
        {
            Debug.Log("You can't shoot any more");
            // Feedbacks
            needToReload = true;
            return;
        }

        bulletAmount--;
        var bullet = PoolOfObject.Instance
            .SpawnFromPool(PoolType.ControllableTurretProjectile, cannonOrigin.position, Quaternion.identity)
            .GetComponent<Rigidbody>();

        bullet.AddForce(transform.forward * bulletSpeed);
    }

    private void Update()
    {
        if(needToReload) Reloading();
    }

    public void Reloading()
    {
        if (reloadTimer >= reloadDuration)
        {
            reloadTimer = 0f;
            needToReload = false;
            bulletAmount = maxBullet;
        }
        else reloadTimer += Time.deltaTime;
    }
}