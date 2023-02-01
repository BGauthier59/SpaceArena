public class HunterManager : EnemyManager
{
    protected override void Death(Entity killer)
    {
        base.Death(killer);
        PoolOfObject.Instance.SpawnFromPool(PoolType.PurpleSplash, transform.position, attackDirection);
    }
}
