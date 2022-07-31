using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : Entity
{
    #region Entity

    public override void TakeDamage(int damage)
    {
        
        
        base.TakeDamage(damage);
    }

    public override void Death()
    {
        base.Death();

    }

    public override void Heal(int heal)
    {
        base.Heal(heal);

    }

    #endregion
}
