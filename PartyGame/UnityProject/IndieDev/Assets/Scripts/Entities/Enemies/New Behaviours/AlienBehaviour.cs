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

    [SerializeField] private float rotateSpeed;
    private float initSpeed;
    private bool isRetreating;
    [SerializeField] private float retreatSpeed;
    [SerializeField] private float durationBetweenRetreats;
    private float timerBetweenRetreats;

    [Header("VFX")] [SerializeField] private ParticleSystem retreatVFX;

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
        isRetreating = false;
        SwitchState(AlienState.Idle);
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

    public override void Update()
    {
        base.Update();

        if (!isRetreating) return;
        if (timerBetweenRetreats > durationBetweenRetreats)
        {
            timerBetweenRetreats = 0f;
            isRetreating = false;
        }
        else timerBetweenRetreats += Time.deltaTime;
    }

    public override void CheckState()
    {
        if (!IsTargetAvailable() && currentState != AlienState.Cooldown && currentState != AlienState.Idle)
        {
            SwitchState(AlienState.Idle);
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
                else
                {
                    /*
                    agent.transform.rotation = Quaternion.Lerp(transform.rotation,
                        Quaternion.LookRotation(-(transform.position - targetPos)),
                        Time.deltaTime * rotateSpeed);
                        */
                    
                    timerBeforeAttack += Time.deltaTime;
                }

                break;

            #endregion

            #region State Cooldown

            case AlienState.Cooldown:

                if (timerCooldown >= durationCooldown)
                {
                    //attackArea.gameObject.SetActive(false);
                    SwitchState(AlienState.Target);
                }
                else timerCooldown += Time.deltaTime;

                break;

            #endregion

            #region State SwitchAggro

            case AlienState.Retreat:

                if (TryRetreat().Item1)
                {
                    //Debug.Log($"Retreat worked ! New target : {target}");
                }
                else
                {
                    //Debug.Log($"Retreat hasn't worked ! Keep old target : {target}");
                }

                SwitchState(AlienState.Follow);

                break;

            #endregion

            #region State Idle

            case AlienState.Idle:

                if (IsTargetAvailable()) SwitchState(AlienState.Follow);
                else Target();

                if (timerBeforeRandomPoint > durationBeforeRandomPointReal)
                {
                    (bool, Vector3) couple;
                    var security = 0;
                    do
                    {
                        couple = RandomPointOnPath();
                        security++;
                        if (security > 100)
                        {
                            Debug.LogWarning("Might be not correct point");
                            return;
                        }
                    } while (!couple.Item1);

                    agent.SetDestination(couple.Item2);
                    timerBeforeRandomPoint = 0f;
                    durationBeforeRandomPointReal = durationBeforeRandomPoint +
                                                    Random.Range(-durationBeforeRandomPointGap,
                                                        durationBeforeRandomPointGap);
                }
                else timerBeforeRandomPoint += Time.deltaTime;

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
                UnstopAgent();
                timerBeforeRandomPoint = 0f;
                durationBeforeRandomPointReal = durationBeforeRandomPoint +
                                                Random.Range(-durationBeforeRandomPointGap,
                                                    durationBeforeRandomPointGap);
                break;
            default:
                Debug.LogError("State not available!");
                break;
        }

        currentState = state;
    }

    private (bool, Entity) TryRetreat()
    {
        speed = initSpeed;

        var ratio = manager.currentLife / manager.totalLife;
        if (!manager || ratio > .5f) return (false, target);

        var random = Random.Range(0, 100);
        if (random > retreatRate) return (false, target);

        target = DetectedEntity(availableTargets, target);
        initSpeed = speed;
        speed = retreatSpeed;
        if (!retreatVFX.isPlaying) retreatVFX.Play();
        isRetreating = true;
        return (true, target);
    }
}