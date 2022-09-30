using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class HeartBreakerBehaviour : EnemyGenericBehaviour
{
    [SerializeField] private HeartBreakerState currentState;
    public Entity lastAttacker;
    public bool hasCalledForHelp;
    [SerializeField] private float helpersDistance;

    [SerializeField] [Range(0, 100)] float helpRate;
    
    [Header("VFX")]
    [SerializeField] private ParticleSystem callHelpVFX;

    public enum HeartBreakerState
    {
        Target,
        Follow,
        Attack,
        Cooldown,
        Idle,
        CallForHelp
    }

    public override void Initialization()
    {
        base.Initialization();
        lastAttacker = null;
        hasCalledForHelp = false;
    }

    public override void Target()
    {
        foreach (var element in BaseManager.instance.allBaseElements)
        {
            if (element.GetType() == typeof(BaseHeart)) target = element;
        }
        // Target le coeur

        if (!IsTargetAvailable())
        {
            Debug.Log("HeartBreaker can't reach the Heart");

            SetAvailableTargets();
            target = DetectedEntity(availableTargets);

            //SwitchState(BreakerState.Idle);
        }
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
        target.TakeDamage(damage);
        // Inflige directement des dégâts au coeur
    }

    public override void CheckState()
    {
        if (!IsTargetAvailable() && currentState != HeartBreakerState.Cooldown &&
            currentState != HeartBreakerState.Target)
        {
            SwitchState(HeartBreakerState.Idle);
        }

        switch (currentState)
        {
            #region State Target

            case HeartBreakerState.Target:

                if (timerBeforeTarget >= durationBeforeTarget)
                {
                    if (IsTargetAvailable()) SwitchState(HeartBreakerState.Follow);
                    else Target();
                }
                else timerBeforeTarget += Time.deltaTime;

                break;

            #endregion

            #region State Follow

            case HeartBreakerState.Follow:

                if (timerBeforeFollow >= durationBeforeFollow)
                {
                    agent.SetDestination(target.transform.position);

                    var distance = Vector3.Distance(transform.position, target.transform.position);
                    if (distance <= minDistanceToAttack)
                    {
                        SwitchState(HeartBreakerState.Attack);
                    }
                }
                else timerBeforeFollow += Time.deltaTime;

                break;

            #endregion

            #region State Attack

            case HeartBreakerState.Attack:

                if (timerBeforeAttack >= durationBeforeAttack)
                {
                    Attack();
                    SwitchState(HeartBreakerState.Cooldown);
                }
                else timerBeforeAttack += Time.deltaTime;

                break;

            #endregion

            #region State Cooldown

            case HeartBreakerState.Cooldown:

                if (timerCooldown >= durationCooldown)
                {
                    SwitchState(HeartBreakerState.Target);
                }
                else timerCooldown += Time.deltaTime;

                break;

            #endregion

            #region State Call for Help

            case HeartBreakerState.CallForHelp:

                TryCallForHelp();
                SwitchState(HeartBreakerState.Follow);

                break;

            #endregion

            #region State Idle

            case HeartBreakerState.Idle:
                // N'a pas de direction prédéterminée
                // Continue à Target
                if (IsTargetAvailable()) SwitchState(HeartBreakerState.Follow);
                else Target();
                break;

            #endregion
        }
    }

    public void SwitchState(HeartBreakerState state)
    {
        switch (state)
        {
            case HeartBreakerState.Target:
                StopAgent();
                agent.velocity = Vector3.zero;
                timerBeforeTarget = 0f;
                break;

            case HeartBreakerState.Follow:
                UnstopAgent();
                timerBeforeFollow = 0f;
                break;

            case HeartBreakerState.Attack:
                StopAgent();
                agent.velocity = Vector3.zero;
                timerBeforeAttack = 0f;
                break;

            case HeartBreakerState.Cooldown:
                StopAgent();
                timerCooldown = 0f;
                break;

            case HeartBreakerState.Idle:
                StopAgent();
                break;
            case HeartBreakerState.CallForHelp:
                StopAgent();
                break;
            default:
                Debug.LogError("State not available!");
                break;
        }

        currentState = state;
    }

    public void TryCallForHelp()
    {
        if (hasCalledForHelp) return;

        var random = Random.Range(0, 100);
        if (random > helpRate) return;

        var helping = new List<EnemyManager>();

        foreach (var e in EnemiesManager.instance.allEnemies)
        {
            if (e.isDead) continue;
            if(Vector3.Distance(transform.position, e.transform.position) > helpersDistance) continue;
            if (!e.behaviour.availableTargets.Contains(lastAttacker)) continue;
            //Debug.Log($"{e} targets attacker!");
            e.behaviour.SetTarget(lastAttacker);
            helping.Add(e);
        }
        //Debug.Log($"Called for help!");

        hasCalledForHelp = true;
        if(!callHelpVFX.isPlaying) callHelpVFX.Play();
    }
}