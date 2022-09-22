using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartBreakerBehaviour : BreakerBehaviour
{
    public override void Target()
    {
        foreach (var element in BaseManager.instance.allBaseElements)
        {
            if (element.GetType() == typeof(BaseHeart)) target = element;
        }
        // Target le coeur
    }

    public override void Attack()
    {
        base.Attack();
        // Inflige directement des dégâts au coeur
    }
}
