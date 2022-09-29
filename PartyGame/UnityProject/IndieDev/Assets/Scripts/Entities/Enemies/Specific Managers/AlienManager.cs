using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienManager : EnemyManager
{
    public override void TakeDamage(int damage, Entity attacker = null)
    {
        base.TakeDamage(damage, attacker);
        Debug.Log("Alien got hurt");
        ((AlienBehaviour)behaviour).SwitchState(AlienBehaviour.AlienState.Retreat);
    }
}