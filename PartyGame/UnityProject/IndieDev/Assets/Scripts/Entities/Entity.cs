using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public int totalLife;
    private int currentLife;
    public Animator animator;
    public Rigidbody rb;

    public bool isDead;

    private void Start()
    {
        Initialization();
    }

    private void Initialization()
    {
        currentLife = totalLife;
    }

    public virtual void TakeDamage(int damage)
    {
        currentLife -= damage;
        if (currentLife <= 0)
        {
            currentLife = 0;
            Death();
        }
    }

    public virtual void Death()
    {
        isDead = true;
    }

    public virtual void Heal(int heal)
    {
        currentLife = Mathf.Min(currentLife + heal, totalLife);
    }
}
