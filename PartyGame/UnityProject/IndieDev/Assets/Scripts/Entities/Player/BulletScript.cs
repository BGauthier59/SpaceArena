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

    [SerializeField] private TrailRenderer bulletLine;
    [SerializeField] private MeshRenderer bulletRenderer;

    public bool isAutoTurret;
    
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
        var hit = false;
        
        PoolOfObject.Instance.SpawnFromPool(PoolType.Bullet_Impact, transform.position, Quaternion.identity);
        var interactable = collision.GetComponent<IInteractable>();
        interactable?.OnHitByProjectile();

        if (collision.CompareTag("Enemy") || collision.CompareTag("Player"))
        {
            var script = collision.GetComponent<Entity>();
            if (script == shooter) return;
            var damage = Range(minDamage, maxDamage);
            script.attackDirection = transform.rotation;
            hit = true;
            script.TakeDamage(damage, shooter);
        }

        lifeTimer = lifeDuration;
        if(!isAutoTurret) shooter.controller.UpdatePrecisionRatio(hit);
        PoolOfObject.Instance.SpawnFromPool(PoolType.Bullet_Impact, transform.position, Quaternion.identity);
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void SetBulletColor()
    {
        bulletLine.material.SetColor("_EmissionColor",
            GameManager.instance.colors[shooter.controller.playerIndex - 1] * 2);
        bulletRenderer.material.SetColor("_EmissionColor",
            GameManager.instance.colors[shooter.controller.playerIndex - 1] * 2);
    }

    private void OnDisable()
    {
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
        }
    }
}