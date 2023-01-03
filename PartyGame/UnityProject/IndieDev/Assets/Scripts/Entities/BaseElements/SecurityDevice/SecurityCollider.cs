using UnityEngine;

public class SecurityCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.GetComponent<Entity>()) return;
        Debug.Log("Dealt 999 damage to an entity!");
        other.GetComponent<Entity>().TakeDamage(999);
    }
}
