using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HunterProjectile : MonoBehaviour
{
    private HunterBehaviour hunter;
    public Rigidbody rb;
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

    public void Initialization(HunterBehaviour entity)
    {
        hunter = entity;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<PlayerManager>();
            player.TakeDamage(hunter.damage);
        }

        lifeTimer = lifeDuration;
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
