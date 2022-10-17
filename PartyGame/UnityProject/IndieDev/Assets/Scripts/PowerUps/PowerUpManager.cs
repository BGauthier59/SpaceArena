using System;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    public PlayerController user;
    
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

public static class PowerUpList
{
    public static PowerUps powerUp;
    public static Dictionary<int, PowerUpManager> powerUpScript = new Dictionary<int, PowerUpManager>()
    {
        {1, new ShotgunBehaviour()},
        {2, new LaserBehaviour()},
        {3, new RageBehaviour()},
        {4, new OverloadBehaviour()},
        {5, new TeslaBehaviour()},
        {6, new TurretBehaviour()},
        {7, new GrenadeLauncherBehaviour()},
        {8, new WormholeBehaviour()}
    };

    public enum PowerUps
    {
        Shotgun, Laser, Turret, GrenadeLauncher, Rage, Tesla, Overload, Wormhole
    }
}

public class ShotgunBehaviour : PowerUpManager
{
    public override void OnUse()
    {
        base.OnUse();
    }
}

public class LaserBehaviour : PowerUpManager
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


