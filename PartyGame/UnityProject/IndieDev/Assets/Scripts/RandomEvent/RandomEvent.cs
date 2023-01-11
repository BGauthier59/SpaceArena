using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public abstract class RandomEvent : MonoBehaviour
{
    public PartyManager.TextOnDisplay randomEventText;
    public TextMeshPro additionalText;

    public Sprite randomEventSprite;

    public float eventDuration;
    public float eventTimer;
    protected bool isRunning;

    protected PartyManager partyManager;

    private void Start()
    {
        partyManager = GameManager.instance.partyManager;
        //partyManager.randomEventManager.events.Add(this);
    }

    public abstract void StartEvent();

    public abstract void EndEvent();

    public virtual void SetAdditionalInfo()
    {
        
    }

    public void DisableAdditionalInfo()
    {
        additionalText.text = string.Empty;
    }

}
