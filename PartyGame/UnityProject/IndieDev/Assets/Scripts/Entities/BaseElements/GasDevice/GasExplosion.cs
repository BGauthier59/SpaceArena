using System;
using UnityEngine;

public class GasExplosion : MonoBehaviour
{
    [SerializeField] private int explosionDamage;
    public SphereCollider areaCollider;

    private void Start()
    {
        areaCollider = GetComponent<SphereCollider>();
        areaCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            var entity = other.GetComponent<Entity>();
            Debug.Log($"{other.name} was hit by explosion !");
            entity.TakeDamage(explosionDamage);
        }
    }
}
