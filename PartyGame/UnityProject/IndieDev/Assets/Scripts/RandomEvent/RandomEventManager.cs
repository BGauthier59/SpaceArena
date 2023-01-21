using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomEventManager : MonoBehaviour
{
    public List<RandomEvent> events;
    public List<RandomEvent> currentEvents;
    private RandomEvent lastStartedEvent;
    public CameraZoom randomEventZoom;
    private NewWavesManager wavesManager;

    public bool isStartingEvent;
    [SerializeField] private GameObject randomEventTimerArea;
    [SerializeField] private TextMeshProUGUI randomEventTimerText;

    private bool begun;
    [SerializeField] private float durationBeforeFirstEvent;
    private float timerBeforeFirstEvent;

    [SerializeField] private float cancellationTime;
    private bool canHappen;

    [SerializeField] private float durationBeforeNextEvent;
    private float timerBeforeNextEvent;

    public bool isEventRunning;

    [SerializeField] private float startingEventDuration;

    private PartyManager partyManager;

    private void Start()
    {
        if (!GameManager.instance.settings.randomEventsOccur)
        {
            enabled = false;
        }
    }

    public void Initialization()
    {
        currentEvents = new List<RandomEvent>();
        partyManager = GameManager.instance.partyManager;
        wavesManager = partyManager.wavesManager;
        isStartingEvent = false;
        begun = true;
    }

    public void StartRandomEventManager()
    {
        begun = false;
        canHappen = true;
    }

    private void Update()
    {
        if (!canHappen || partyManager.gameState == PartyManager.GameState.End) return;

        CheckFirstTimer();
        CheckTimer();
        CheckEventDuration();
        UpdateTimer();
    }

    public void CheckPartyTimer(float remainingTime)
    {
        if (!canHappen || remainingTime > cancellationTime) return;
        canHappen = false;
        Debug.Log($"Cancels Random Event Manager at {remainingTime} seconds!");
    }

    private void UpdateTimer()
    {
        if (currentEvents.Count == 0) return;
        var ev = currentEvents[0];
        randomEventTimerText.text = ((int) (ev.eventDuration - ev.eventTimer)).ToString();
    }

    private void CheckFirstTimer()
    {
        if (begun) return;

        if (timerBeforeFirstEvent >= durationBeforeFirstEvent)
        {
            timerBeforeFirstEvent = 0f;
            timerBeforeNextEvent = 0f;
            StartNewRandomEvent();
            begun = true;
        }
        else timerBeforeFirstEvent += Time.deltaTime;
    }

    private void CheckTimer()
    {
        if (!begun) return;
        if (isStartingEvent) return;
        if (wavesManager.isSpawning) return;

        if (timerBeforeNextEvent >= durationBeforeNextEvent)
        {
            StartNewRandomEvent();
        }
        else timerBeforeNextEvent += Time.deltaTime;
    }

    private void CheckEventDuration()
    {
        if (currentEvents.Count == 0) return;
        if (!isEventRunning) return;

        for (int i = 0; i < currentEvents.Count; i++)
        {
            var re = currentEvents[i];

            if (re.eventTimer >= re.eventDuration)
            {
                re.eventTimer = 0f;
                RemoveEvent(re);
            }
            else re.eventTimer += Time.deltaTime;
        }

        if (currentEvents.Count == 0) isEventRunning = false;
    }

    public void RemoveEvent(RandomEvent @event)
    {
        @event.EndEvent();
        currentEvents.Remove(@event);
        if (currentEvents.Count == 0) randomEventTimerArea.SetActive(false);
    }

    private void StartNewRandomEvent()
    {
        // Set Random Event
        RandomEvent newEvent;
        var security = 0;
        do
        {
            newEvent = events[Random.Range(0, events.Count)];

            security++;
            if (security > 100)
            {
                Debug.LogWarning("No Event found. Choosing same event");
                break;
            }
        } while (currentEvents.Contains(newEvent) || lastStartedEvent == newEvent);

        lastStartedEvent = newEvent;
        StartCoroutine(StartingNewEvent(newEvent));
    }

    private IEnumerator StartingNewEvent(RandomEvent @event)
    {
        timerBeforeNextEvent = 0f;
        isStartingEvent = true;

        currentEvents.Add(@event);

        partyManager.cameraManager.SetZoom(randomEventZoom);

        StartCoroutine(partyManager.RandomEventSetDisplay(@event));
        // Afficher l'event sur le main screen

        yield return new WaitForSeconds(startingEventDuration);

        if (partyManager.gameState == PartyManager.GameState.End)
        {
            Debug.Log("Security cancellation");
            yield break;
        }

        isStartingEvent = false;
        isEventRunning = true;
        @event.StartEvent();
        randomEventTimerArea.SetActive(true);
    }

    public void CancelRandomEventManager()
    {
        randomEventTimerArea.SetActive(false);

        if (currentEvents.Count == 0)
        {
            Debug.Log("No event to cancel");
            return;
        }

        foreach (var re in currentEvents)
        {
            re.EndEvent();
        }

        currentEvents.Clear();
        StopAllCoroutines();
        partyManager.SetScreenRandomEvent(false);
    }
}