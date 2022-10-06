using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(order = 2, fileName = "New Zoom", menuName = "Zooms")]
public class CameraZoom : ScriptableObject
{
    public PossibleTarget target;

    public Vector3 offset;
    public float offsetSpeed;
    public Vector3 eulerAngles;
    public float eulerAnglesSpeed;
    public float fieldOfView;
    public float fovSpeed;
    public bool hasDuration;
    [Range(0.1f, 10f)] public float zoomDuration;
}

public enum PossibleTarget
{
    Players,
    Host
}