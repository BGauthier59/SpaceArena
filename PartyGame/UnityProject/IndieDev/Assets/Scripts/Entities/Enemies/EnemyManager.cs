using UnityEngine;

public class EnemyManager : Entity
{
    public EnemyGenericBehaviour behaviour;
    public int powerUpScore;
    [SerializeField] private Animator mesh;
    public int hitPoint;
    public int deathPoint;

    #region Entity

    public override void TakeDamage(int damage, Entity attacker = null)
    {
        if (mesh != null)
        {
            mesh.Play("OnHit");
        }

        base.TakeDamage(damage, attacker);
        

        if (!attacker) return;
        if (!isDead) return;
        var player = (PlayerManager)attacker;
        if (player == null)
        {
            Debug.LogWarning("Cast did not work?");
            return;
        }
        player.controller.IncreasePowerUpGauge(powerUpScore);
        player.GetPoint(deathPoint, transform.position);
    }

    protected override void Death(Entity killer)
    {
        base.Death(killer);
        partyManager.arenaFeedbackManager.OnExcitementGrows?.Invoke(1);
        gameObject.SetActive(false);
    }

    private void ResetEnemy()
    {
        // Reset enemy after death
       

        Heal(totalLife);
        isDead = false;
    }

    protected override void Heal(int heal)
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

    protected override void StunEnable()
    {
        base.StunEnable();
        behaviour.agent.enabled = false;
        behaviour.enabled = false;
    }

    protected override void StunDisable()
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