public class BreakerManager : EnemyManager
{
    public override void TakeDamage(int damage, Entity attacker = null)
    {
        base.TakeDamage(damage, attacker);
        ((BreakerBehaviour)behaviour).SwitchState(BreakerBehaviour.BreakerState.ChangeTarget);
    }
    
    protected override void Death(Entity killer)
    {
        base.Death(killer);
        PoolOfObject.Instance.SpawnFromPool(PoolType.CyanSplash, transform.position, attackDirection);
    }
}
