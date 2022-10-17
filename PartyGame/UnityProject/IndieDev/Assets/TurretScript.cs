using System;
using UnityEngine;

public class TurretScript : MonoBehaviour
{
    [SerializeField] private float lifespan;
    [SerializeField] private float shootingRate;
    [SerializeField] private float bulletSpeed;
    private bool hasTarget;
    private Transform target;
    private float lastShoot;
    private Vector3 shootingDirection;
    public PlayerController user;

    private void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTarget && other.CompareTag("Enemy"))
        {
            target = other.transform;
            hasTarget = true;
        }
    }

    private void Shoot()
    {
        if (hasTarget && Time.time > lastShoot + shootingRate)
        {
            shootingDirection = (transform.position - target.position).normalized;
            lastShoot = Time.time;
            
            var newBullet = PoolOfObject.Instance.SpawnFromPool(PoolType.Bullet, transform.position, Quaternion.identity);
            var bullet = newBullet.GetComponent<BulletScript>();
            bullet.shooter = user.manager;
            bullet.rb.AddForce(shootingDirection * bulletSpeed);
            newBullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletSpeed);
        }
    }

    private void CheckTarget()
    {
        if (target == null)
        {
            hasTarget = false;
        }
    }

    private void Update()
    {
        Shoot();
        CheckTarget();
    }
}
