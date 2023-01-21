using UnityEngine;

public class RotatingAxe : MonoBehaviour
{
    [SerializeField] private float speed;
    
    private void Update()
    {
        transform.Rotate(Vector3.up * (Time.deltaTime * speed));
    }
}
