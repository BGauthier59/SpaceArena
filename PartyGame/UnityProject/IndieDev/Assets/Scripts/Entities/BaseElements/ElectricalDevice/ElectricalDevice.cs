using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricalDevice : BaseElementManager
{
    [SerializeField] private Light directionalLight;
    [SerializeField] private ElectricalLight[] allLights;
    private bool isSwitchingLightsOn;
    
    [Serializable]
    public class ElectricalLight
    {
        public Light light;
        public bool isOn;
        public float lightOnIntensity;
        public float lightingOnDuration;
        private float lightingOnTimer;

        public void LightingOff()
        {
            light.intensity = 0f;
            isOn = false;
        }
        
        public void LightingOn()
        {
            light.intensity = Mathf.Lerp(0, lightOnIntensity, 
                lightingOnTimer / lightingOnDuration);
            
            if (lightingOnTimer >= lightingOnDuration)
            {
                light.intensity = lightOnIntensity;
                lightingOnTimer = 0f;
                isOn = true;
            }
            else lightingOnTimer += Time.deltaTime;
        }
    }
    
    public override void TakeDamage(int damage)
    {
        Debug.Log("Electrical Device is hurt");
        base.TakeDamage(damage);
    }

    public override void OnDestroyed()
    {
        base.OnDestroyed();

        Debug.Log("Electrical Device is dead");

        foreach (var light in allLights)
        {
            light.LightingOff();
        }
        directionalLight.enabled = false;
    }

    public override void OnFixed()
    {
        base.OnFixed();

        isSwitchingLightsOn = true;
    }

    public override void Update()
    {
        base.Update();

        SwitchingLightsOn();
    }

    private void SwitchingLightsOn()
    {
        if (!isSwitchingLightsOn) return;
        foreach (var light in allLights)
        {
            light.LightingOn();
        }

        foreach (var light in allLights)
        {
            if (!light.isOn) return;
        }
            
        Debug.Log("All lights are on!");

        directionalLight.enabled = true;
        isSwitchingLightsOn = false;
    }
}
