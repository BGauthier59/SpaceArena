using UnityEngine;

public class RotaryWallArea : MonoBehaviour
{
    [SerializeField] private RotaryWall linkedWall;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            linkedWall.AddEntityToArea(other.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            other.transform.SetParent(null);
        }
    }
}
