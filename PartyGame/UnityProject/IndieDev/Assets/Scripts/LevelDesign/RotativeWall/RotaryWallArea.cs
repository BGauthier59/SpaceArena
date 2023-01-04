using UnityEngine;

public class RotaryWallArea : MonoBehaviour
{
    [SerializeField] private RotaryWall linkedWall;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("entered");
            linkedWall.AddPlayerToArea(other.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("exited");
            other.transform.SetParent(null);
        }
    }
}
