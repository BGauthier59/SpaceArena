using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewVent : MonoBehaviour
{
    private List<PlayerController> playersOnVent = new List<PlayerController>();
    private PlayerController ventingPlayer;

    public NewConduit conduit;
    public ConduitPoint linkedVentPoint;
    public ConduitPoint firstReachedPoint;

    public Transform frontPos;
    
    private Vector3 initPos;
    private Vector3 posToreach;
    private float distanceBetweenPoints;

    private bool isVenting;
    private float ventingTimer;
    private bool isEntering;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<PlayerController>();
            player.accessibleNewVent = this;
            playersOnVent.Add(player);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<PlayerController>();
            player.accessibleNewVent = null;
            playersOnVent.Remove(player);
        }
    }

    public void Initialization(NewConduit linkedConduit)
    {
        conduit = linkedConduit;
    }

    public void EntersVent(PlayerController player)
    {
        playersOnVent.Remove(player);

        ventingPlayer = player;
        ventingPlayer.DeactivatePlayer();
        ventingPlayer.col.enabled = false;
        initPos = frontPos.position;
        posToreach = firstReachedPoint.pointPos.position;

        distanceBetweenPoints = Vector3.Distance(initPos, posToreach);
        
        isVenting = true;
        isEntering = true;
    }
    
    public void ExitsVent(PlayerController player)
    {
        ventingPlayer = player;
        ventingPlayer.DeactivatePlayer();
        initPos = linkedVentPoint.pointPos.position;
        posToreach = frontPos.position;
        
        distanceBetweenPoints = Vector3.Distance(initPos, posToreach);

        isVenting = true;
        isEntering = false;
    }

    private void Update()
    {
        if(isVenting) Venting();
    }

    public void Venting()
    {
        if (ventingTimer > (conduit.moveDurationUnit * distanceBetweenPoints))
        {
            ventingTimer = 0f;
            isVenting = false;

            if (isEntering)
            {
                conduit.EntersConduit(this, ventingPlayer);
            }
            else
            {
                ventingPlayer.ActivatePlayer();
                ventingPlayer.col.enabled = true;
                ventingPlayer = null;
            }
        }
        else
        {
            ventingPlayer.transform.position = Vector3.Lerp(initPos, posToreach, ventingTimer / (conduit.moveDurationUnit * distanceBetweenPoints));
            ventingTimer += Time.deltaTime;
        }
    }
}
