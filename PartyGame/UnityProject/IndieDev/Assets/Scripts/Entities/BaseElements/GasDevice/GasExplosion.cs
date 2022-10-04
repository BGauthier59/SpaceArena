using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class GasExplosion : MonoBehaviour
{
    [SerializeField] private int explosionDamage;
    public SphereCollider areaCollider;

    [SerializeField] private int explosionForce;
    
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
            
            entity.TakeDamage(explosionDamage);

            var forceDirection = other.transform.position - transform.position;
            entity.Project(forceDirection, true, 2);

            
            Debug.DrawRay(transform.position, forceDirection, Color.green, 5);
            
            entity.rb.AddForce(forceDirection * explosionForce);
        }
    }
}
