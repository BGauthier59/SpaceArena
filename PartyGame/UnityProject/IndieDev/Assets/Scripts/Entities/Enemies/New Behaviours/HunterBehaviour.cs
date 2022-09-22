using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class HunterBehaviour : EnemyGenericBehaviour
{
    [SerializeField] private HunterState currentState;

   // [SerializeField] private float distanceFromPlayer;
   [SerializeField] private float2 distancesFromPlayer;

    [SerializeField] private float durationAim;
    private float timerAim;

    [SerializeField] private float rotationSpeed;
    [SerializeField] private float fleeSpeed;

    [SerializeField] private GameObject projectile;
    
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
        agent.stoppingDistance = distancesFromPlayer.x;
        SwitchState(HunterState.Target);
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
        // Instantiate projectile
        // Set damage
        // Set direction
        // Set speed
    }

    public override void CheckState()
    {
        if (!IsTargetAvailable() && currentState != HunterState.Cooldown && currentState != HunterState.Target)
        {
            SwitchState(HunterState.Target);
        }

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
                    var distancePosition = Vector3.Distance(transform.position, target.transform.position);
                    if (distancePosition <= distancesFromPlayer.y) // Dans la zone d'attaque
                    {
                        // Avant de viser : check qu'il n'y a rien entre lui et la cible (raycast)
                        // S'il y a un truc, continuer d'avancer (retirer stopping distance ?)
                        
                        SwitchState(HunterState.Aim);
                    }
                    else // Trop loin pour attaquer
                    {
                        agent.SetDestination(target.transform.position);
                    }
                }
                else timerBeforeFollow += Time.deltaTime;

                break;

            #endregion

            #region Aim

            case HunterState.Aim:

                // Viser et reculer si nécessaire

                agent.transform.rotation = Quaternion.Lerp(transform.rotation,
                    Quaternion.LookRotation(-(transform.position - target.transform.position)),
                    Time.deltaTime * rotationSpeed);
                
                var distanceAim = Vector3.Distance(transform.position, target.transform.position);
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
                Debug.Log(state);
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
                StopAgent();
                break;
        }

        currentState = state;
    }
}