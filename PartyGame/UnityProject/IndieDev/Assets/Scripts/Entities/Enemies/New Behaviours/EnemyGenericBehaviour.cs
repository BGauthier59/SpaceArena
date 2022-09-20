using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyGenericBehaviour : MonoBehaviour
{
    protected Entity[] availableTargets;
    [SerializeField] protected Entity target;
    [SerializeField] private float speed;
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

    [SerializeField] protected float attackDuration;
    protected float attackTimer;

    [SerializeField] protected float durationCooldown;
    protected float timerCooldown;

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
        agent.speed = speed;
    }

    public virtual void Target()
    {
        SetAvailableTargets();
        target = DetectedEntity(availableTargets);
        
        if (!target)
        {
            Debug.LogWarning("No target found !");
        }
    }
    
    public bool IsTargetAvailable()
    {
        if (!target || target.isDead) return false;

        var path = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, target.transform.position, NavMesh.AllAreas, path);
        return path.status == NavMeshPathStatus.PathComplete;
    }

    public Entity DetectedEntity(Entity[] targets)
    {
        var reachableEntities = new List<Entity>();

        foreach (var t in targets)
        {
            if (t && !t.isDead) reachableEntities.Add(t);
        }

        // Sélectionner le plus proche
        var nearestDistance = Mathf.Infinity;
        Entity nearest = null;
        for (int i = 0; i < reachableEntities.Count; i++)
        {
            var reachable = reachableEntities[i];

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
        
        // Pour l'instant :
        target.TakeDamage(damage);
    }

    private void OnEnable()
    {
        Initialization();
    }

    private void OnDisable()
    {
        agent.enabled = false;
    }
}
