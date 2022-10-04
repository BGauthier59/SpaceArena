using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public Language currentLanguage;
    public bool rumbleActivated;
}

public enum Language
{
    French, English
}
