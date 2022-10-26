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
            player.SetAccessibleNewVent(this);
            playersOnVent.Add(player);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<PlayerController>();
            player.SetAccessibleNewVent(null);
            playersOnVent.Remove(player);
        }
    }

    public void Initialization(NewConduit linkedConduit)
    {
        conduit = linkedConduit;
    }

    public void EntersVent(PlayerController player)
    {
        foreach (var v in conduit.vents)
        {
            if (v.ventingPlayer) return;
        }

        playersOnVent.Remove(player);

        ventingPlayer = player;

        ventingPlayer.SetVentingPlayer(true);
        initPos = frontPos.position;
        posToreach = firstReachedPoint.pointPos.position;

        distanceBetweenPoints = Vector3.Distance(initPos, posToreach);

        isVenting = true;
        isEntering = true;
    }

    public void ExitsVent(PlayerController player)
    {
        ventingPlayer = player;
        ventingPlayer.SetLastTakenNewVent(this);

        initPos = linkedVentPoint.pointPos.position;
        posToreach = frontPos.position;

        distanceBetweenPoints = Vector3.Distance(initPos, posToreach);

        isVenting = true;
        isEntering = false;
    }

    private void Update()
    {
        if (isVenting) Venting();
    }

    public void Venting()
    {
        if (ventingTimer > (conduit.moveDurationUnit * distanceBetweenPoints))
        {
            ventingPlayer.transform.position = posToreach;

            foreach (var v in conduit.vents)
            {
                v.ventingTimer = 0f;
                v.isVenting = false;
            }

            if (isEntering)
            {
                conduit.EntersConduit(this, ventingPlayer);
            }
            else
            {
                ventingPlayer.SetVentingPlayer(false);

                foreach (var v in conduit.vents)
                {
                    v.ventingPlayer = null;
                }
            }
        }
        else
        {
            ventingPlayer.transform.position = Vector3.Lerp(initPos, posToreach,
                ventingTimer / (conduit.moveDurationUnit * distanceBetweenPoints));
            ventingTimer += Time.deltaTime;
        }
    }
}