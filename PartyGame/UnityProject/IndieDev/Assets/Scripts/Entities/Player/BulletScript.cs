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

    [SerializeField] private TrailRenderer bulletLine;
    [SerializeField] private MeshRenderer bulletRenderer;
    [SerializeField] private ParticleSystem sparkVfx;

    public bool isAutoTurret;
    

    private void OnTriggerEnter(Collider collision)
    {
        var hit = false;
        
        PoolOfObject.Instance.SpawnFromPool(PoolType.Bullet_Impact, transform.position, Quaternion.identity);
        var interactable = collision.GetComponent<IInteractable>();
        interactable?.OnHitByProjectile(transform.forward);

        if (collision.CompareTag("Enemy") || collision.CompareTag("Player"))
        {
            var script = collision.GetComponent<Entity>();
            if (script == shooter) return;
            var damage = Range(minDamage, maxDamage);
            script.attackDirection = transform.rotation;
            hit = true;
            script.TakeDamage(damage, shooter);
        }
        
        if(!isAutoTurret) shooter.controller.UpdatePrecisionRatio(hit);
        gameObject.SetActive(false);
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void InitializeBullet()
    {
        bulletLine.material.SetColor("_EmissionColor",
            GameManager.instance.colors[shooter.controller.playerIndex - 1] * 2);
        bulletRenderer.material.SetColor("_EmissionColor",
            GameManager.instance.colors[shooter.controller.playerIndex - 1] * 2);
        
        sparkVfx.Play();
    }

    private void OnDisable()
    {
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
        }
    }
}