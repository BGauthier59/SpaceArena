using UnityEngine;

public class EnemyManager : Entity
{
    public EnemyGenericBehaviour behaviour;
    public int powerUpScore;
    [SerializeField] private Animator mesh;
    public int hitPoint;
    public int deathPoint;
    public EnemyType enemyType;
    
    public enum EnemyType
    {
        Breaker, Hunter, Base, HeartBreaker 
    }

    #region Entity

    public override void TakeDamage(int damage, Entity attacker = null)
    {
        if (mesh != null)
        {
            mesh.Play("OnHit");
        }

        base.TakeDamage(damage, attacker);
        switch (enemyType)
        {
            case EnemyType.Base :
                PoolOfObject.Instance.SpawnFromPool(PoolType.YellowSplash, transform.position, attackDirection);
                break;
            
            case EnemyType.Breaker :
                PoolOfObject.Instance.SpawnFromPool(PoolType.CyanSplash, transform.position, attackDirection);
                break;
            
            case EnemyType.HeartBreaker :
                PoolOfObject.Instance.SpawnFromPool(PoolType.PinkSplash, transform.position, attackDirection);
                break;
            
            case EnemyType.Hunter :
                PoolOfObject.Instance.SpawnFromPool(PoolType.PurpleSplash, transform.position, attackDirection);
                break;
        }

        if (!attacker) return;
        if (!isDead) return;
        var player = (PlayerManager)attacker;
        if (player == null)
        {
            Debug.LogWarning("Cast did not work?");
            return;
        }
        player.controller.IncreasePowerUpGauge(powerUpScore);
        player.GetPoint(deathPoint);
    }

    protected override void Death()
    {
        base.Death();
        gameObject.SetActive(false);

        // Pour l'instant : non
        //PoolOfObject.Instance.SpawnFromPool(PoolType.EnemyDeath, transform.position, Quaternion.identity);
    }

    private void ResetEnemy()
    {
        // Reset enemy after death

        Heal(totalLife);
        isDead = false;
    }

    public override void Heal(int heal)
    {
        base.Heal(heal);
    }

    public override void Start()
    {
        base.Start();
        if (partyManager == null) return;
        partyManager.enemiesManager.AddEnemy(this);
        //Destroy(gameObject, 2f);
    }

    public override void Update()
    {
        base.Update();
    }

    public override void StunEnable()
    {
        base.StunEnable();
        behaviour.agent.enabled = false;
        behaviour.enabled = false;
    }

    public override void StunDisable()
    {
        base.StunDisable();
        behaviour.agent.enabled = true;
        behaviour.enabled = true;
    }

    #endregion

    private void OnEnable()
    {
        if (partyManager == null) return;
        partyManager.enemiesManager.AddEnemy(this);
        ResetEnemy();
    }

    private void OnDisable()
    {
        if (partyManager == null) return;
        partyManager.enemiesManager.RemoveEnemy(this);
    }
}