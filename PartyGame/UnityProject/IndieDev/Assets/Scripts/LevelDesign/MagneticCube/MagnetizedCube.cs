using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetizedCube : MonoBehaviour
{
    public Rigidbody rb;
    [SerializeField] private bool isElectrified;
    [SerializeField] private GameObject electricity; // Plus tard : un FX
    public List<MagneticCube> currentMagneticCubes;

    public void EnableElectricity()
    {
        if (currentMagneticCubes.Count == 0) return;
        isElectrified = true;
        electricity.SetActive(true);
    }

    public void DisableElectricity()
    {
        if (currentMagneticCubes.Count != 0) return;
        isElectrified = false;
        electricity.SetActive(false);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (isElectrified && (other.CompareTag("Enemy") || other.CompareTag("Player")))
        {
            var script = other.GetComponent<Entity>();
            script.TakeDamage(script.totalLife);
        }
    }
}
