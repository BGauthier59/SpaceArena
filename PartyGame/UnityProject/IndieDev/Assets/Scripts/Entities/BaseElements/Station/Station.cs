using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Station : BaseElementManager
{
    [SerializeField] private Transform waterPlane;

    private bool waterRising;
    private bool waterLowering;

    private float waterInitPosY;
    [SerializeField] private float waterHigh;

    [SerializeField] private float risingDuration;
    private float risingTimer;

    [SerializeField] private float speedModifier;
    
    public override void Start()
    {
        base.Start();

        waterInitPosY = waterPlane.position.y;
    }

    public override void TakeDamage(int damage, Entity attacker = null)
    {
        Debug.Log("Station is hurt");
        base.TakeDamage(damage, attacker);
    }

    public override void OnDestroyed()
    {
        base.OnDestroyed();

        Debug.Log("Station is dead");

        waterRising = true;
        foreach (var pc in GameManager.instance.allPlayers)
        {
            if (!pc) return;
            pc.ModifySpeed(speedModifier);
        }
    }

    public override void OnFixed()
    {
        base.OnFixed();

        waterLowering = true;
        foreach (var pc in GameManager.instance.allPlayers)
        {
            if (!pc) return;
            pc.ResetSpeed();
        }
    }

    public override void Update()
    {
        base.Update();

        WaterRising();
        WaterLowering();
    }

    private void WaterRising()
    {
        if (!waterRising) return;

        if (risingTimer > risingDuration)
        {
            risingTimer = 0f;
            waterRising = false;
        }
        else
        {
            var posY = Mathf.Lerp(waterInitPosY, waterInitPosY + waterHigh,
                risingTimer / risingDuration);

            waterPlane.position = new Vector3(waterPlane.position.x, posY, waterPlane.position.z);

            risingTimer += Time.deltaTime;
        }
    }

    private void WaterLowering()
    {
        if (!waterLowering) return;

        if (risingTimer > risingDuration)
        {
            risingTimer = 0f;
            waterLowering = false;
        }
        else
        {
            var posY = Mathf.Lerp(waterInitPosY, waterInitPosY + waterHigh,
                1 - (risingTimer / risingDuration));

            waterPlane.position = new Vector3(waterPlane.position.x, posY, waterPlane.position.z);

            risingTimer += Time.deltaTime;
        }
    }
}