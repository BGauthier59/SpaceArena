using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReparationArea : MonoBehaviour
{
    [Header("Reparation data")]
    public BaseElementManager associatedElement;
    public Transform iconPosition;
    private Collider areaCollider;
    public bool isPlayerOn;
    public bool isWaitingForInput;
    public PlayerController currentPlayerOnArea;
    public List<PlayerController> playersOnArea;

    [Header("Renderers")]
    public Renderer reparationAreaRenderer;
    public Renderer[] reparationAreaDevicesRenderer;
    
    public Material reparationAreaDisabled;
    public Material reparationAreaEnabled;
    public Material reparationAreaDeviceDisabled;

    private void Start()
    {
        areaCollider = GetComponent<Collider>();
        DeactivateArea();
    }

    public void ActivateArea()
    {
        reparationAreaRenderer.material = reparationAreaEnabled;

        foreach (var rd in reparationAreaDevicesRenderer)
        {
            rd.material = associatedElement.reparationAreaDeviceEnabled;
        }
        
        areaCollider.enabled = true;
    }

    public void DeactivateArea()
    {
        reparationAreaRenderer.material = reparationAreaDisabled;
        
        foreach (var rd in reparationAreaDevicesRenderer)
        {
            rd.material = reparationAreaDeviceDisabled;
        }
        
        areaCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<PlayerController>();
            if(!playersOnArea.Contains(player)) playersOnArea.Add(player);

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
