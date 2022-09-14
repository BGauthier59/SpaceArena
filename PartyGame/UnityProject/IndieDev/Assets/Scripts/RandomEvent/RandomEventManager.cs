using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomEventManager : MonoBehaviour
{
    public static RandomEventManager instance;
    
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            DestroyImmediate(this);
            return;
        }

        instance = this;
    }

    public RandomEvent currentEvent;

    public void StartNewRandomEvent()
    {
        currentEvent.StartEvent();
    }

    private void Update()
    {
        if (currentEvent == null) return;

        if (currentEvent.eventTimer >= currentEvent.eventDuration)
        {
            currentEvent.EndEvent();
            currentEvent.eventTimer = 0;
            currentEvent = null;
        }
        else currentEvent.eventTimer += Time.deltaTime;
    }
}
