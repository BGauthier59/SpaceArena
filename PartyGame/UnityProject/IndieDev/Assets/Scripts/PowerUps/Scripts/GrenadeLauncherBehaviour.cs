using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeLauncherBehaviour : PowerUpManager
{
    [SerializeField] private float grenadeAmount;
    [SerializeField] private float grenadeSpeed;
    [SerializeField] private float shootingRate;

    public override void OnActivate()
    {
        user.durationBeforeNextShoot = shootingRate;
    }

    public override void OnUse()
    {
        grenadeAmount--;
        var newGrenade =
            PoolOfObject.Instance.SpawnFromPool(PoolType.Grenade, user.transform.position, user.transform.rotation);
        newGrenade.GetComponent<Rigidbody>().AddForce(user.transform.forward * grenadeSpeed);
    }

    public override bool OnConditionCheck()
    {
        if (grenadeAmount <= 0)
        {
            user.durationBeforeNextShoot = 0.1f;
            return true;
        }
        else
        {
            return false;
        }
    }

    private void Update()
    {
        
    }
}
