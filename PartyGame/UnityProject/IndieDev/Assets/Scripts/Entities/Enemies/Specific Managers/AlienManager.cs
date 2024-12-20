using UnityEngine;

public class AlienManager : EnemyManager
{
    public override void TakeDamage(int damage, Entity attacker = null)
    {
        base.TakeDamage(damage, attacker);
        ((AlienBehaviour)behaviour).SwitchState(AlienBehaviour.AlienState.Retreat);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("BaseElement"))
        {
            var entity = other.GetComponent<Entity>();
            entity.TakeDamage(behaviour.damage);
        }
    }
    
    protected override void Death(Entity killer)
    {
        base.Death(killer);
        PoolOfObject.Instance.SpawnFromPool(PoolType.YellowSplash, transform.position, attackDirection);
    }
}
