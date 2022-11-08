using System;
using System.Collections;
using UnityEngine;

public class RageBehaviour : PowerUpManager
{
    [SerializeField] private float duration;
    private float elapsedTime;
    [SerializeField] private int damage;
    [SerializeField] private float speedMultiplier;
    [SerializeField] private bool isFinished;

    public override void OnActivate(PlayerController player)
    {
        base.OnActivate(player);

        isFinished = false;
        elapsedTime = 0;
        user.ModifySpeed(speedMultiplier);
        user.rageCollider.user = user.GetComponent<Entity>();
        user.rageCollider.damage = damage;
        user.rageCollider.collider.enabled = true;
    }
    
    private void Update()
    {
        if (isFinished) return;
        
        if (elapsedTime >= duration)
        {
            isFinished = true;
        }
        else
        {
            elapsedTime += Time.deltaTime;
            user.playerUI.powerUpSlider.fillAmount = 1 - (elapsedTime / duration);
        }
    }

    public override bool OnConditionCheck()
    {
        return isFinished;
    }

    public override void OnDeactivate()
    {
        user.ResetSpeed();
        user.rageCollider.collider.enabled = false;
        base.OnDeactivate();
    }
}
