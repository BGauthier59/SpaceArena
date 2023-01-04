using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// An interface for every behaviours that interact with players' projectiles
public interface IInteractable
{
    public void OnHitByProjectile(Vector3 forward);

}
