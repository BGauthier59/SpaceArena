using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartBreakerManager : EnemyManager
{
    public override void TakeDamage(int damage, Entity attacker = null)
    {
        base.TakeDamage(damage, attacker);
        var hb = (HeartBreakerBehaviour)behaviour;
        hb.lastAttacker = attacker;
        hb.SwitchState(HeartBreakerBehaviour.HeartBreakerState.CallForHelp);
    }
}
