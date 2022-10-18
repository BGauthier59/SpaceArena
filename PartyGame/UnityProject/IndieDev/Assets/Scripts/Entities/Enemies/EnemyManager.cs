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

        base.TakeDamage(damage, attacker);
        if (mesh != null)
        {
            mesh.Play("OnHit");
        }

        if (!attacker) return;
        var player = (PlayerManager)attacker;
        if (player == null)
        {
            Debug.LogWarning("Cast did not work?");
            return;
        }

        int point;
        if (currentLife == 0)
        {
            player.controller.IncreasePowerUpGauge(powerUpScore);
            point = deathPoint;
        }
        else
        {
            point = hitPoint;
        }
        
        player.GetPoint(point);
    }

    protected override void Death()
    {
        base.Death();
        ResetEnemy();

        // Pour l'instant : non
        //PoolOfObject.Instance.SpawnFromPool(PoolType.EnemyDeath, transform.position, Quaternion.identity);
    }

    private void ResetEnemy()
    {
        // Reset enemy after death

        Heal(totalLife);
        isDead = false;
        gameObject.SetActive(false);
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
    }

    private void OnDisable()
    {
        if (partyManager == null) return;
        partyManager.enemiesManager.RemoveEnemy(this);
    }
}