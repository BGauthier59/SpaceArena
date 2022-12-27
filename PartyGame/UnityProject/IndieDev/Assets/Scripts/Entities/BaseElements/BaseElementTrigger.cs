using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseElementTrigger : MonoBehaviour
{
    [SerializeField] private BaseElementManager baseElement;
    private List<Collider> players = new List<Collider>();
    private bool active;
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        players.Add(other);
        active = true;
    }

    private void FixedUpdate()
    {
        baseElement.SetBaseElementInfo(active);
        SecurityCheck();
    }

    private void SecurityCheck()
    {
        // Si un joueur meurt, on doit l'enlever de la liste

        for (var i = players.Count - 1; i >= 0; i--)
        {
            if (players[i].enabled) continue;
            OnTriggerExit(players[i]);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        players.Remove(other);
        if (players.Count == 0) active = false;
    }
}