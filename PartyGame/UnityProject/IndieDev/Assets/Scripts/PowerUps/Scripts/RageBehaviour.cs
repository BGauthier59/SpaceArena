using System.Collections;
using UnityEngine;

public class RageBehaviour : PowerUpManager
{
    [SerializeField] private float duration;
    [SerializeField] private int damage;
    [SerializeField] private float speedMultiplier;
    [SerializeField] private bool isFinished;

    public override void OnActivate()
    {
        isFinished = false;
        StartCoroutine(Duration());
        user.ModifySpeed(speedMultiplier);
        user.rageCollider.user = user.GetComponent<Entity>();
        user.rageCollider.damage = damage;
        user.rageCollider.collider.enabled = true;
    }

    private IEnumerator Duration()
    {
        yield return new WaitForSeconds(duration);
        isFinished = true;
    }

    public override bool OnConditionCheck()
    {
        if (isFinished)
        {
            Debug.Log("Rage is finished");
            user.ResetSpeed();
            user.rageCollider.collider.enabled = false;
        }
        
        return isFinished;
    }
}
