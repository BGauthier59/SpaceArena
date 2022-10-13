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
        color = GameManager.instance.partyManager.baseManager.baseHeartColor;
        foreach (var rd in elementColorRenderers)
        {
            rd.material = GameManager.instance.partyManager.baseManager.colorVariantMaterial;
            rd.material.color = color;
            rd.material.SetColor("_EmissionColor", color * 2);
        }
    }

    protected override void OnDestroyed()
    {
        base.OnDestroyed();
        
        if (GameManager.instance.partyManager == null)
        {
            Debug.LogError("Party Manager is null!");
            return;
        }
        GameManager.instance.partyManager.EndingGame(false);
    }

    protected override void OnFixed()
    {
        // Can't be fixed...
    }
}
