using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    public Transform target;
    public EnemyManager manager;
    public float moveSpeed;
    public bool isMoving;
    public bool isAttacking;

    public NavMeshAgent agent;
    
    public virtual void Target()
    {
        
    }

    public virtual void Attack()
    {
        
    }
    
}
