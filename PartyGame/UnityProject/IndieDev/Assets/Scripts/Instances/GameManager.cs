using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.XInput;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public SettingsManager settings;
    public FeedbacksManager feedbacks;
    public GameObject mainCanvas;

    public EventSystem eventSystem;
    private Gamepad _mainGamepad;
    public Gamepad mainGamepad
    {
        get { return _mainGamepad; }
        set
        {
            _mainGamepad = value;
            mainGamepadInfo.linked = _mainGamepad != null;
            mainGamepadInfo.name = _mainGamepad == null ? "[Not linked]" : _mainGamepad.name;
        }
    }
    public MainGamepadData mainGamepadInfo;

    [Serializable]
    public struct MainGamepadData
    {
        public bool linked;
        public string name;
    }

    public bool mainGamepadOnly;

    public int playersNumber;
    public PlayerInputManager playerInputManager;

    public List<PlayerController> allPlayers;

    public List<TranslatableText> allTranslatableTexts;

    [Header("Players Parameters")] 
    public Color[] colors = new Color[4];

    [Header("Scenes Index")] 
    public int mainMenuIndex;
    public int pauseMenuIndex;
    public int optionMenuIndex;
    
    public bool isPaused;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            DestroyImmediate(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        EnableAllControllers();
        InputSystem.onDeviceChange += OnDeviceChange;
    }

    private void OnDisable()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;
    }
    
    public void TranslateTexts()
    {
        var textsToDelete = new List<TranslatableText>();

        foreach (var tmp in allTranslatableTexts)
        {
            if (tmp == null)
            {
                textsToDelete.Add(tmp);
                continue;
            }

            switch (settings.currentLanguage)
            {
                case Language.French:
                    tmp.translatableText.text = tmp.frenchText;
                    break;
                case Language.English:
                    tmp.translatableText.text = tmp.englishText;
                    break;
            }
        }

        Debug.Log($"Removing {textsToDelete.Count} elements");
        foreach (var tmp in textsToDelete)
        {
            allTranslatableTexts.Remove(tmp);
        }
    }
    
    #region Gamepads Management

    private void OnDeviceChange(InputDevice device, InputDeviceChange deviceChange)
    {
        if (deviceChange == InputDeviceChange.Disconnected)
        {
            if (device.device == mainGamepad)
            {
                mainGamepad = null;

                foreach (var gamepad in Gamepad.all)
                {
                    if (gamepad == null) continue;
                    SetMainGamepad(gamepad);
                    if (mainGamepadOnly) EnableMainControllerOnly();
                    return;
                }
            }
            else
            {
                Debug.Log("A Gamepad has been disconnected (not main)");
            }
        }
        else if (deviceChange == InputDeviceChange.Reconnected || deviceChange == InputDeviceChange.Added)
        {
            if (Gamepad.all.Contains(device.device))
            {
                if (mainGamepad != null)
                {
                    if (device == mainGamepad) return;

                    if (mainGamepadOnly) EnableMainControllerOnly();
                    else EnableAllControllers();
                }
                else
                {
                    if (SceneManager.GetActiveScene().buildIndex == 0)
                    {
                        Debug.Log("Can't be set as main on first screen");
                        return;
                    }

                    SetMainGamepad((Gamepad) device.device);
                    if (mainGamepadOnly) EnableMainControllerOnly();
                    else EnableAllControllers();
                }
            }
            else
            {
                Debug.Log("New connected device is not a gamepad");
            }
        }
    }

    public void SetMainGamepad(Gamepad gamepad)
    {
        mainGamepad = gamepad;
        Debug.Log($"{mainGamepad.name} is now the main!");
    }

    public void EnableMainControllerOnly()
    {
        mainGamepadOnly = true;

        foreach (var gamepad in Gamepad.all)
        {
            if (gamepad == mainGamepad)
            {
                InputSystem.EnableDevice(gamepad);
                Debug.Log($"Enabled {gamepad.name}!");
                continue;
            }
            InputSystem.DisableDevice(gamepad);
            Debug.Log($"Disabled {gamepad.name}!");
        }
    }

    public void EnableAllControllers()
    {
        mainGamepadOnly = false;

        foreach (var gamepad in Gamepad.all)
        {
            InputSystem.EnableDevice(gamepad);
            Debug.Log($"Enabled {gamepad.name}!");
        }
    }

    #endregion
}