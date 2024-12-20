using System;
using UnityEngine;

public class RageScript : MonoBehaviour
{
    public Entity user;
    public CapsuleCollider collider;
    public int damage;
    [SerializeField] private float knockbackStrength;
    
    private void Start()
    {
        collider = GetComponent<CapsuleCollider>();
    } 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            Entity entity = other.GetComponent<Entity>();
            Rigidbody rb = other.GetComponent<Rigidbody>();
            entity.attackDirection = transform.rotation;
            entity.TakeDamage(damage, user);
            rb.AddForce((other.transform.position - transform.position).normalized * knockbackStrength);
        }
    }
}
