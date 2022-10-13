using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricalDevice : BaseElementManager
{
    private Light directionalLight;
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
        public Renderer[] colorElements;

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

    public override void Start()
    {
        base.Start();
        directionalLight = GameManager.instance.partyManager.mainLight;
        foreach (var light in allLights)
        {
            foreach (var rd in light.colorElements)
            {
                rd.material.color = color;
                rd.material.SetColor("_EmissionColor", color * 2);
            }
        }
    }
    
    public override void TakeDamage(int damage, Entity attacker = null)
    {
        base.TakeDamage(damage, attacker);
    }

    protected override void OnDestroyed()
    {
        base.OnDestroyed();

        foreach (var light in allLights)
        {
            light.LightingOff();
        }
        directionalLight.enabled = false;
    }

    protected override void OnFixed()
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
            
        directionalLight.enabled = true;
        isSwitchingLightsOn = false;
    }
}
