using System;
using UnityEngine;

public class PlayerManager : Entity
{
    public PlayerController controller;
    [Header("Respawn")] [SerializeField] private float respawnDuration;
    private float respawnTimer;
    private bool isRespawning;
    public int score;

    [SerializeField] [Tooltip("Must be negative")]
    private int deathMalusPoint;

    public override void Update()
    {
        base.Update();
        if (isRespawning) Respawn();
    }

    private void Respawn()
    {
        if (respawnTimer > respawnDuration)
        {
            respawnTimer = 0f;
            isRespawning = false;
            isDead = false;
            Heal(totalLife);
            controller.ResetAfterDeath();
        }
        else respawnTimer += Time.deltaTime;
    }

    public void GetPoint(int point)
    {
        score += point;
        if (score <= 0) score = 0;
        GameManager.instance.partyManager.OnScoresChange();
    }

    public int ReturnPoint()
    {
        return score;
    }

    #region Entity

    public override void TakeDamage(int damage, Entity attacker = null)
    {
        base.TakeDamage(damage, attacker);
        GameManager.instance.feedbacks.RumbleConstant(controller.dataGamepad, VibrationsType.TakeDamage);
        if (attacker)
        {
            Debug.Log(attacker.name +  " has attacked a player!");
        }
    }

    protected override void Death()
    {
        base.Death();
        
        controller.ResetWhenDeath();
        GetPoint(deathMalusPoint);
        isRespawning = true;
        
    }

    public override void Heal(int heal)
    {
        base.Heal(heal);
    }

    public override void Fall()
    {
        controller.CancelDash();
        base.Fall();
    }

    public override void StunEnable()
    {
        base.StunEnable();
        controller.DeactivatePlayer();
    }

    public override void StunDisable()
    {
        base.StunDisable();
        controller.ActivatePlayer();
    }

    #endregion
}