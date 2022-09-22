using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AlienBehaviour : EnemyGenericBehaviour
{
    [SerializeField] private AlienState currentState;

    public enum AlienState
    {
        Target,
        Follow,
        Attack,
        Cooldown,
        SwitchAggro,
        Idle
    }

    public override void Initialization()
    {
        base.Initialization();
        SwitchState(AlienState.Target);
    }

    public override void Target()
    {
        base.Target();
    }

    public override void SetAvailableTargets()
    {
        var entities = new List<Entity>();
        foreach (var player in GameManager.instance.allPlayers)
        {
            entities.Add(player.manager);
        }

        foreach (var baseElement in BaseManager.instance.allBaseElements)
        {
            entities.Add(baseElement);
        }

        availableTargets = entities.ToArray();
    }

    public override void Attack()
    {
        base.Attack();
    }

    public override void CheckState()
    {
        if (!IsTargetAvailable() && currentState != AlienState.Cooldown && currentState != AlienState.Target)
        {
            SwitchState(AlienState.Target);
        }

        switch (currentState)
        {
            #region State Target

            case AlienState.Target:

                if (timerBeforeTarget >= durationBeforeTarget)
                {
                    if (IsTargetAvailable()) SwitchState(AlienState.Follow);
                    else Target();
                }
                else timerBeforeTarget += Time.deltaTime;

                break;

            #endregion

            #region State Follow

            case AlienState.Follow:

                if (timerBeforeFollow >= durationBeforeFollow)
                {
                    agent.SetDestination(target.transform.position);

                    var distance = Vector3.Distance(transform.position, target.transform.position);
                    if (distance <= minDistanceToAttack)
                    {
                        SwitchState(AlienState.Attack);
                    }
                }
                else timerBeforeFollow += Time.deltaTime;

                break;

            #endregion

            #region State Attack

            case AlienState.Attack:

                if (timerBeforeAttack >= durationBeforeAttack)
                {
                    Attack();
                    SwitchState(AlienState.Cooldown);
                }
                else timerBeforeAttack += Time.deltaTime;

                break;

            #endregion

            #region State Cooldown

            case AlienState.Cooldown:

                if (timerCooldown >= durationCooldown)
                {
                    SwitchState(AlienState.Target);
                }
                else timerCooldown += Time.deltaTime;

                break;

            #endregion

            #region State SwitchAggro

            case AlienState.SwitchAggro:

                Debug.LogWarning("Hasn't found any target");

                break;

            #endregion

            #region State Idle

            case AlienState.Idle:
                break;

            #endregion
        }
    }

    public void SwitchState(AlienState state)
    {
        switch (state)
        {
            case AlienState.Target:
                StopAgent();
                timerBeforeTarget = 0f;
                break;

            case AlienState.Follow:
                UnstopAgent();
                timerBeforeFollow = 0f;
                break;

            case AlienState.Attack:
                StopAgent();
                timerBeforeAttack = 0f;
                break;

            case AlienState.Cooldown:
                StopAgent();
                timerCooldown = 0f;
                break;

            case AlienState.SwitchAggro:
                UnstopAgent();
                break;

            case AlienState.Idle:
                StopAgent();
                break;
        }

        currentState = state;
    }
}