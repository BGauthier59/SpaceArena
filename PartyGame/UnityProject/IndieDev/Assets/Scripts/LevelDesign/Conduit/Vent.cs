using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vent : MonoBehaviour
{
    private List<PlayerController> playersOnVent;
    [SerializeField] private Vent otherVent;

    private Conduit conduit;
    private Vector3 centerPos;
    private bool isPlayerEntering;

    private PlayerController enteringPlayer;
    private Way ventDirection;

    private Vector3 otherVentCenterPos;
    private float ventingTimer;
    

    public enum Way
    {
        Inside, Outside
    }

    public void Initialization(Conduit linkedConduit)
    {
        if(!otherVent) Debug.LogError("A linked vent is missing!");
        conduit = linkedConduit;
        playersOnVent = new List<PlayerController>();
        
        centerPos = transform.position;
        centerPos = new Vector3(centerPos.x, 1, centerPos.z);

        otherVentCenterPos = otherVent.transform.position;
        otherVentCenterPos = new Vector3(otherVentCenterPos.x, 1,
            otherVentCenterPos.z);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<PlayerController>();
            player.accessibleVent = this;
            playersOnVent.Add(player);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<PlayerController>();
            player.accessibleVent = null;
            playersOnVent.Remove(player);
        }
    }

    public void PlayerEnters(PlayerController player)
    {
        if (isPlayerEntering) return; // One player at the time
        if (otherVent.isPlayerEntering) return;

        isPlayerEntering = true;
        enteringPlayer = player;
        
        enteringPlayer.DeactivatePlayer();
        enteringPlayer.col.enabled = false;
    }

    private void Update()
    {
        if (isPlayerEntering) Venting();
    }

    private void Venting()
    {
        if (ventingTimer >= conduit.ventingDuration)
        {
            ventingTimer = 0f;   
            enteringPlayer.ActivatePlayer();
            enteringPlayer.col.enabled = true;
            enteringPlayer = null;
            isPlayerEntering = false;
        }
        else
        {
            ventingTimer += Time.deltaTime;
            
            enteringPlayer.transform.position = Vector3.Lerp(centerPos, otherVentCenterPos, ventingTimer / conduit.ventingDuration);
        }
    }
}
