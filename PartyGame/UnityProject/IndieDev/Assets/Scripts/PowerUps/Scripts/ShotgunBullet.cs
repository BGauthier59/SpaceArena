using UnityEngine;

public class ShotgunBullet : MonoBehaviour
{
    public PlayerManager shooter;
    public Rigidbody rb;
    [SerializeField] private int damage;

    [SerializeField] private TrailRenderer bulletLine;
    [SerializeField] private MeshRenderer bulletRenderer;


    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log(collision.name);
        PoolOfObject.Instance.SpawnFromPool(PoolType.Bullet_Impact, transform.position, Quaternion.identity);
        var interactable = collision.GetComponent<IInteractable>();
        interactable?.OnHitByProjectile(transform.forward);

        if (collision.CompareTag("Enemy") || collision.CompareTag("Player"))
        {
            var script = collision.GetComponent<Entity>();
            if (script == shooter) return;
            script.attackDirection = transform.rotation;
            script.TakeDamage(damage, shooter);
        }
        else if (collision.CompareTag("Wall")) return;
        gameObject.SetActive(false);
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void SetBulletColor()
    {
        bulletLine.material.SetColor("_EmissionColor",
            GameManager.instance.colors[shooter.controller.playerIndex - 1] * 2);
        bulletRenderer.material.SetColor("_EmissionColor",
            GameManager.instance.colors[shooter.controller.playerIndex - 1] * 2);
    }

    private void OnDisable()
    {
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
        }
    }
}
