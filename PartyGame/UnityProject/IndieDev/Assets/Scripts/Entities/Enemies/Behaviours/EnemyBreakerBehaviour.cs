using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBreakerBehaviour : EnemyBehaviour
{
    private EnemyBreakerState currentState;

    private enum EnemyBreakerState
    {
        Target,
        Follow,
        Attack,
        Cooldown,
        Idle,
    }


    [Header("Timers")] 
    [SerializeField] private float durationBeforeTarget;
    private float timerBeforeTarget;

    [SerializeField] private float durationBeforeFollow;
    private float timerBeforeFollow;

    [SerializeField] private float durationBeforeAttack;
    private float timerBeforeAttack;
    
    [SerializeField] private float durationCooldown;
    private float timerCooldown;
    
    private void Start()
    {
        // Quand est initialisé
    }

    public override void OnEnable()
    {
        // Quand est activé (parce que les ennemis ne sont pas destroy)

        base.OnEnable();
        Initialization();
    }

    public override void OnDisable()
    {
        base.OnDisable();
    }

    private void Initialization()
    {
        SwitchState(EnemyBreakerState.Target);
    }

    private void Update()
    {
        CheckState();
    }

    private void CheckState()
    {
        switch (currentState)
        {
            case EnemyBreakerState.Target:
                if (timerBeforeTarget > durationBeforeTarget)
                {
                    Target();
                    var path = new NavMeshPath();
                    if (target == null || target.isDead)
                    {
                        SwitchState(EnemyBreakerState.Target);
                        return;
                    }
                    
                    NavMesh.CalculatePath(transform.position, target.transform.position, NavMesh.AllAreas, path);
                    SwitchState(path.status != NavMeshPathStatus.PathComplete ? EnemyBreakerState.Target : EnemyBreakerState.Follow);
                }
                else timerBeforeTarget += Time.deltaTime;
                break;

            case EnemyBreakerState.Follow:
                if (target == null || target.isDead || agent.path.status != NavMeshPathStatus.PathComplete)
                {
                    SwitchState(EnemyBreakerState.Target);
                    return;
                }

                if (timerBeforeFollow > durationBeforeFollow)
                {
                    var targetPos = target.transform.position;
                    agent.SetDestination(targetPos);
                    var distance = Vector3.Distance(targetPos, transform.position);
                    if (distance <= minDistanceToAttack)
                    {
                        SwitchState(EnemyBreakerState.Attack);
                    }
                }
                else timerBeforeFollow += Time.deltaTime;

                break;

            case EnemyBreakerState.Attack:
                if (timerBeforeAttack > durationBeforeAttack)
                {
                    Attack();
                    SwitchState(EnemyBreakerState.Cooldown);
                }
                else timerBeforeAttack += Time.deltaTime;
                break;

            case EnemyBreakerState.Idle:
                break;

            case EnemyBreakerState.Cooldown:
                if (timerCooldown >= durationCooldown)
                {
                    if (target == null || target.isDead)
                    {
                        Debug.Log("Current target has been defeated ? Targeting new one");
                        SwitchState(EnemyBreakerState.Target);
                    }
                    else
                    {
                        var targetPos = target.transform.position;
                        var distance = Vector3.Distance(targetPos, transform.position);
                        SwitchState(distance <= minDistanceToAttack ? EnemyBreakerState.Attack : EnemyBreakerState.Follow);
                    }
                }
                else timerCooldown += Time.deltaTime;
                break;
            
            default:
                Debug.LogError("State is not valid");
                break;
        }
    }

    private void SwitchState(EnemyBreakerState state)
    {
        Debug.Log("Switch state");
        switch (state)
        {
            case EnemyBreakerState.Target:
                agent.isStopped = true;
                timerBeforeTarget = 0f;
                break;

            case EnemyBreakerState.Follow:
                agent.isStopped = false;
                timerBeforeFollow = 0f;
                break;

            case EnemyBreakerState.Attack:
                timerBeforeAttack = 0f;
                agent.isStopped = true;
                break;

            case EnemyBreakerState.Idle:

                manager.rb.velocity = Vector3.zero;
                agent.isStopped = true;
                Debug.LogWarning("Neither base element nor player has been found!");
                break;

            case EnemyBreakerState.Cooldown:
                timerCooldown = 0f;
                agent.isStopped = true;
                break;
            
            default:
                Debug.LogError("State is not valid");
                break;
        }

        currentState = state;
    }

    public override void Target()
    {
        target = BaseElementDetected();
        if (target == null) target = PlayerDetected();
    }

    public override void Attack()
    {
        Debug.Log("Attack !");
        base.Attack();
    }

    private BaseElementManager BaseElementDetected()
    {
        var reachableElements = new List<BaseElementManager>();

        // Stocker tous les bâtiments sensibles
        foreach (var baseElement in BaseManager.instance.allBaseElements)
        {
            if (baseElement != null && !baseElement.isDead)
            {
                reachableElements.Add(baseElement);
            }
        }

        // Sélectionner le plus proche
        var nearestDistance = Mathf.Infinity;
        BaseElementManager nearest = null;
        for (int i = 0; i < reachableElements.Count; i++)
        {
            var reachable = reachableElements[i];

            // Est-ce que le chemin est accessible ?
            var path = new NavMeshPath();
            NavMesh.CalculatePath(transform.position, reachable.transform.position, NavMesh.AllAreas, path);
            if (path.status != NavMeshPathStatus.PathComplete) continue;
            
            // Est-ce que l'élément est plus proche que le précédent sélectionné ?
            var distance = Vector3.Distance(reachable.transform.position, transform.position);
            if (distance > nearestDistance) continue;
            
            nearestDistance = distance;
            nearest = reachable;
        }

        return nearest;
    }

}