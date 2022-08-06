using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBreakerBehaviour : EnemyBehaviour
{
    [SerializeField] private Entity detectedEntity;
    private EnemyBreakerState currentState;

    public enum EnemyBreakerState
    {
        Target,
        Follow,
        Attack,
        Destroyed,
        Stopped
    }

    [SerializeField] private float minDistance;

    [Header("Timers")] 
    [SerializeField] private float durationBeforeTarget;
    private float timerBeforeTarget;
    
    [SerializeField] private float durationBeforeFollow;
    private float timerBeforeFollow;
    
    [SerializeField] private float durationBeforeAttack;
    private float timerBeforeAttack;


    private void Start()
    {
        // Quand est initialisé
    }

    private void OnEnable()
    {
        // Quand est activé (parce que les ennemis ne sont pas destroy)

        Initialization();
        Target();
    }

    private void Initialization()
    {
        agent.enabled = true;

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
                if (timerBeforeTarget > durationBeforeTarget) Target();
                else timerBeforeTarget += Time.deltaTime;
                break;

            case EnemyBreakerState.Follow:
                if (timerBeforeFollow > durationBeforeFollow)
                {
                    var targetPos = detectedEntity.transform.position;
                    agent.SetDestination(targetPos);
                    var distance = Vector3.Distance(targetPos, transform.position);
                    if (distance >= minDistance)
                    {
                        SwitchState(EnemyBreakerState.Attack);
                    }
                }
                else timerBeforeFollow += Time.deltaTime;
                break;

            case EnemyBreakerState.Attack:
                break;

            case EnemyBreakerState.Destroyed:
                break;

            case EnemyBreakerState.Stopped:
                break;
            default:
                Debug.LogError("State is not valid");
                break;
        }
    }

    private void SwitchState(EnemyBreakerState state)
    {
        switch (state)
        {
            case EnemyBreakerState.Target:
                timerBeforeTarget = 0f;
                break;
            
            case EnemyBreakerState.Follow:
                timerBeforeFollow = 0f;
                break;
            
            case EnemyBreakerState.Attack:
                timerBeforeAttack = 0f;
                break;
            
            case EnemyBreakerState.Destroyed:
                break;
            
            case EnemyBreakerState.Stopped:
                break;
            
            default:
                Debug.LogError("State is not valid");
                break;
        }

        currentState = state;
    }

    public override void Target()
    {
        detectedEntity = BaseElementDetected();

        if (detectedEntity == null) detectedEntity = PlayerDetected();

        if (detectedEntity == null)
        {
            manager.rb.velocity = Vector3.zero;
            agent.enabled = false;
            Debug.LogWarning("Neither base element nor player has been found!");
            SwitchState(EnemyBreakerState.Stopped);
        }
        else
        {
            SwitchState(EnemyBreakerState.Follow);
        }
    }

    public override void Attack()
    {
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

            var distance = Vector3.Distance(reachable.transform.position, transform.position);
            if (distance > nearestDistance) continue;
            nearestDistance = distance;
            nearest = reachable;
        }

        return nearest;
    }

    private PlayerManager PlayerDetected()
    {
        var reachablePlayers = new List<PlayerManager>();

        // Stocker tous les joueurs sensibles

        foreach (var player in GameManager.instance.allPlayers)
        {
            if (player != null && !player.manager.isDead)
            {
                reachablePlayers.Add(player.manager);
            }
        }

        // Sélectionner le plus proche

        var nearestDistance = Mathf.Infinity;
        PlayerManager nearest = null;
        for (int i = 0; i < reachablePlayers.Count; i++)
        {
            var reachable = reachablePlayers[i];

            var distance = Vector3.Distance(reachable.transform.position, transform.position);
            if (distance > nearestDistance) continue;
            nearestDistance = distance;
            nearest = reachable;
        }

        return nearest;
    }
}