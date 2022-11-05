using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public Language currentLanguage;
    public bool rumbleActivated;
    [Tooltip("Set to false for a prototype version of the game without any random event")] public bool randomEventsOccur;
}

public enum Language
{
    French, English
}
