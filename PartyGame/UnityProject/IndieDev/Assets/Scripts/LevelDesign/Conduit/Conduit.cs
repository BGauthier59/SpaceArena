using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conduit : MonoBehaviour
{
    [SerializeField] private VentsCouple[] linkedVent;

    [Serializable]
    public struct VentsCouple
    {
        public Vent ventIn;
        public Vent ventOut;
    }

    public float ventingDuration;

    private void Start()
    {
        foreach (var vent in linkedVent)
        {
            vent.ventIn.Initialization(this);
            vent.ventOut.Initialization(this);
        }
    }
}
