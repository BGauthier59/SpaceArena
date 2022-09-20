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
        // Inflige directement des dégâts au base element
    }

    public override void CheckState()
    {
        switch (currentState)
        {
            case BreakerState.Target:
                break;

            case BreakerState.Follow:
                break;

            case BreakerState.Attack:
                break;

            case BreakerState.Cooldown:
                break;

            case BreakerState.Idle:
                break;
        }
    }

    public void SwitchState(BreakerState state)
    {
        switch (currentState)
        {
            case BreakerState.Target:
                break;

            case BreakerState.Follow:
                break;

            case BreakerState.Attack:
                break;

            case BreakerState.Cooldown:
                break;

            case BreakerState.Idle:
                break;
        }

        currentState = state;
    }
}