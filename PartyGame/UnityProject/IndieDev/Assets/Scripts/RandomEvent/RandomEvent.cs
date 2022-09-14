using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RandomEvent
{
    // Que font les random events ?
    
    
    // Ouvre des trappes dans le sol ?
    // Inverse les contrôles ?
    // Fumée qui brouille la vue ?
    
    // Point commun : duration, activation de script


    public float eventDuration;
    public float eventTimer;

    public abstract void StartEvent();

    public abstract void EndEvent();

}
