using UnityEngine;

public class RandomEventManager : MonoBehaviour
{
    public RandomEvent[] events;
    public RandomEvent currentEvent;
    public CameraZoom randomEventZoom;

    public void StartNewRandomEvent()
    {
        //currentEvent = events[0]; // Pour l'instant
        
        GameManager.instance.partyManager.cameraBehaviour.SetZoom(randomEventZoom);
        currentEvent?.StartEvent();
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
