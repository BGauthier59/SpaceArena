using UnityEngine;

[CreateAssetMenu(order = 1, fileName = "New Vibrations", menuName = "Vibrations")]
public class Vibrations : ScriptableObject
{
    [Header("Constant Data")]
    public float rumbleDuration;
    public float low;
    public float high;
    
    [Header("Pulse Data")]
    public float pulseDuration;
    public float rumbleStep;
}

public enum VibrationsType
{
    MainConnection, Connection, Reparation, TakeDamage, Shoot, CantShoot, ControllableTurretShoot, EnterTurret
}

