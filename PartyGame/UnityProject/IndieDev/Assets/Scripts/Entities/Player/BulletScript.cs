using System;
using System.Collections;
using UnityEngine;
using static UnityEngine.Random;

public class BulletScript : MonoBehaviour
{
    public PlayerManager shooter;
    public Rigidbody rb;
    [SerializeField] private int maxDamage;
    [SerializeField] private int minDamage;
    
    [SerializeField] private float lifeDuration;
    private float lifeTimer;

    private void Update()
    {
        if (lifeTimer >= lifeDuration)
        {
            lifeTimer = 0f;
            gameObject.SetActive(false);
        }
        else lifeTimer += Time.deltaTime;
    }
    
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player")) return;
        
        if (collision.CompareTag("Enemy"))
        {
            var script = collision.GetComponent<EnemyManager>();
            var damage = Range(minDamage, maxDamage);
            script.TakeDamage(damage, shooter);
        }
        lifeTimer = lifeDuration;
        PoolOfObject.Instance.SpawnFromPool(PoolType.Bullet_Impact, transform.position, Quaternion.identity);
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    
    private void OnDisable()
    {
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
        }
    }
}