using System.Collections.Generic;
using UnityEngine;

public abstract class PowerUpManager : MonoBehaviour
{
    public PlayerController user;
    public Sprite powerUpImage;

    public virtual void OnActivate(PlayerController player)
    {
        user = player;
        user.powerUpIsActive = true;
        GameManager.instance.partyManager.arenaFeedbackManager.OnExcitementGrows?.Invoke(1);
    } // When player presses power-up activation input

    public virtual void OnUse()
    {
    } // When player presses fire input after activating the power-up

    public abstract bool OnConditionCheck(); // Is called every frame to check if power stops or not

    public virtual void OnDeactivate()
    {
        user.powerUpIsActive = false;
        user = null;
    } // When the stopping condition is checked
}

public static class PowerUpList
{
    public static PowerUps powerUp;

    public static Dictionary<int, PowerUpManager> powerUpScript = new Dictionary<int, PowerUpManager>()
    {
        { 1, new ShotgunBehaviour() },
        { 2, new LaserBehaviour() },
        { 3, new RageBehaviour() },
        { 4, new OverloadBehaviour() },
        { 5, new TeslaBehaviour() },
        { 6, new TurretBehaviour() },
        { 7, new GrenadeLauncherBehaviour() },
        { 8, new WormholeBehaviour() }
    };

    public enum PowerUps
    {
        Shotgun,
        Laser,
        Turret,
        GrenadeLauncher,
        Rage,
        Tesla,
        Overload,
        Wormhole
    }
}

public class LaserBehaviour : PowerUpManager
{
    public override bool OnConditionCheck()
    {
        return true; // Must be modified
    }
}

public class OverloadBehaviour : PowerUpManager
{
    public override bool OnConditionCheck()
    {
        return true; // Must be modified
    }
}

public class WormholeBehaviour : PowerUpManager
{
    public override bool OnConditionCheck()
    {
        return true; // Must be modified
    }
}