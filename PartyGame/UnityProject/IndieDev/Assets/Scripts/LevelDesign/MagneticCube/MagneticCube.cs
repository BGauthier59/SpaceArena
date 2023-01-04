using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagneticCube : MonoBehaviour, IInteractable
{
    // Peut être tourné à 90 dégrés s'il reçoit un projectile d'un player

    // Envoie un raycast en face de lui dès qu'il est stable


    private bool rotating;
    [SerializeField] private float rotatingDuration;
    private float rotatingTimer;
    private Vector3 currentEulerAngles;

    [SerializeField] private Transform raycastOrigin;
    [SerializeField] private float rayDistance;
    [SerializeField] private float forceValue;
    private bool castRay;
    [SerializeField] private LayerMask magnetizedCubeLayer;
    [SerializeField] private Transform rotatingCube;
    private MagnetizedCube currentMagnetizedCube;

    private bool attractCube;

    private void Start()
    {
        Initialization();
    }

    private void Initialization()
    {
        castRay = true;
        rotating = false;
        attractCube = false;
    }

    private void Update()
    {
        if (castRay) CastRay();
        if (rotating) Rotating();
        if (attractCube) ApplyForceOnCurrentMagnetizedCube();
    }

    private void CastRay()
    {
        RaycastHit hit;
        if (Physics.Raycast(raycastOrigin.position, raycastOrigin.forward, out hit, rayDistance, magnetizedCubeLayer))
        {
            Debug.DrawRay(raycastOrigin.position, raycastOrigin.forward * rayDistance, Color.green);
            currentMagnetizedCube = hit.transform.GetComponent<MagnetizedCube>();
            currentMagnetizedCube.currentMagneticCubes.Add(this);
            currentMagnetizedCube.EnableElectricity();
            attractCube = true;
            castRay = false;
            return;
        }

        Debug.DrawRay(raycastOrigin.position, raycastOrigin.forward * rayDistance, Color.red);
        if (!currentMagnetizedCube) return;
        currentMagnetizedCube.currentMagneticCubes.Remove(this);
        currentMagnetizedCube.DisableElectricity();
        currentMagnetizedCube = null;
    }

    private void ApplyForceOnCurrentMagnetizedCube()
    {
        var forceDir = transform.position - currentMagnetizedCube.transform.position;
        forceDir.Normalize();
        currentMagnetizedCube.rb.velocity = forceDir * forceValue;
    }

    public void OnHitByProjectile(Vector3 forward)
    {
        Debug.Log("Hit!");
        if (rotating) return;

        currentEulerAngles = rotatingCube.eulerAngles;
        rotating = true;
        castRay = false;
        attractCube = false;
    }


    private void Rotating()
    {
        if (rotatingTimer >= rotatingDuration)
        {
            rotatingCube.eulerAngles = currentEulerAngles + Vector3.up * 90;
            rotatingTimer = 0f;
            rotating = false;
            castRay = true;
        }
        else
        {
            rotatingCube.eulerAngles = Vector3.Lerp(currentEulerAngles, currentEulerAngles + Vector3.up * 90,
                rotatingTimer / rotatingDuration);

            rotatingTimer += Time.deltaTime;
        }
    }
}