using UnityEngine;

public class TurretBehaviour : PowerUpManager
{
    [SerializeField] private TurretScript turret;
    private bool turretUsed;
    
    public override void OnActivate(PlayerController player)
    {
        base.OnActivate(player);
        turretUsed = false;
    }

    public override void OnUse()
    {
        var newTurret = Instantiate(turret, user.transform.position, Quaternion.identity, null);
        newTurret.user = user;
        turretUsed = true;
    }

    public override bool OnConditionCheck()
    {
        return turretUsed;
    }
}
