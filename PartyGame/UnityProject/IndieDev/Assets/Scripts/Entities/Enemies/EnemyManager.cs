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
        /* Pour l'instant : non
        var damageIndicator = PoolOfObject.Instance.SpawnFromPool(PoolType.Damage, transform.position, Quaternion.identity)
            .GetComponent<TextMeshProUGUI>();
        damageIndicator.rectTransform.SetParent(GameManager.instance.mainCanvas.transform);
        damageIndicator.transform.position = Camera.main.WorldToScreenPoint(transform.position);
        damageIndicator.text = damage.ToString();
        animator.Play("EnemyHit");
        */

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
        if (GameManager.instance.partyManager == null) return;
        GameManager.instance.partyManager.enemiesManager.AddEnemy(this);
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
        if (GameManager.instance.partyManager == null) return;
        GameManager.instance.partyManager.enemiesManager.AddEnemy(this);
        ResetEnemy();
    }

    private void OnDisable()
    {
        if (GameManager.instance.partyManager == null) return;
        GameManager.instance.partyManager.enemiesManager.RemoveEnemy(this);
    }
}