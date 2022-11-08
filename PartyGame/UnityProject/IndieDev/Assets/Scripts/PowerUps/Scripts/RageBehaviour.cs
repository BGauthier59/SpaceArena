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

    public override void OnActivate()
    {
        isFinished = false;
        elapsedTime = 0;
        user.ModifySpeed(speedMultiplier);
        user.rageCollider.user = user.GetComponent<Entity>();
        user.rageCollider.damage = damage;
        user.rageCollider.collider.enabled = true;
    }
    

    private void Update()
    {
        if (!isFinished)
        {
            elapsedTime += Time.deltaTime;
            user.playerUI.powerUpSlider.fillAmount = 1 - (elapsedTime / duration);
            if (elapsedTime >= duration)
            {
                isFinished = true;
            }
        }
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
