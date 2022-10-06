using UnityEngine;

public class BaseElementTrigger : MonoBehaviour
{
    [SerializeField] private BaseElementManager baseElement;

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        baseElement.SetBaseElementInfo(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        baseElement.SetBaseElementInfo(false);
    }
}