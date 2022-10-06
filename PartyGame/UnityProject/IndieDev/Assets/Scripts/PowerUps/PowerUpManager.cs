using System;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    public PlayerController user;
    public PowerUps powerUp;
    public Dictionary<PowerUps, PowerUpManager> powerUpScript;

    public enum PowerUps
    {
        Shotgun, Laser, Turret, GrenadeLauncher, Rage, Tesla, Overload, Wormhole 
    }

    public virtual void OnActivate()
    {
        
    }

    public virtual void OnUse()
    {
        
    }
    
    public virtual bool OnConditionCheck()
    {
        return true;
    }
}

public class ShotgunBehaviour : PowerUpManager
{
    
}

public class LaserBehaviour : PowerUpManager
{
    
}

public class TurretBehaviour : PowerUpManager
{
    
}

public class GrenadeLauncherBehaviour : PowerUpManager
{
    
}

public class RageBehaviour : PowerUpManager
{
    
}

public class TeslaBehaviour : PowerUpManager
{
    
}

public class OverloadBehaviour : PowerUpManager
{
    
}

public class WormholeBehaviour : PowerUpManager
{
    
}


