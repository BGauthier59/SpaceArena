using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReparationArea : MonoBehaviour
{
    public BaseElementManager associatedElement;
    public Transform iconPosition;
    private Collider areaCollider;
    public bool isPlayerOn;
    public bool isWaitingForInput;
    public PlayerController currentPlayerOnArea;
    public List<PlayerController> playersOnArea;

    private void Start()
    {
        areaCollider = GetComponent<Collider>();
        DeactivateArea();
    }

    public void ActivateArea()
    {
        areaCollider.enabled = true;
    }

    public void DeactivateArea()
    {
        areaCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<PlayerController>();
            playersOnArea.Add(player);

            if (playersOnArea.Count == 1)
            {
                player.reparationArea = this;
                isPlayerOn = true;
                currentPlayerOnArea = playersOnArea[0];
                associatedElement.CheckPlayersOnReparationAreas();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<PlayerController>();
            playersOnArea.Remove(player);
            player.reparationArea = null;

            if (playersOnArea.Count == 0)
            {
                currentPlayerOnArea = null;
                isPlayerOn = false;
                isWaitingForInput = false;
                associatedElement.CancelReparation();
            }
            else
            {
                currentPlayerOnArea = playersOnArea[0];
                currentPlayerOnArea.reparationArea = this;
            }
        }
    }
}
