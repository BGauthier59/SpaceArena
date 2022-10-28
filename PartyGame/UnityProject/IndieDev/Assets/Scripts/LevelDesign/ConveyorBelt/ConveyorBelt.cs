using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    [SerializeField] private Direction direction;
    private Vector3 directionVector;
    [SerializeField] private float conveyorStrength;
    private List<Transform> trs = new List<Transform>();
    
    private enum Direction
    {
        Left, Right, Up, Down
    }

    private void Start()
    {
        directionVector = direction switch
        {
            Direction.Left => Vector3.left,
            Direction.Right => Vector3.right,
            Direction.Up => Vector3.forward,
            Direction.Down => Vector3.back,
            _ => directionVector
        };
    }

    private void Update()
    {
        foreach (var tr in trs)
        {
            tr.position += directionVector * (conveyorStrength * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            trs.Add(other.transform);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            trs.Remove(other.transform);
        }
    }
}
