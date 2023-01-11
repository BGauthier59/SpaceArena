using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public int totalLife;
    public int currentLife;
    public Animator animator;
    public Rigidbody rb;
    public Quaternion attackDirection;

    public bool isDead;
    public bool isStunned;
    private float currentStunDuration;
    private float currentStunTimer;
    
    protected PartyManager partyManager;

    public virtual void Start()
    {
        Initialization();
    }

    public virtual void Update()
    {
        Stunning();
    }

    private void Initialization()
    {
        partyManager = GameManager.instance.partyManager;
        currentLife = totalLife;
    }

    public virtual void TakeDamage(int damage, Entity attacker = null)
    {
        if (isDead) return;
        currentLife -= damage;
        if (currentLife <= 0)
        {
            currentLife = 0;
            Death(attacker);
        }
    }

    protected virtual void Death(Entity killer)
    {
        isDead = true;
    }

    protected virtual void Heal(int heal)
    {
        currentLife = Mathf.Min(currentLife + heal, totalLife);
    }

    public void Project(Vector3 force, bool isStun = false, float stunDuration = 1)
    {
        rb.AddForce(force);
        if (isStun && !isStunned)
        {
            StunEnable();
            currentStunDuration = stunDuration;
        }
    }

    private void Stunning()
    {
        if (!isStunned) return;

        if (currentStunTimer >= currentStunDuration)
        {
            StunDisable();
        }
        else currentStunTimer += Time.deltaTime;
    }

    protected virtual void Fall()
    {
        TakeDamage(totalLife);
    }

    protected virtual void StunEnable()
    {
        isStunned = true;
    }

    protected virtual void StunDisable()
    {
        isStunned = false;
    }

    #region Trigger & Collisions

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hole"))
        {
            Fall();
        }
    }

    #endregion
    
}