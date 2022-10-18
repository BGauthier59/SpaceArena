using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretScript : MonoBehaviour
{
    [SerializeField] private float lifespan;
    [SerializeField] private float shootingRate;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float maxRange;
    private bool hasTarget;
    public Transform target;
    private float lastShoot;
    private Vector3 shootingDirection;
    public PlayerController user;
    public MeshRenderer meshRenderer;
    [SerializeField] private List<Transform> enemiesInRange;

    private void Start()
    {
        StartCoroutine(Lifespan());
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = user.rd.material;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemiesInRange.Add(other.transform);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemiesInRange.Remove(other.transform);
        }
    }

    private void Shoot()
    {
        if (hasTarget && Time.time > lastShoot + shootingRate)
        {
            shootingDirection = (target.position - transform.position ).normalized;
            lastShoot = Time.time;
            
            var newBullet = PoolOfObject.Instance.SpawnFromPool(PoolType.Bullet, transform.position, Quaternion.identity);
            var bullet = newBullet.GetComponent<BulletScript>();
            bullet.shooter = user.manager;
            bullet.rb.AddForce(shootingDirection * bulletSpeed);
        }
    }

    private void CheckTarget()
    {
        if (!hasTarget)
        {
            float distance = 100;
            foreach (var enemy in enemiesInRange)
            {
                if (Vector3.Distance(enemy.position, transform.position) <= distance)
                {
                    target = enemy;
                    hasTarget = true;
                }
            }
            if (target == null)
            {
                hasTarget = false;
            }
        }
        else
        {
            if (Vector3.Distance(target.position, transform.position) >= maxRange)
            {
                hasTarget = false;
                enemiesInRange.Remove(target);
            }
            if (!target.gameObject.activeSelf)
            {
                hasTarget = false;
                enemiesInRange.Remove(target);
            }
        }
    }

    private void Update()
    {
        Shoot();
        CheckTarget();
    }

    private IEnumerator Lifespan()
    {
        yield return new WaitForSeconds(lifespan);
        Destroy(gameObject);
    }
}
