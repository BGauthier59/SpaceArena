using UnityEngine;

public class TurretBehaviour : PowerUpManager
{
    [SerializeField] private TurretScript turret;
    private bool turretUsed;
    public override void OnActivate()
    {
        base.OnActivate();
    }

    public override void OnUse()
    {
        Debug.Log("Je pose une tourelle");
        TurretScript newTurret = Instantiate(turret, user.transform.position, Quaternion.identity, null);
        newTurret.user = user;
        turretUsed = true;
    }

    public override bool OnConditionCheck()
    {
        return turretUsed;
    }
}
