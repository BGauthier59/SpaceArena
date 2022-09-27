using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyGenericBehaviour : MonoBehaviour
{
    public Entity[] availableTargets;
    [SerializeField] protected Entity target;
    [SerializeField] protected float speed;
    public EnemyManager manager;
    public bool isMoving;
    public bool isAttacking;
    public Collider attackArea;

    public int damage;

    public NavMeshAgent agent;
    
    public float minDistanceToAttack;
    
    [Header("Timers")] [SerializeField] protected float durationBeforeTarget;
    protected float timerBeforeTarget;

    [SerializeField] protected float durationBeforeFollow;
    protected float timerBeforeFollow;

    [SerializeField] protected float durationBeforeAttack;
    protected float timerBeforeAttack;

    [SerializeField] protected float durationCooldown;
    protected float timerCooldown;

    protected Vector3 targetPos;

    private void Start()
    {
        Initialization();
    }

    private void Update()
    {
        CheckState();
    }

    public virtual void Initialization()
    {
        agent.enabled = true;
        agent.speed = speed;
        targetPos = Vector3.zero;
    }

    public virtual void Target()
    {
        SetAvailableTargets();
        target = DetectedEntity(availableTargets);
    }
    
    public bool IsTargetAvailable()
    {
        if (!target || target.isDead) return false;

        var path = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, targetPos, NavMesh.AllAreas, path);
        
        return path.status == NavMeshPathStatus.PathComplete;
    }

    public Entity DetectedEntity(Entity[] targets, Entity exception = null)
    {
        var reachableEntities = new List<Entity>();
        
        foreach (var t in targets)
        {
            if(t == exception) continue;
            if (t && !t.isDead) reachableEntities.Add(t);
        }

        // Sélectionner le plus proche
        var nearestDistance = Mathf.Infinity;
        Entity nearest = null;
        for (int i = 0; i < reachableEntities.Count; i++)
        {
            var reachable = reachableEntities[i];
            Debug.Log(reachable.name);

            // Est-ce que le chemin est accessible ?
            var path = new NavMeshPath();
            NavMesh.CalculatePath(transform.position, reachable.transform.position, NavMesh.AllAreas, path);
            if (path.status != NavMeshPathStatus.PathComplete) continue;
            
            // Est-ce que l'entité est plus proche que la précédente sélectionnée ?
            var distance = Vector3.Distance(reachable.transform.position, transform.position);
            if (distance > nearestDistance) continue;
            
            nearestDistance = distance;
            nearest = reachable;
        }
        
        Debug.Log(nearest);

        return nearest;
    }

    public virtual void SetAvailableTargets()
    {
        
    }
    
    public virtual void CheckState()
    {
        
    }
    
    public virtual void Attack()
    {
        //attackArea.enabled = true;
        // Au final : activation, désactivation dans l'animation d'attaque ?
        
        // Pour l'instant :
        target.TakeDamage(damage);
    }
    
    public void StopAgent()
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        manager.rb.velocity = Vector3.zero;
        manager.rb.isKinematic = true;
    }

    public void UnstopAgent()
    {
        if (!agent.enabled)
        {
            Debug.LogWarning("agent was disabled");
            return;
        }
        agent.isStopped = false;
        manager.rb.isKinematic = false;
    }

    public void SetTarget(Entity t) => target = t;
    
    private void OnEnable()
    {
        Initialization();
    }

    private void OnDisable()
    {
        agent.enabled = false;
    }
}
