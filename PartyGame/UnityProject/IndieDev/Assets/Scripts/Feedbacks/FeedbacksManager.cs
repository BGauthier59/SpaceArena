using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FeedbacksManager : MonoBehaviour
{
    public Vibrations mainVibration;
    public Vibrations connectionVibration;
    public Vibrations reparationVibration;
    public Vibrations takeDamageVibration;
    public Vibrations shootVibrations;
    
    public Vibrations GetVibrations(VibrationsType type)
    {
        switch (type)
        {
            case VibrationsType.MainConnection:
                return mainVibration;
            case VibrationsType.Connection:
                return connectionVibration;
            case VibrationsType.Reparation:
                return reparationVibration;
            case VibrationsType.TakeDamage:
                return takeDamageVibration;
            case VibrationsType.Shoot:
                return shootVibrations;
        }

        Debug.LogError("Hasn't found any vibrations");
        return null;
    }
    
    public void RumbleConstant(GamepadData g, VibrationsType type)
    {
        if (g.isRumbling) return;

        var data = GetVibrations(type);
        
        g.isRumbling = true;
        g.activeRumblePattern = RumblePattern.Constant;
        g.lowA = data.low;
        g.highA = data.high;
        g.rumbleDuration = Time.time + data.rumbleDuration;
    }

    public void RumblePulse(GamepadData g, VibrationsType type)
    {
        if (g.isRumbling) return;

        var data = GetVibrations(type);
        
        g.isRumbling = true;
        g.activeRumblePattern = RumblePattern.Pulse;

        g.lowA = data.low;
        g.highA = data.high;

        g.rumbleStep = data.rumbleStep;
        g.pulseDuration = Time.time + data.rumbleStep;
        g.rumbleDuration = Time.time + data.rumbleDuration;

        g.isMotorActive = true;

        g.gamepad?.SetMotorSpeeds(g.lowA, g.highA);
    }

    public void StopRumble(GamepadData g)
    {
        g.gamepad?.SetMotorSpeeds(0, 0);
        g.isRumbling = false;
    }

    private void Update()
    {
        foreach (var player in GameManager.instance.allPlayers)
        {
            var g = player.dataGamepad;
            
            if (Time.time > g.rumbleDuration)
            {
                StopRumble(g);
                continue;
            }
            
            if (g.gamepad == null) continue;
            
            switch (g.activeRumblePattern)
            {
                case RumblePattern.Constant:
                    g.gamepad.SetMotorSpeeds(g.lowA, g.highA);
                    break;
            
                case RumblePattern.Pulse:
                    if (Time.time > g.pulseDuration)
                    {
                        g.isMotorActive = !g.isMotorActive;
                        g.pulseDuration = Time.time + g.rumbleStep;
                        if (!g.isMotorActive) g.gamepad.SetMotorSpeeds(0, 0);
                        else g.gamepad.SetMotorSpeeds(g.lowA, g.highA);
                    }
                    break;
            }
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        foreach (var player in GameManager.instance.allPlayers)
        {
            StopRumble(player.dataGamepad);
        }
    }
}

public enum RumblePattern
{
    Constant,
    Pulse,
}
