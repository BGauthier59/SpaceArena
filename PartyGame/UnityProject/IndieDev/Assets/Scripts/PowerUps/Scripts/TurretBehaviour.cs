using UnityEngine;

public class TurretBehaviour : PowerUpManager
{
    [SerializeField] private TurretScript turret;
    private bool turretUsed;

    public override void OnActivate(PlayerController player)
    {
        base.OnActivate(player);
        turretUsed = true;
        var newTurret = Instantiate(turret, user.transform.position, Quaternion.identity, null);
        newTurret.user = user;
    }

    public override bool OnConditionCheck()
    {
        return turretUsed;
    }
}