using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseHeart : BaseElementManager
{
    public override void TakeDamage(int damage)
    {
        Debug.Log("Base heart is hurt");
        base.TakeDamage(damage);
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
