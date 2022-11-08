using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeLauncherBehaviour : PowerUpManager
{
    [SerializeField] private float grenadeAmount;
    [SerializeField] private float maxGrenadeAmount;
    [SerializeField] private float grenadeSpeed;
    [SerializeField] private float shootingRate;

    public override void OnActivate()
    {
        user.shootCooldownDuration = shootingRate;
        grenadeAmount = maxGrenadeAmount;
    }

    public override void OnUse()
    {
        grenadeAmount--;
        user.playerUI.powerUpSlider.fillAmount = grenadeAmount / maxGrenadeAmount;
        var newGrenade =
            PoolOfObject.Instance.SpawnFromPool(PoolType.Grenade, user.transform.position, user.transform.rotation);
        newGrenade.GetComponent<Rigidbody>().AddForce(user.transform.forward * grenadeSpeed);
    }

    public override bool OnConditionCheck()
    {
        if (grenadeAmount <= 0)
        {
            user.shootCooldownDuration = 0.1f;
            user = null;
            return true;
        }
        return false;
    }

    private void Update()
    {
        
    }
}
