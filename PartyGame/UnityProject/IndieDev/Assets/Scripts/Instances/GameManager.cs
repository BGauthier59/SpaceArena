using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Variables: Instances

    [Header("Instances")] public static GameManager instance;
    public SettingsManager settings;
    public FeedbacksManager feedbacks;
    public PartyManager partyManager;

    #endregion

    #region Variables: Data

    [Header("Data")] public EventSystem eventSystem;
    private Gamepad _mainGamepad;
    public string mainName;

    public Gamepad mainGamepad
    {
        get { return _mainGamepad; }
        set
        {
            _mainGamepad = value;
            mainName = _mainGamepad.name;
            /*
            mainGamepadInfo.linked = _mainGamepad != null;
            mainGamepadInfo.name = _mainGamepad == null ? "[Not linked]" : _mainGamepad.name;
            */
        }
    }

    public List<GamepadSerializable> allGamepads;
    public string gamepadSettings;
    public bool mainGamepadOnly;
    public int playersNumber;
    public PlayerInputManager playerInputManager;
    public List<PlayerController> allPlayers;
    public List<PowerUpManager> powerUps;
    public List<TranslatableText> allTranslatableTexts;
    public MainMenuManager.ArenasPanel currentPanel;
    public bool isPaused;

    #endregion

    #region Variables: Parameters

    [Header("Players Parameters")] public Color[] colors = new Color[4];

    [Header("Scenes Indexes")] public int mainMenuIndex;
    public int pauseMenuIndex;
    public int optionMenuIndex;

    #endregion
    
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

    #region Translation Management

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

    #endregion

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

        foreach (var gs in allGamepads)
        {
            gs.isMain = gs.g == mainGamepad;
        }
    }

    public void EnableMainControllerOnly()
    {
        gamepadSettings = "EnableMainControllerOnly";
        mainGamepadOnly = true;
        allGamepads.Clear();

        foreach (var gamepad in Gamepad.all)
        {
            if (gamepad == mainGamepad)
            {
                InputSystem.EnableDevice(gamepad);
                Debug.Log($"Enabled {gamepad.name}!");
                
                allGamepads.Add(new GamepadSerializable()
                {
                    g = gamepad,
                    enabled = gamepad.enabled,
                    name = gamepad.name,
                    isMain = true
                });
                continue;
            }

            InputSystem.DisableDevice(gamepad);
            Debug.Log($"Disabled {gamepad.name}!");
            
            allGamepads.Add(new GamepadSerializable()
            {
                g = gamepad,
                enabled = gamepad.enabled,
                name = gamepad.name,
                isMain = false
            });
        }
    }

    public void EnableAllControllers()
    {
        gamepadSettings = "EnableAllControllers";

        mainGamepadOnly = false;
        allGamepads.Clear();

        foreach (var gamepad in Gamepad.all)
        {
            InputSystem.EnableDevice(gamepad);
            Debug.Log($"Enabled {gamepad.name}!");
            
            allGamepads.Add(new GamepadSerializable()
            {
                g = gamepad,
                enabled = gamepad.enabled,
                name = gamepad.name,
                isMain = gamepad == mainGamepad
            });
        }
    }

    public void DisableAllControllers()
    {
        gamepadSettings = "DisableAllControllers";

        mainGamepadOnly = false;
        allGamepads.Clear();

        foreach (var gamepad in Gamepad.all)
        {
            InputSystem.DisableDevice(gamepad);
            Debug.Log($"Disabled {gamepad.name}!");
            
            allGamepads.Add(new GamepadSerializable()
            {
                g = gamepad,
                enabled = gamepad.enabled,
                name = gamepad.name,
                isMain = gamepad == mainGamepad
            });
        }
    }

    #endregion
    
    [Serializable]
    public class GamepadSerializable
    {
        public Gamepad g;
        public string name;
        public bool enabled;
        public bool isMain;
    }
}

[Serializable]
public class GamepadData
{
    public Gamepad gamepad;
    public bool isRumbling;

    public RumblePattern activeRumblePattern;
    public float rumbleDuration;
    public float pulseDuration;
    public float lowA;
    public float highA;
    public float rumbleStep;
    public bool isMotorActive;
}

