using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HeartBreakerBehaviour : BreakerBehaviour
{
    
    // N'hérite pas de BreakerBehaviour
    
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
            SwitchState(BreakerState.Idle);
        }
    }

    public override void Attack()
    {
        base.Attack();
        // Inflige directement des dégâts au coeur
    }
}
