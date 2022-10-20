using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseHeart : BaseElementManager
{
    public override void TakeDamage(int damage, Entity attacker = null)
    {
        base.TakeDamage(damage, attacker);
    }

    protected override void SetBaseElementColor()
    {
        color = partyManager.baseManager.baseHeartColor;
        foreach (var rd in elementColorRenderers)
        {
            rd.material = partyManager.baseManager.colorVariantMaterial;
            rd.material.color = color;
            rd.material.SetColor("_EmissionColor", color * 2);
        }
    }

    protected override void OnDestroyed()
    {
        base.OnDestroyed();
        
        if (partyManager == null)
        {
            Debug.LogError("Party Manager is null!");
            return;
        }
        partyManager.EndingGame(false);
    }

    protected override void OnFixed()
    {
        // Can't be fixed...
    }
}
