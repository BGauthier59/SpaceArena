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
            
            controller.ResetRespawn();
        }
        else respawnTimer += Time.deltaTime;

        int time = (int)respawnTimer;
        controller.deathTimerText.text = controller.playerUI.deathTimerUI.text = (respawnDuration - time).ToString();
    }

    public void GetPoint(int point, Vector3 pos)
    {
        score += point;
        if (point > 0) partyManager.DisplayScoreFeedback(point, controller.playerIndex - 1, pos);

        if (score <= 0) score = 0;
        partyManager.OnScoresChange();
    }

    public int ReturnPoint()
    {
        return score;
    }

    public void LinkReferences()
    {
        partyManager = GameManager.instance.partyManager;
    }

    #region Entity

    public override void TakeDamage(int damage, Entity attacker = null)
    {
        base.TakeDamage(damage, attacker);
        GameManager.instance.feedbacks.RumbleConstant(controller.dataGamepad, VibrationsType.TakeDamage);
        controller.playerUI.lifeSlider.value = currentLife;
        if (attacker)
        {
            var attackingPlayer = (PlayerManager)attacker;
            if(attackingPlayer) controller.GetShotByFriendlyFire();
        }
    }

    protected override void Death()
    {
        base.Death();
        partyManager.arenaFeedbackManager.OnExcitementGrows?.Invoke(1);
        controller.ResetDeath();
        GetPoint(deathMalusPoint, Vector3.zero);
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