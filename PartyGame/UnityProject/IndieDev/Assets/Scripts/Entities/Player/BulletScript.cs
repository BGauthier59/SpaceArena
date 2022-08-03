using System;
using System.Collections;
using UnityEngine;
using static UnityEngine.Random;

public class BulletScript : MonoBehaviour
{
    [SerializeField] private float lifespan;
    private Rigidbody rb;
    [SerializeField] private float recoil;
    [SerializeField] private int maxDamage;
    [SerializeField] private int minDamage;
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player")) return;
        
        if (collision.CompareTag("Enemy"))
        {
            var script = collision.GetComponent<EnemyManager>();
            script.rb.AddForce(recoil * rb.velocity);
            var damage = Range(minDamage, maxDamage);
            script.TakeDamage(damage);
        }
        gameObject.SetActive(false);
        PoolOfObject.Instance.SpawnFromPool(PoolType.Bullet_Impact, transform.position, Quaternion.identity);
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        StartCoroutine(Lifespan());
    }

    private IEnumerator Lifespan()
    {
        yield return new WaitForSeconds(lifespan);
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
        }
    }
}