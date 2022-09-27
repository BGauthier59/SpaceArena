using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakerManager : EnemyManager
{
    public override void TakeDamage(int damage, Entity attacker = null)
    {
        base.TakeDamage(damage, attacker);
        Debug.Log("Breaker got hurt");
        ((BreakerBehaviour)behaviour).SwitchState(BreakerBehaviour.BreakerState.ChangeTarget);
    }
}
