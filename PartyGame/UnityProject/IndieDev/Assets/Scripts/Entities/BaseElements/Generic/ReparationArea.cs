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

    [Header("Renderers")] public Renderer reparationAreaRenderer;
    public Renderer[] reparationAreaDevicesRenderer;

    public Material reparationAreaDisabled;
    public Material reparationAreaEnabled;

    private void Start()
    {
        areaCollider = GetComponent<Collider>();

        foreach (var rd in reparationAreaDevicesRenderer)
        {
            rd.material = BaseManager.instance.colorVariantMaterial;
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
            raColor = Color.Lerp(associatedElement.color, BaseManager.instance.disabledColor, timer);

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

        reparationAreaRenderer.material = reparationAreaEnabled;

        isActivated = true;
        areaCollider.enabled = true;
    }

    public void DeactivateArea()
    {
        if (!gameObject.activeSelf) return;

        reparationAreaRenderer.material = reparationAreaDisabled;

        foreach (var rd in reparationAreaDevicesRenderer)
        {
            rd.material.SetColor("_EmissionColor", BaseManager.instance.disabledColor * 1);
        }

        areaCollider.enabled = false;
        isActivated = false;

        playersOnArea.Clear();
        currentPlayerOnArea = null;
        isPlayerOn = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<PlayerController>();
            if (!playersOnArea.Contains(player)) playersOnArea.Add(player);

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