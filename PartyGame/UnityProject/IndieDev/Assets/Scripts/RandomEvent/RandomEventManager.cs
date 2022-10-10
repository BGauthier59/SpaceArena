using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomEventManager : MonoBehaviour
{
    public RandomEvent[] events;
    public List<RandomEvent> currentEvents;
    public CameraZoom randomEventZoom;
    private WavesManager wavesManager;

    public bool isRunningEvent;
    public bool isStartingEvent;

    [SerializeField] private float eventDuration;
    private float eventTimer;

    [SerializeField] private float startingEventDuration;
    private float startingEventTimer;

    public void Initialization()
    {
        currentEvents = new List<RandomEvent>();
        wavesManager = GameManager.instance.partyManager.wavesManager;
        isStartingEvent = false;

        // Set first event timer
    }

    private void Update()
    {
        if (!isRunningEvent) return;
        if (startingEventTimer >= startingEventDuration)
        {
            isStartingEvent = false;
        }
        else startingEventTimer += Time.deltaTime;

        if (!isStartingEvent) return;
        if (wavesManager.isSpawning) return;

        if (eventTimer >= eventDuration)
        {
            StartNewRandomEvent();
        }
        else eventTimer += Time.deltaTime;
    }

    public void StartNewRandomEvent()
    {
        startingEventTimer = 0f;
        eventTimer = 0f;
        isStartingEvent = true;
        isRunningEvent = true;

        GameManager.instance.partyManager.cameraManager.SetZoom(randomEventZoom);

        // Set Random Event
        RandomEvent newEvent;
        do
        {
            newEvent = events[Random.Range(0, events.Length)];
        } while (events.Contains(newEvent));

        currentEvents.Add(newEvent);


        newEvent.StartEvent();

        isStartingEvent = false;
    }
}