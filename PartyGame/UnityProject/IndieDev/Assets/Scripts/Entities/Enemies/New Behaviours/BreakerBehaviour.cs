using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BreakerBehaviour : EnemyGenericBehaviour
{
    [SerializeField] private BreakerState currentState;

    public enum BreakerState
    {
        Target,
        Follow,
        Attack,
        Cooldown,
        Idle
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
        }

        currentState = state;
    }
}