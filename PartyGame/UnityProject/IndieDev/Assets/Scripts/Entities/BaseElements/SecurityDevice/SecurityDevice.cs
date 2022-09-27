using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityDevice : BaseElementManager
{
    public AssociatedDoor[] associatedDoors;

    private bool isMovingDoors;

    private List<Transform> playersInArea = new List<Transform>();
    private bool isInArea;

    [Serializable]
    public class AssociatedDoor
    {
        public Transform tr;
        public float yPosOpened;
        public float yPosClosed;

        public float movingDuration;
        private float movingTimer;

        public bool isMoved;

        private Vector3 nextPosY;

        public void SetDoor(bool closing)
        {
            var pos = tr.position;
            var yPos = closing ? yPosClosed : yPosOpened;
            nextPosY = new Vector3(pos.x, yPos, pos.z);
        }

        public void MoveDoor()
        {
            tr.position = Vector3.Lerp(tr.position, nextPosY, movingTimer / movingDuration);

            if (movingTimer >= movingDuration)
            {
                isMoved = true;
                movingTimer = 0f;
            }
            else movingTimer += Time.deltaTime;
            
        }
    }

    public override void TakeDamage(int damage, Entity attacker = null)
    {
        Debug.Log("Security Device is hurt");
        base.TakeDamage(damage, attacker);
    }

    public override void OnDestroyed()
    {
        base.OnDestroyed();

        Debug.Log("Security Device is dead");
        
        foreach (var door in associatedDoors)
        {
            door.SetDoor(true);
        }

        isMovingDoors = true;
    }

    public override void OnFixed()
    {
        base.OnFixed();

        foreach (var door in associatedDoors)
        {
            door.SetDoor(false);
        }

        isMovingDoors = true;
    }
    
    public override void Update()
    {
        base.Update();
        
        MovingDoors();
    }

    private void MovingDoors()
    {
        if (!isMovingDoors) return;
        foreach (var door in associatedDoors)
        {
            door.MoveDoor();
        }

        foreach (var door in associatedDoors)
        {
            if (!door.isMoved) return;
        }
            
        Debug.Log("All doors have moved!");

        foreach (var door in associatedDoors)
        {
            door.isMoved = false;
        }

        isMovingDoors = false;
    }
}