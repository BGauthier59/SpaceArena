using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GrenadeScript : MonoBehaviour
{
    [SerializeField] private float timer;
    [SerializeField] private List<EnemyManager> enemiesInRange;
    [SerializeField] private int damage;
    [SerializeField] private CameraShakeScriptable explosionShake;

    private void Start()
    {
        StartCoroutine(Lifespan());
    }

    private IEnumerator Lifespan()
    {
        yield return new WaitForSeconds(timer);
        Explode();
    }

    private void Explode()
    {
        foreach (var enemy in enemiesInRange)
        {
            enemy.TakeDamage(damage);
        }

        PoolOfObject.Instance.SpawnFromPool(PoolType.Explosion, transform.position, Quaternion.identity);
        GameManager.instance.partyManager.cameraShake.AddShakeEvent(explosionShake);
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Enemy"))
        {
            Explode();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemiesInRange.Add(other.GetComponent<EnemyManager>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        enemiesInRange.Remove(other.GetComponent<EnemyManager>());
    }
}
