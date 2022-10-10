using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SmokeEvent : RandomEvent
{
    public override void StartEvent()
    {
        // Lance FX de fumée
        Debug.Log("Smoke Event starts!");
        
    }

    public override void EndEvent()
    {
        Debug.Log("Smoke Event ends!");

        // Enlève fumée
    }
}
