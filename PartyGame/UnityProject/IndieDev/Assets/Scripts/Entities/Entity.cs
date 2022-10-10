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

    public bool isDead;
    public bool isStunned;
    private float currentStunDuration;
    private float currentStunTimer;

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
        currentLife = totalLife;
    }

    public virtual void TakeDamage(int damage, Entity attacker = null)
    {
        if (isDead) return;
        currentLife -= damage;
        if (currentLife <= 0)
        {
            currentLife = 0;
            Death();
        }
    }

    protected virtual void Death()
    {
        isDead = true;
    }

    public virtual void Heal(int heal)
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

    public void Stunning()
    {
        if (!isStunned) return;

        if (currentStunTimer >= currentStunDuration)
        {
            StunDisable();
        }
        else currentStunTimer += Time.deltaTime;
    }

    public virtual void StunEnable()
    {
        isStunned = true;
    }

    public virtual void StunDisable()
    {
        isStunned = false;
    }
}
