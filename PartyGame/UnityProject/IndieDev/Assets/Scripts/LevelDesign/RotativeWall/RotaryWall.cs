using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RotaryWall : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform rotaryBase;
    [SerializeField] private NavMeshObstacle obstacle;
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

    public void OnHitByProjectile(Vector3 forward)
    {
        var factor = Vector3.Dot(forward, rotaryBase.forward);
        if (factor < 0) return;
        
        isRotating = true;
        obstacle.enabled = false;

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
            obstacle.enabled = true;
        }
        else
        {
            rotaryBase.rotation = Quaternion.Lerp(currentRotation, nextRotation, rotationTimer / rotationDuration);
            rotationTimer += Time.deltaTime;
        }
    }

    public void AddEntityToArea(Transform tr)
    {
        tr.SetParent(rotaryBase);
    }
}