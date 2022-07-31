using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public int totalLife;
    public int currenLife;

    public virtual void TakeDamage(int damage)
    {
        currenLife -= damage;
        if (currenLife <= 0)
        {
            currenLife = 0;
            Death();
        }
    }

    public virtual void Death()
    {
        
    }

    public virtual void Heal(int heal)
    {
        currenLife = Mathf.Min(currenLife + heal, totalLife);
    }
}