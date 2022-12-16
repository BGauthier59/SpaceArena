using UnityEngine;

public class GrenadeLauncherBehaviour : PowerUpManager
{
    [SerializeField] private float grenadeAmount;
    [SerializeField] private float maxGrenadeAmount;
    [SerializeField] private float grenadeSpeed;
    [SerializeField] private float shootingRate;
    private PlayerController player;

    public override void OnActivate(PlayerController player)
    {
        base.OnActivate(player);
        this.player = player;

        user.shootCooldownDuration = shootingRate;
        grenadeAmount = maxGrenadeAmount;
    }

    public override void OnUse()
    {
        grenadeAmount--;
        user.playerUI.powerUpSlider.fillAmount = grenadeAmount / maxGrenadeAmount;
        var newGrenade =
            PoolOfObject.Instance.SpawnFromPool(PoolType.Grenade, user.bulletOrigin.position, user.transform.rotation).GetComponent<GrenadeScript>();
        newGrenade.shooter = player;
        newGrenade.SetBulletColor();
        newGrenade.rb.AddForce(user.transform.forward * grenadeSpeed);
    }

    public override bool OnConditionCheck()
    {
        return grenadeAmount <= 0;
    }
    
    public override void OnDeactivate()
    {
        user.ResetShootCooldown();
        base.OnDeactivate();
    }
}
