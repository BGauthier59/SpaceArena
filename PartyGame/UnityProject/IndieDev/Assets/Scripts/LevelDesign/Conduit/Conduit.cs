using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conduit : MonoBehaviour
{
    public VentsCouple[] linkedVent;
    [SerializeField] private Renderer[] renderers;
    [SerializeField] private float transparency;
    public List<PlayerController> playersInConduit;

    [Serializable]
    public struct VentsCouple
    {
        public Vent ventIn;
        public Vent ventOut;
    }

    public float ventingDuration;

    private void Start()
    {
        playersInConduit = new List<PlayerController>();
        foreach (var vent in linkedVent)
        {
            vent.ventIn.Initialization(this);
            vent.ventOut.Initialization(this);
        }
        DisableTransparency();
    }

    public void EnableTransparency()
    {
        if (playersInConduit.Count > 1) return;
        
        foreach (var rd in renderers)
        {
            rd.material.color = new Color(1, 1, 1, transparency);
        }
    }

    public void DisableTransparency()
    {
        if (playersInConduit.Count > 0) return;
        
        foreach (var rd in renderers)
        {
            rd.material.color = new Color(1, 1, 1, 1);
        }
    }
}
