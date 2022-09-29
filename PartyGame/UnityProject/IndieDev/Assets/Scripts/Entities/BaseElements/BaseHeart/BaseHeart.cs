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
        
        if (GameManager.instance.partyManager == null)
        {
            Debug.LogError("Party Manager is null!");
            return;
        }
        GameManager.instance.partyManager.EndingGame(false);
    }

    public override void OnFixed()
    {
        // Can't be fixed...
    }
}
