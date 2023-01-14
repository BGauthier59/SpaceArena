using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public abstract class EnemyGenericBehaviour : MonoBehaviour
{
    #region Variables

    public Entity[] availableTargets;
    [SerializeField] protected Entity target;
    [SerializeField] protected float speed;
    public EnemyManager manager;
    public bool isMoving;
    public bool isAttacking;
    public Collider attackArea;

    public int damage;

    public NavMeshAgent agent;

    [SerializeField] private float maxDistanceToTarget;
    protected float sqrMaxDistanceToTarget;
    
    public float minDistanceToAttack;

    [Header("Timers")] [SerializeField] protected float durationBeforeTarget;
    protected float timerBeforeTarget;

    [SerializeField] protected float durationBeforeFollow;
    protected float timerBeforeFollow;

    [SerializeField] protected float durationBeforeAttack;
    protected float timerBeforeAttack;

    [SerializeField] protected float durationCooldown;
    protected float timerCooldown;

    [SerializeField] protected float attackDuration;
    protected float attackTimer;

    protected Vector3 targetPos;

    [SerializeField] protected float durationBeforeRandomPoint;
    protected float timerBeforeRandomPoint;
    [SerializeField] protected float durationBeforeRandomPointGap;
    [SerializeField] protected float durationBeforeRandomPointReal;

    [SerializeField] private float idleRandomPointMaxDistance;
    [SerializeField] private float idleRandomPointMinDistance;

    protected PartyManager partyManager;

    #endregion

    private void Start()
    {
        Initialization();
    }

    public virtual void Update()
    {
        CheckState();
        AttackDuration();
    }

    protected virtual void Initialization()
    {
        partyManager = GameManager.instance.partyManager;
        agent.enabled = true;
        agent.speed = speed;
        target = null;
        targetPos = Vector3.zero;
        sqrMaxDistanceToTarget = Mathf.Pow(maxDistanceToTarget, 2);
    }

    protected virtual void Target()
    {
        SetAvailableTargets();
        target = DetectedEntity(availableTargets);
    }

    protected bool IsTargetAvailable()
    {
        if (!target || target.isDead) return false;

        var path = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, targetPos, NavMesh.AllAreas, path);

        return path.status == NavMeshPathStatus.PathComplete || path.status == NavMeshPathStatus.PathPartial;
    }

    protected Entity DetectedEntity(Entity[] targets, Entity exception = null)
    {
        var reachableEntities = new List<Entity>();

        foreach (var t in targets)
        {
            if (t == exception) continue;
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

    protected abstract void SetAvailableTargets();

    protected abstract void CheckState();

    protected virtual void Attack()
    {
        attackArea.gameObject.SetActive(true);
        isAttacking = true;
    }

    private void AttackDuration()
    {
        if (!isAttacking) return;
        if (attackTimer > attackDuration)
        {
            attackTimer = 0f;
            attackArea.gameObject.SetActive(false);
            isAttacking = false;
        }
        else attackTimer += Time.deltaTime;
    }

    protected (bool, Vector3) RandomPointOnPath()
    {
        // Set un point aléatoire dans le radius d'idle
        var randomPoint = transform.position + Random.insideUnitSphere * idleRandomPointMaxDistance;

        // Check si on navmesh
        NavMeshHit hit;
        if (!NavMesh.SamplePosition(randomPoint, out hit, idleRandomPointMaxDistance, NavMesh.AllAreas))
            return (false, Vector3.zero);

        var nextPos = hit.position;
        if (Vector3.Distance(transform.position, nextPos) < idleRandomPointMinDistance) return (false, Vector3.zero);

        var path = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, nextPos, NavMesh.AllAreas, path);
        if (path.status == NavMeshPathStatus.PathInvalid || path.status == NavMeshPathStatus.PathPartial)
            return (false, Vector3.zero);

        return (true, nextPos);
    }

    protected void StopAgent()
    {
        if (!agent.enabled) return;

        agent.isStopped = true;
        agent.updateRotation = false;
        agent.velocity = Vector3.zero;
        manager.rb.velocity = Vector3.zero;
        manager.rb.isKinematic = true;
    }

    protected void UnstopAgent()
    {
        if (!agent.enabled) return;

        agent.isStopped = false;
        agent.updateRotation = true;
        manager.rb.isKinematic = false;
    }

    public void SetTarget(Entity t)
    {
        target = t;
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