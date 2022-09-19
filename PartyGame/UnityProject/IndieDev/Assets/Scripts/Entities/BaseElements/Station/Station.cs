using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Station : BaseElementManager
{
    public override void TakeDamage(int damage)
    {
        Debug.Log("Station is hurt");
        base.TakeDamage(damage);
    }

    public override void OnDestroyed()
    {
        base.OnDestroyed();

        Debug.Log("Station is dead");

        // Water rises
        
        // Faire monter un plane avec un shader d'eau
        // Appliquer ralentissement
    }

    public override void OnFixed()
    {
        base.OnFixed();
        
    }

    public override void Update()
    {
        base.Update();
        
    }
}
