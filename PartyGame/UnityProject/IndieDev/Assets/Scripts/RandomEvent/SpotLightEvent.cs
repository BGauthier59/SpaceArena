using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using Random = UnityEngine.Random;

public class SpotLightEvent : RandomEvent
{
    [SerializeField] private Transform rotatingSphere;
    [SerializeField] private float rotatingSpeed;
    [SerializeField] private Light mainLight;
    private float mainLightIntensity;
    [SerializeField] private Light spotLight;
    [SerializeField] private float spotLightIntensity;
    private bool isSwitchingOn;
    [SerializeField] private float spotLightOnDuration;
    private float spotLightOnTimer;
    [SerializeField] private AnimationCurve lightIntensityFactorOverTime;

    private Queue<Color> colors = new Queue<Color>();
    private Color currentColor;
    private Color nextColor;
    private bool isSwitchingColor;
    [SerializeField] private float changeColorDuration;
    private float changeColorTimer;

    private bool isSwitchingOff;

    public override void StartEvent()
    {
        Debug.Log("Spot Light Event starts!");

        rotatingSphere.rotation = Quaternion.identity;
        
        colors.Clear();
        colors.Enqueue(Color.red);
        colors.Enqueue(Color.yellow);
        colors.Enqueue(Color.green);

        mainLightIntensity = mainLight.intensity;
        mainLight.intensity = 0f;

        spotLight.enabled = true;
        isSwitchingOn = true;
        spotLight.intensity = 0f;

        currentColor = Color.blue;
        nextColor = colors.Dequeue();
        spotLight.color = currentColor;

        isRunning = true;
    }

    private void Update()
    {
        if (!isRunning) return;

        RotateSphere();
        if (isSwitchingOn) SwitchingOn();
        if (isSwitchingColor) ChangeLightColor();
        if (isSwitchingOff) SwitchingOff();
    }

    private void RotateSphere()
    {
        rotatingSphere.Rotate(Vector3.up * rotatingSpeed * Time.deltaTime);
    }
    
    private void SwitchingOn()
    {
        if (spotLightOnTimer >= spotLightOnDuration)
        {
            spotLightOnTimer = 0f;
            isSwitchingOn = false;
            spotLight.intensity = spotLightIntensity;
            isSwitchingColor = true;
            SetNextColor();
        }
        else
        {
            spotLight.intensity = Mathf.Lerp(0f,
                spotLightIntensity * lightIntensityFactorOverTime.Evaluate(spotLightOnTimer / spotLightOnDuration),
                spotLightOnTimer / spotLightOnDuration);
            spotLightOnTimer += Time.deltaTime;
        }
    }

    private void SwitchingOff()
    {
        if (spotLightOnTimer >= spotLightOnDuration)
        {
            spotLight.intensity = 0f;
            spotLightOnTimer = 0f;
            isSwitchingOff = false;
            isSwitchingColor = false;
            spotLight.enabled = false;
            mainLight.intensity = mainLightIntensity;
            isRunning = false;
        }
        else
        {
            spotLight.intensity =
                Mathf.Lerp(
                    spotLightIntensity * (1-lightIntensityFactorOverTime.Evaluate(spotLightOnTimer / spotLightOnDuration)),
                    0, spotLightOnTimer / spotLightOnDuration);
            spotLightOnTimer += Time.deltaTime;
        }
    }

    private void SetNextColor()
    {
        colors.Enqueue(nextColor);
        currentColor = nextColor;
        nextColor = colors.Dequeue();
    }

    private void ChangeLightColor()
    {
        if (changeColorTimer >= changeColorDuration)
        {
            changeColorTimer = 0f;
            SetNextColor();
        }
        else
        {
            spotLight.color = Color.Lerp(currentColor, nextColor, changeColorTimer / changeColorDuration);
            changeColorTimer += Time.deltaTime;
        }
    }


    public override void EndEvent()
    {
        Debug.Log("Spot Light Event ends!");
        isSwitchingOff = true;
    }
}