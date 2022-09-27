using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class BreakerBehaviour : EnemyGenericBehaviour
{
    [SerializeField] private BreakerState currentState;
    [SerializeField] [Range(0, 100)] private float changeTargetRate;
    
    public enum BreakerState
    {
        Target,
        Follow,
        Attack,
        Cooldown,
        Idle,
        ChangeTarget
    }

    public override void Target()
    {
        base.Target();
    }

    public override void Initialization()
    {
        base.Initialization();
        SwitchState(BreakerState.Target);
    }

    public override void SetAvailableTargets()
    {
        var entities = new List<Entity>();
        foreach (var baseElement in BaseManager.instance.allBaseElements)
        {
            entities.Add(baseElement);
        }

        availableTargets = entities.ToArray();
    }

    public override void Attack()
    {
        base.Attack();
        // Inflige directement des dégâts au base element
    }

    public override void CheckState()
    {
        if (!IsTargetAvailable() && currentState != BreakerState.Cooldown && currentState != BreakerState.Target)
        {
            SwitchState(BreakerState.Target);
        }

        switch (currentState)
        {
            #region State Target

            case BreakerState.Target:

                if (timerBeforeTarget >= durationBeforeTarget)
                {
                    if (IsTargetAvailable()) SwitchState(BreakerState.Follow);
                    else Target();
                }
                else timerBeforeTarget += Time.deltaTime;

                break;

            #endregion

            #region State Follow

            case BreakerState.Follow:

                if (timerBeforeFollow >= durationBeforeFollow)
                {
                    agent.SetDestination(target.transform.position);

                    var distance = Vector3.Distance(transform.position, target.transform.position);
                    if (distance <= minDistanceToAttack)
                    {
                        SwitchState(BreakerState.Attack);
                    }
                }
                else timerBeforeFollow += Time.deltaTime;

                break;

            #endregion

            #region State Attack

            case BreakerState.Attack:

                if (timerBeforeAttack >= durationBeforeAttack)
                {
                    Attack();
                    SwitchState(BreakerState.Cooldown);
                }
                else timerBeforeAttack += Time.deltaTime;

                break;

            #endregion

            #region State Cooldown

            case BreakerState.Cooldown:

                if (timerCooldown >= durationCooldown)
                {
                    SwitchState(BreakerState.Target);
                }
                else timerCooldown += Time.deltaTime;

                break;

            #endregion

            #region State Change Target

            case BreakerState.ChangeTarget:

                if (TryChangeTarget().Item1)
                {
                    Debug.Log($"Change target worked ! New target : {target}");

                }
                else
                {
                    
                }
                SwitchState(BreakerState.Follow);
                break;

            #endregion
            
            #region State Idle

            case BreakerState.Idle:
                break;

            #endregion
        }
    }

    public void SwitchState(BreakerState state)
    {
        switch (state)
        {
            case BreakerState.Target:
                StopAgent();
                agent.velocity = Vector3.zero;
                timerBeforeTarget = 0f;
                break;

            case BreakerState.Follow:
                UnstopAgent();
                timerBeforeFollow = 0f;
                break;

            case BreakerState.Attack:
                StopAgent();
                agent.velocity = Vector3.zero;
                timerBeforeAttack = 0f;
                break;

            case BreakerState.Cooldown:
                StopAgent();
                timerCooldown = 0f;
                break;

            case BreakerState.Idle:
                StopAgent();
                break;
            case BreakerState.ChangeTarget:
                StopAgent();
                break;
            default:
                Debug.LogError("State not available!");
                break;
        }

        currentState = state;
    }

    private (bool, Entity) TryChangeTarget()
    {
        var random = Random.Range(0, 100);
        if (random >= changeTargetRate)
        {
            // Change Target
            target = DetectedEntity(availableTargets, target);
            return (true, target);
        }

        return (false, target);
    }
}