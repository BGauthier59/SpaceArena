using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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

    public override void OnEnable()
    {
        base.OnEnable();
        Initialization();
    }
    
    private void Initialization()
    {
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

                if (timerBeforeTarget >= durationBeforeTarget)
                {
                    if (IsTargetAvailable())
                    {
                        SwitchState(EnemyAttackerState.Follow);
                    }
                    else Target();
                }
                else timerBeforeTarget += Time.deltaTime;
                
                break;

            case EnemyAttackerState.Follow:
                
                if (timerBeforeFollow >= durationBeforeFollow)
                {
                    if (IsTargetAvailable())
                    {
                        agent.SetDestination(target.transform.position);

                        var distance = Vector3.Distance(transform.position, target.transform.position);
                        if (distance <= minDistanceToAttack)
                        {
                            SwitchState(EnemyAttackerState.Attack);
                        }
                    }
                    else SwitchState(EnemyAttackerState.Target);
                }
                else timerBeforeFollow += Time.deltaTime;
                
                break;

            case EnemyAttackerState.Attack:

                if (timerBeforeAttack >= durationBeforeAttack)
                {
                    if (IsTargetAvailable())
                    {
                        Attack();
                        SwitchState(EnemyAttackerState.Cooldown);
                    }
                    else SwitchState(EnemyAttackerState.Target);
                }
                else timerBeforeAttack += Time.deltaTime;

                break;

            case EnemyAttackerState.Cooldown:

                if (timerCooldown >= durationCooldown)
                {
                    SwitchState(EnemyAttackerState.Target);
                }
                else timerCooldown += Time.deltaTime;

                break;

            case EnemyAttackerState.Idle:

                Debug.LogError("Not valid");
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
}
