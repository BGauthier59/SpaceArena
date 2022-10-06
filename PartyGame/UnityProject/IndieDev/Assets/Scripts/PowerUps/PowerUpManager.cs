using System;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    public PlayerController user;

    public void Start()
    {
        OnUse();
    }

    public virtual void OnUse()
    {
        
    }
    
    public virtual void OnConditionCheck()
    {
        
    }
}
