using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienManager : EnemyManager
{
    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        Debug.Log("Alien got hurt");
        ((AlienBehaviour)behaviour).SwitchState(AlienBehaviour.AlienState.Retreat);
    }
}
