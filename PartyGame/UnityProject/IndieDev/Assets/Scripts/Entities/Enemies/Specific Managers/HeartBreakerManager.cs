public class HeartBreakerManager : EnemyManager
{
    public override void TakeDamage(int damage, Entity attacker = null)
    {
        base.TakeDamage(damage, attacker);
        var hb = (HeartBreakerBehaviour)behaviour;
        hb.lastAttacker = attacker;
        hb.SwitchState(HeartBreakerBehaviour.HeartBreakerState.CallForHelp);
    }
    
    protected override void Death(Entity killer)
    {
        base.Death(killer);
        PoolOfObject.Instance.SpawnFromPool(PoolType.PinkSplash, transform.position, attackDirection);
    }
}
