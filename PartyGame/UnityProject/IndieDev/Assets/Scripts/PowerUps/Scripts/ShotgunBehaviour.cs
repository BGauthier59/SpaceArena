using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunBehaviour : PowerUpManager
{
    private int bullets;
    [SerializeField] private int maxBullets;
    [SerializeField] private float shootingRate;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private int bulletAmount;
    [SerializeField] private int baseAngle = 90;
    private float angleDelta;

    public override void OnActivate(PlayerController player)
    {
        base.OnActivate(player);
        bullets = maxBullets;
        user = player;
        player.shootCooldownDuration = shootingRate;
        angleDelta = (float)baseAngle / bulletAmount;
    }

    public override void OnUse()
    {
        bullets--;
        user.playerUI.powerUpSlider.fillAmount = (float)bullets / maxBullets;
        var currentAngle = (float)baseAngle / 2;
        for (int i = 0; i < bulletAmount; i++)
        {
            Debug.Log("Je loop");
            var newBullet = PoolOfObject.Instance.SpawnFromPool(PoolType.ShotgunBullet, user.bulletOrigin.position,
                Quaternion.Euler(0, currentAngle + user.transform.eulerAngles.y, 0)).GetComponent<ShotgunBullet>();
            newBullet.shooter = user.manager;
            newBullet.SetBulletColor();
            newBullet.rb.AddForce(newBullet.transform.forward.normalized * bulletSpeed);
            currentAngle -= angleDelta;
        }
    }

    public override bool OnConditionCheck()
    {
        return bullets <= 0;
    }

    public override void OnDeactivate()
    {
        user.ResetShootCooldown();
        base.OnDeactivate();
    }
}