using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GrenadeScript : MonoBehaviour
{
    [SerializeField] private float duration;
    private float timer;
    [SerializeField] private List<EnemyManager> enemiesInRange;
    [SerializeField] private int damage;
    [SerializeField] private CameraShakeScriptable explosionShake;

    private void Update()
    {
        if (timer >= duration)
        {
            timer = 0f;
            Explode();
        }
        else timer += Time.deltaTime;
    }

    private void Explode()
    {
        foreach (var enemy in enemiesInRange)
        {
            enemy.TakeDamage(damage);
        }
        enemiesInRange.Clear();

        PoolOfObject.Instance.SpawnFromPool(PoolType.Explosion, new Vector3(transform.position.x, 0.1f, transform.position.z), Quaternion.identity);
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
