using System;
using UnityEngine;

public class RandomEventManager : MonoBehaviour
{
    public RandomEvent[] events;
    public RandomEvent currentEvent;
    public CameraZoom randomEventZoom;
    private WavesManager wavesManager;

    private bool isWaitingForNextEvent;

    public void Initialization()
    {
        wavesManager = GameManager.instance.partyManager.wavesManager;
        isWaitingForNextEvent = false;
        
        // Set first event timer
    }

    // Quand start un nouvel event random ?
    
    // Waves Manager doit être en isWaitingForNextWave (pas d'event pendant le spawn d'une wave)
    
    // Conséquence d'un event aléatoire ?
    // Durant la cinématique de l'évent (camera change, screen animations), le timer du Waves Manager est en pause
    
    public void StartNewRandomEvent()
    {
        //currentEvent = events[0]; // Pour l'instant
        
        GameManager.instance.partyManager.cameraManager.SetZoom(randomEventZoom);
        currentEvent?.StartEvent();
    }

    private void Update()
    {
        if (!isWaitingForNextEvent) return;
    }
}
