using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Entity
{
    public PlayerController controller;
    
    #region Entity

    public override void TakeDamage(int damage, Entity attacker = null)
    {
        base.TakeDamage(damage, attacker);

    }

    public override void Death()
    {
        base.Death();

    }

    public override void Heal(int heal)
    {
        base.Heal(heal);

    }
    
    public override void StunEnable()
    {
        base.StunEnable();
        controller.DeactivatePlayer();
    }

    public override void StunDisable()
    {
        base.StunDisable();
        controller.ActivatePlayer();
    }

    #endregion

    #region Trigger & Collision

    private void OnTriggerEnter(Collider other)
    {
        // S'il entre dans la zone de dégâts d'un ennemi
        
        // S'il entre dans la zone de réparation d'un bâtiment
        
        // S'il entre dans une zone piégée, enflammée, électrique, etc...
    }

    #endregion
    
}
