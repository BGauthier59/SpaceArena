using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class AlienBehaviour : EnemyGenericBehaviour
{
    [SerializeField] public AlienState currentState;
    [SerializeField] [Range(0, 100)] private float retreatRate;

    private float initSpeed;
    [SerializeField] private float retreatSpeed;

    public enum AlienState
    {
        Target,
        Follow,
        Attack,
        Cooldown,
        Retreat,
        Idle
    }

    public override void Initialization()
    {
        base.Initialization();
        initSpeed = speed;
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

            case AlienState.Retreat:

                if (TryRetreat().Item1)
                {
                    Debug.Log($"Retreat worked ! New target : {target}");
                }
                else
                {
                    Debug.Log($"Retreat hasn't worked ! Keep old target : {target}");
                }

                SwitchState(AlienState.Follow);

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

            case AlienState.Retreat:
                UnstopAgent();
                break;

            case AlienState.Idle:
                StopAgent();
                break;
        }

        currentState = state;
    }

    private (bool, Entity) TryRetreat()
    {
        speed = initSpeed;

        var ratio = manager.currentLife / manager.totalLife;
        Debug.Log(ratio);

        if (!manager || ratio > .5f) return (false, target);

        var random = Random.Range(0, 100);
        if (random >= retreatRate)
        {
            target = DetectedEntity(availableTargets, target);
            initSpeed = speed;
            speed = retreatSpeed;
            return (true, target);
        }

        return (false, target);
    }
}