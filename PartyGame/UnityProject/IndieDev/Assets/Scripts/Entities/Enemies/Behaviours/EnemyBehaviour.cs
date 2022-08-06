using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    public Entity target;
    public EnemyManager manager;
    public bool isMoving;
    public bool isAttacking;

    public int damage;

    public NavMeshAgent agent;
    
    public virtual void Target()
    {
        
    }

    public virtual void Attack()
    {
        
    }
    
}
