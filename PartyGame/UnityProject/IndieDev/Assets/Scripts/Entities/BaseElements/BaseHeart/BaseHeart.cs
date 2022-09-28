using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseHeart : BaseElementManager
{
    public override void TakeDamage(int damage, Entity attacker = null)
    {
        base.TakeDamage(damage, attacker);
    }

    public override void OnDestroyed()
    {
        base.OnDestroyed();
        
        Debug.Log("End of party");
    }

    public override void OnFixed()
    {
        // Can't be fixed...
    }
}
