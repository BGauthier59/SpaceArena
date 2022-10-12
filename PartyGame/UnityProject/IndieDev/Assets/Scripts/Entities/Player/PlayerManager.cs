using System;
using UnityEngine;

public class PlayerManager : Entity
{
    public PlayerController controller;
    [Header("Respawn")] [SerializeField] private float respawnDuration;
    private float respawnTimer;
    private bool isRespawning;

    public override void Update()
    {
        base.Update();
        if (isRespawning) Respawn();
    }

    private void Respawn()
    {
        if (respawnTimer > respawnDuration)
        {
            transform.position = controller.initPos;
            controller.rd.gameObject.SetActive(true);
            isDead = false;
            Heal(totalLife);
            respawnTimer = 0f;
            isRespawning = false;
            controller.col.enabled = true;
            controller.directionArrow.enabled = true;
            controller.playerLight.enabled = true;
            controller.SetGaugesState(true);
            controller.ActivatePlayer();
        }
        else respawnTimer += Time.deltaTime;
    }

    #region Entity

    public override void TakeDamage(int damage, Entity attacker = null)
    {
        base.TakeDamage(damage, attacker);
        GameManager.instance.feedbacks.RumbleConstant(controller.dataGamepad, VibrationsType.TakeDamage);
    }

    protected override void Death()
    {
        base.Death();

        if (controller.reparationArea != null && controller.reparationArea.isEveryPlayerOn)
        {
            controller.reparationArea.OnTriggerExit(controller.col);
        }

        controller.DeactivatePlayer();
        isRespawning = true;
        controller.rd.gameObject.SetActive(false);
        controller.col.enabled = false;
        controller.directionArrow.enabled = false;
        controller.playerLight.enabled = false;
        controller.ResetSpeed();
        controller.SetGaugesState(false);
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