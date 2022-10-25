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
    public Rigidbody controllableTurretProjectile;

    private void OnTriggerEnter(Collider other)
    {
        if (isPlayerInside) return;
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<PlayerController>();
            players.Add(player);
            player.accessibleControllableTurret = this;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<PlayerController>();
            players.Remove(player);
            player.accessibleControllableTurret = null;
        }
    }

    public void OnPlayerEnters(PlayerController player)
    {
        isPlayerInside = true;
        playerInside = player;
        playerInside.DeactivatePlayer();

        playerInside.transform.position = seat.position;

        playerInside.transform.SetParent(rotatingPart);
        playerInside.transform.localRotation = Quaternion.identity;

        playerInside.currentControllableTurret = this;
    }

    public void OnPlayerExits()
    {
        playerInside.currentControllableTurret = null;
        playerInside.transform.SetParent(null);
        DontDestroyOnLoad(playerInside.gameObject); // Il faut le refaire ?
        playerInside.ActivatePlayer();
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
}