using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackerBehaviour : EnemyBehaviour
{
    private EnemyAttackerState currentState;
    
    private enum EnemyAttackerState
    {
        Target, Follow, Attack, Cooldown, Idle
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

    private void OnEnable()
    {
        Initialization();
    }
    
    private void Initialization()
    {
        agent.enabled = true;

        SwitchState(EnemyAttackerState.Target);
    }

    private void Update()
    {
        CheckState();
    }

    private void CheckState()
    {
        switch (currentState)
        {
            case EnemyAttackerState.Target:
                if (timerBeforeTarget > durationBeforeTarget)
                {
                    Target();
                    SwitchState(target == null ? EnemyAttackerState.Idle : EnemyAttackerState.Follow);
                }
                else timerBeforeTarget += Time.deltaTime;
                break;
            
            case EnemyAttackerState.Follow:
                if (target == null)
                {
                    Debug.Log("Current target has been disabled ? Targeting new one");
                    SwitchState(EnemyAttackerState.Target);
                    return;
                }

                if (timerBeforeFollow > durationBeforeFollow)
                {
                    var targetPos = target.transform.position;
                    agent.SetDestination(targetPos);
                    var distance = Vector3.Distance(targetPos, transform.position);
                    if (distance <= minDistanceToAttack)
                    {
                        SwitchState(EnemyAttackerState.Attack);
                    }
                }
                else timerBeforeFollow += Time.deltaTime;    
                break;
            
            case EnemyAttackerState.Attack:
                if (timerBeforeAttack > durationBeforeAttack)
                {
                    Attack();
                    SwitchState(EnemyAttackerState.Cooldown);
                }
                else timerBeforeAttack += Time.deltaTime;
                break;
            
            case EnemyAttackerState.Cooldown:
                if (timerCooldown >= durationCooldown)
                {
                    if (target == null || target.isDead)
                    {
                        Debug.Log("Current target has been defeated ? Targeting new one");
                        SwitchState(EnemyAttackerState.Target);
                    }
                    else
                    {
                        var targetPos = target.transform.position;
                        var distance = Vector3.Distance(targetPos, transform.position);
                        SwitchState(distance <= minDistanceToAttack ? EnemyAttackerState.Attack : EnemyAttackerState.Follow);
                    }
                }
                else timerCooldown += Time.deltaTime;
                break;
            
            case EnemyAttackerState.Idle:
                
                break;
            
            default:
                Debug.LogError("State is not valid");
                break;
        }
    }

    private void SwitchState(EnemyAttackerState state)
    {
        switch (state)
        {
            case EnemyAttackerState.Target:
                agent.isStopped = true;
                timerBeforeTarget = 0f;
                break;
            
            case EnemyAttackerState.Follow:
                agent.isStopped = false;
                timerBeforeFollow = 0f;
                break;
            
            case EnemyAttackerState.Attack:
                agent.isStopped = true;
                timerBeforeAttack = 0f;
                break;
            
            case EnemyAttackerState.Cooldown:
                agent.isStopped = true;
                timerCooldown = 0f;
                break;
            
            case EnemyAttackerState.Idle:
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
        target = PlayerDetected();
    }
}
