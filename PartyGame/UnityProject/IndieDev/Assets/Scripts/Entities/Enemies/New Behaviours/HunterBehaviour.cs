using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class HunterBehaviour : EnemyGenericBehaviour
{
    [SerializeField] private HunterState currentState;

   [SerializeField] private float2 distancesFromPlayer;

    [SerializeField] private float durationAim;
    private float timerAim;

    [SerializeField] private float rotationSpeed;
    [SerializeField] private float fleeSpeed;

    [SerializeField] private float projectileSpeed;

    [SerializeField] private LayerMask detectionLayers;
    [SerializeField] private CapsuleCollider col;

    public enum HunterState
    {
        Target,
        Position,
        Aim,
        Shoot,
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
        SwitchState(HunterState.Idle);
    }

    public override void SetAvailableTargets()
    {
        var entities = new List<Entity>();
        foreach (var player in GameManager.instance.allPlayers)
        {
            entities.Add(player.manager);
        }

        availableTargets = entities.ToArray();
    }

    public override void Attack()
    {
        Debug.DrawLine(transform.position, targetPos, Color.magenta, .5f);

        var projectile =
            PoolOfObject.Instance.SpawnFromPool(PoolType.HunterProjectile, transform.position, Quaternion.identity);
        var hunterProjectile = projectile.GetComponent<HunterProjectile>();
        hunterProjectile.Initialization(this);
        hunterProjectile.rb.AddForce(transform.forward * projectileSpeed);
    }

    public override void CheckState()
    {
        if (!IsTargetAvailable() && currentState != HunterState.Cooldown && currentState != HunterState.Idle)
        {
            SwitchState(HunterState.Idle);
        }
        
        if(target) targetPos = target.transform.position;
        
        switch (currentState)
        {
            #region State Target

            case HunterState.Target:

                if (timerBeforeTarget >= durationBeforeTarget)
                {
                    if (IsTargetAvailable()) SwitchState(HunterState.Position);
                    else Target();
                }
                else timerBeforeTarget += Time.deltaTime;

                break;

            #endregion

            #region State Position

            case HunterState.Position:

                agent.ResetPath();
                if (timerBeforeFollow >= durationBeforeFollow)
                {
                    var distancePosition = Vector3.Distance(transform.position, targetPos);
                    if (distancePosition <= distancesFromPlayer.y) // Cible à portée
                    {
                        RaycastHit hit;
                        if (Physics.Raycast(transform.position + transform.forward * (col.radius + .01f), -(transform.position - targetPos), 
                                out hit, distancePosition, detectionLayers)) // S'il y a un obstacle entre l'ennemi et la cible
                        {
                            agent.SetDestination(targetPos);
                        }
                        else SwitchState(HunterState.Aim);
                    }
                    else agent.SetDestination(targetPos);
                }
                else timerBeforeFollow += Time.deltaTime;

                break;

            #endregion

            #region Aim

            case HunterState.Aim:

                // Viser et reculer si nécessaire
                Debug.DrawLine(transform.position, targetPos, Color.green);

                agent.transform.rotation = Quaternion.Lerp(transform.rotation,
                    Quaternion.LookRotation(-(transform.position - targetPos)),
                    Time.deltaTime * rotationSpeed);
                
                var distanceAim = Vector3.Distance(transform.position, targetPos);
                if (distanceAim < distancesFromPlayer.x) // Trop près du joueur
                {
                    var nextPos = transform.position - transform.forward;
                    transform.position = Vector3.Lerp(transform.position, nextPos, fleeSpeed * Time.deltaTime);
                }

                if (timerAim >= durationAim)
                {
                    SwitchState(HunterState.Shoot);
                }
                else timerAim += Time.deltaTime;
                break;

            #endregion

            #region State Shoot

            case HunterState.Shoot:

                if (timerBeforeAttack >= durationBeforeAttack)
                {
                    Attack();
                    SwitchState(HunterState.Cooldown);
                }
                else timerBeforeAttack += Time.deltaTime;

                break;

            #endregion

            #region State Cooldown

            case HunterState.Cooldown:

                if (timerCooldown >= durationCooldown)
                {
                    SwitchState(HunterState.Target);
                }
                else timerCooldown += Time.deltaTime;

                break;

            #endregion

            #region State Idle

            case HunterState.Idle:
                
                if (IsTargetAvailable()) SwitchState(HunterState.Position);
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

    public void SwitchState(HunterState state)
    {
        switch (state)
        {
            case HunterState.Target:
                StopAgent();
                timerBeforeTarget = 0f;
                break;

            case HunterState.Position:
                UnstopAgent();
                timerBeforeFollow = 0f;
                break;

            case HunterState.Aim:
                StopAgent();
                timerAim = 0f;
                break;

            case HunterState.Shoot:
                StopAgent();
                timerBeforeAttack = 0f;
                break;

            case HunterState.Cooldown:
                StopAgent();
                timerCooldown = 0f;
                break;

            case HunterState.Idle:
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
}