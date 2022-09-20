using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HunterBehaviour : EnemyGenericBehaviour
{
    [SerializeField] private HunterState currentState;

    public enum HunterState
    {
        Target, Follow, Attack, Cooldown, Idle
    }
    
    public override void Target()
    {
        base.Target();
    }
    
    public override void Initialization()
    {
        base.Initialization();
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
        // Activation de collider
    }
    
    public override void CheckState()
    {
        switch (currentState)
        {
            case HunterState.Target:
                break;
            
            case HunterState.Follow:
                break;
            
            case HunterState.Attack:
                break;
            
            case HunterState.Cooldown:
                break;
            
            case HunterState.Idle:
                break;

        }
    }
    
    public void SwitchState(HunterState state)
    {
        switch (currentState)
        {
            case HunterState.Target:
                break;
            
            case HunterState.Follow:
                break;
            
            case HunterState.Attack:
                break;
            
            case HunterState.Cooldown:
                break;
            
            case HunterState.Idle:
                break;

        }
        
        currentState = state;
    }
}
