using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    public Transform target;
    public float moveSpeed;
    public bool isMoving;
    public bool isAttacking;
    
    public virtual void Target()
    {
        
    }

    public virtual void Attack()
    {
        
    }
    
}
