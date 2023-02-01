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
    public MeshRenderer[] meshRenderers;
    [SerializeField] private List<Transform> enemiesInRange;
    [SerializeField] private Transform shootOrigin;
    [SerializeField] private Transform rotatingPart;

    private void Start()
    {
        StartCoroutine(Lifespan());
        foreach (var rd in meshRenderers)
        {
            rd.material = user.rd.material;
        }
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
            
            rotatingPart.rotation = Quaternion.LookRotation(shootingDirection);
            var newBullet = PoolOfObject.Instance.SpawnFromPool(PoolType.Bullet, shootOrigin.position, rotatingPart.rotation);
            var bullet = newBullet.GetComponent<BulletScript>();
            bullet.isAutoTurret = true;
            bullet.shooter = user.manager;
            bullet.InitializeBullet();
            bullet.rb.AddForce(rotatingPart.forward * bulletSpeed);
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
