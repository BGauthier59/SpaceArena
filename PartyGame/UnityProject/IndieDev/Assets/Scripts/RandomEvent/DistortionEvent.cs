using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Mathematics;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable]
public class DistortionEvent : RandomEvent
{
    [SerializeField] private VolumeProfile globalVolume;
    private LensDistortion lensDistortion;

    [SerializeField] private float2 distortionValuesInit;
    private float2 distortionValues;

    [SerializeField] private float distortionDuration;
    private float distortionTimer;


    public override void StartEvent()
    {
        // Lance FX de fumée
        Debug.Log("Distortion Event starts!");
        
        if (!globalVolume.TryGet(out lensDistortion)) Debug.LogError("No Lens Distorsion found");

        distortionValues.x = distortionValuesInit.x;
        distortionValues.y = distortionValuesInit.y;

        isRunning = true;
    }

    private void Update()
    {
        if (isRunning) ModifyLensDistortion();
    }

    private void ModifyLensDistortion()
    {
        if (distortionTimer >= distortionDuration)
        {
            var x = distortionValues.x;
            var y = distortionValues.y;

            distortionValues.x = y;
            distortionValues.y = x;

            distortionTimer = 0f;
        }
        else
        {
            lensDistortion.intensity.Override(Mathf.Lerp(distortionValues.x, distortionValues.y,
                distortionDuration / distortionTimer));

            distortionTimer += Time.deltaTime;
        }
    }

    public override void EndEvent()
    {
        Debug.Log("Distortion Event ends!");

        lensDistortion.intensity.Override(0f);
        
        isRunning = false;
    }
}