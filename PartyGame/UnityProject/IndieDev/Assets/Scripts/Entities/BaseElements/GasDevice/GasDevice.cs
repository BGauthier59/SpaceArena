using UnityEngine;

public class GasDevice : BaseElementManager
{
    [SerializeField] private GasExplosion linkedExplosion;
    public bool isBlowingUp;

    [SerializeField] private float explosionDuration;
    private float explosionTimer;

    [SerializeField] private float explosionInitRadius;
    [SerializeField] private float explosionMaxRadius;

    public override void TakeDamage(int damage, Entity attacker = null)
    {
        Debug.Log("Gas Device is hurt");
        base.TakeDamage(damage, attacker);
    }

    public override void OnDestroyed()
    {
        base.OnDestroyed();

        Debug.Log("Gas Device is dead");

        // Explosion
        
        isBlowingUp = true;
        linkedExplosion.areaCollider.enabled = true;
        // FX Explosion
    }

    public override void OnFixed()
    {
        base.OnFixed();

    }

    public override void Update()
    {
        base.Update();
        
        BlowingUp();
    }

    public void BlowingUp()
    {
        if (!isBlowingUp) return;

        if (explosionTimer >= explosionDuration)
        {
            linkedExplosion.areaCollider.enabled = false;
            linkedExplosion.areaCollider.radius = explosionInitRadius;
            explosionTimer = 0f;
            isBlowingUp = false;
        }
        else
        {
            linkedExplosion.areaCollider.radius = Mathf.Lerp(explosionInitRadius, explosionMaxRadius,
                explosionTimer / explosionDuration);
            explosionTimer += Time.deltaTime;
        }
    }
}