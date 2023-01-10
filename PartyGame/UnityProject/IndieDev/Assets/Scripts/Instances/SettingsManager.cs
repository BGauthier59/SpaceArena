using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public Language currentLanguage;
    public bool rumbleActivated;
    [Tooltip("Set to false for a prototype version of the game without any random event")] public bool randomEventsOccur;
    [Tooltip("Set to false for a prototype version of the game allowing to play solo")]
    public bool multiplayerVersion;

    public int mainVolumeValue;
    public int musicVolumeValue;
    public int sfxVolumeValue;
}

public enum Language
{
    French, English
}
