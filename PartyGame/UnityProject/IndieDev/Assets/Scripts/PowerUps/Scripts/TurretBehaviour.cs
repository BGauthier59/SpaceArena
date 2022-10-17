using UnityEngine;

public class TurretBehaviour : PowerUpManager
{
    [SerializeField] private TurretScript turret;

    public override void OnActivate()
    {
        base.OnActivate();
    }

    public override void OnUse()
    {
        TurretScript newTurret = Instantiate(turret, transform.position, Quaternion.identity, null);
        newTurret.user = user;
    }
}
