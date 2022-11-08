using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeslaBehaviour : PowerUpManager
{
    [SerializeField] private float maxRange;
    [SerializeField] private int damage;
    [SerializeField] private float duration;
    private bool isFinished;

    public override void OnActivate(PlayerController player)
    {
        base.OnActivate(player);
        isFinished = false;
        StartCoroutine(Duration());
    }

    public override void OnUse()
    {
        
    }

    public override bool OnConditionCheck()
    {
        return isFinished;
    }

    private IEnumerator Duration()
    {
        yield return new WaitForSeconds(duration);
        isFinished = true;
    }
}
