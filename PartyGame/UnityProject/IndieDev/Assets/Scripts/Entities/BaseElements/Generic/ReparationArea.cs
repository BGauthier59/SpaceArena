using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReparationArea : MonoBehaviour
{
    [Header("Reparation data")] public BaseElementManager associatedElement;
    public Transform iconPosition;
    private Collider areaCollider;
    private bool isActivated;
    private float timer;
    private Color raColor;
    public bool isPlayerOn;
    public bool isEveryPlayerOn;
    public bool isWaitingForInput;
    public PlayerController currentPlayerOnArea;
    public List<PlayerController> playersOnArea;

    public bool isCompleted;

    [Header("Renderers")] public Renderer[] reparationAreaDevicesRenderer;

    private PartyManager partyManager;

    private void Start()
    {
        areaCollider = GetComponent<Collider>();
        partyManager = GameManager.instance.partyManager;

        foreach (var rd in reparationAreaDevicesRenderer)
        {
            rd.material = partyManager.baseManager.colorVariantMaterial;
            rd.material.color = Color.black;
        }

        DeactivateArea();
    }

    private void Update()
    {
        if (isActivated) SetColor();
    }

    private void SetColor()
    {
        if (isEveryPlayerOn)
        {
            foreach (var rd in reparationAreaDevicesRenderer)
            {
                rd.material.SetColor("_EmissionColor", associatedElement.color * 3);
            }

            return;
        }

        if (timer > 1) timer = 0f;
        else
        {
            raColor = Color.Lerp(associatedElement.color, partyManager.baseManager.disabledColor, timer);

            foreach (var rd in reparationAreaDevicesRenderer)
            {
                rd.material.SetColor("_EmissionColor", raColor * 3);
            }

            timer += Time.deltaTime;
        }
    }

    public void ActivateArea()
    {
        if (!gameObject.activeSelf) return;

        isActivated = true;
        areaCollider.enabled = true;
    }

    public void DeactivateArea()
    {
        if (!gameObject.activeSelf) return;

        foreach (var rd in reparationAreaDevicesRenderer)
        {
            rd.material.SetColor("_EmissionColor", partyManager.baseManager.disabledColor * 0);
        }

        areaCollider.enabled = false;
        isActivated = false;

        Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<PlayerController>();
            if (!playersOnArea.Contains(player)) playersOnArea.Add(player);

            if (playersOnArea.Count == 1)
            {
                player.SetCurrentReparationArea(this);
                Fill();

                // Check other areas
                if(associatedElement.IsEveryPlayerReady()) associatedElement.EveryPlayerIsReady();
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<PlayerController>();
            playersOnArea.Remove(player);
            player.SetCurrentReparationArea(null);

            if (playersOnArea.Count == 0)
            {
                Clear();
                
                // Cancels Reparation
                associatedElement.CancelReparation();
            }
            else
            {
                currentPlayerOnArea = playersOnArea[0];
                currentPlayerOnArea.SetCurrentReparationArea(this);
            }
        }
    }

    public void Fill()
    {
        isPlayerOn = true;
        currentPlayerOnArea = playersOnArea[0];
    }

    private void Clear()
    {
        playersOnArea.Clear();
        currentPlayerOnArea = null;
        isPlayerOn = false;
        isWaitingForInput = false;
        isCompleted = false;
    }

    public void Complete()
    {
        isCompleted = true;
        associatedElement.TryRepair();
    }

    public void UnComplete()
    {
        isCompleted = false;
    }
}