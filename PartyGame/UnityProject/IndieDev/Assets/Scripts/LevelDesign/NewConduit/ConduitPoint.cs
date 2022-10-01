using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConduitPoint : MonoBehaviour
{
    public Transform pointPos;
    public ConduitPointType conduitPointType;

    // Les points accessibles selon les Inputs (peut être null)
    public ConduitPoint upPoint;
    public ConduitPoint bottomPoint;
    public ConduitPoint leftPoint;
    public ConduitPoint rightPoint;

    [Header("Vent Point")] [Tooltip("For vent point only")]
    public NewVent linkedVent;
    
    public enum ConduitPointType
    {
        VentPoint,
        IntersectionPoint
    }
}
