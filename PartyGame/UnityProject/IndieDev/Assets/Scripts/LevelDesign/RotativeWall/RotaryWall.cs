using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotaryWall : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform rotaryBase;
    private bool isRotating;
    [SerializeField] private float rotationDuration;
    private float rotationTimer;
    private Quaternion currentRotation;
    private Quaternion nextRotation;

    private void Start()
    {
        currentRotation = rotaryBase.transform.rotation;
        nextRotation = Quaternion.Euler(currentRotation.eulerAngles + Vector3.up * 180);
    }

    public void OnHitByProjectile()
    {
        isRotating = true;
        
        // Feedbacks
    }

    private void Update()
    {
        if (isRotating) Rotating();
    }

    private void Rotating()
    {
        if (rotationTimer >= rotationDuration)
        {
            rotaryBase.rotation = nextRotation;
            currentRotation = nextRotation;
            nextRotation = Quaternion.Euler(currentRotation.eulerAngles + Vector3.up * 180);
            rotationTimer = 0f;
            isRotating = false;
        }
        else
        {
            rotaryBase.rotation = Quaternion.Lerp(currentRotation, nextRotation, rotationTimer / rotationDuration);
            rotationTimer += Time.deltaTime;
        }
    }

    public void AddPlayerToArea(Transform tr)
    {
        tr.SetParent(rotaryBase);
    }
}