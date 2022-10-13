using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CameraRotationEvent : RandomEvent
{
    private Transform _camera;
    [SerializeField] private float rotationSpeed;

    private List<(RotationDirection, Quaternion)> _rotationDirections;
    private (RotationDirection, Quaternion) _currentDir;
    private Quaternion _quaternionDir;

    private enum RotationDirection
    {
        Left,
        Right,
        Bottom,
        Up
    }

    public override void StartEvent()
    {
        // Lance FX de fumée
        Debug.Log("Rotation Event starts!");

        _camera = GameManager.instance.partyManager.cameraManager.transform;
        GameManager.instance.partyManager.cameraManager.enabled = false;

        _rotationDirections = new List<(RotationDirection, Quaternion)>
        {
            (RotationDirection.Left, Quaternion.Euler(Vector3.left)),
            (RotationDirection.Right, Quaternion.Euler(Vector3.right)),
            (RotationDirection.Bottom, Quaternion.Euler(Vector3.down)),
            (RotationDirection.Up, Quaternion.Euler(Vector3.up)),
        };

        isRunning = true;
        
        SelectCurrentRotationDirection();
    }

    private void SelectCurrentRotationDirection()
    {
        _currentDir = _rotationDirections[Random.Range(0, _rotationDirections.Count)];
        _rotationDirections.Remove(_currentDir);

        _quaternionDir = _currentDir.Item2;
    }

    private void LateUpdate()
    {
        if (!isRunning) return;

        /*
        if (eventTimer >= eventDuration / 4)
        {
            eventTimer = 0f;
            SelectCurrentRotationDirection();
        }
        else
        {
            _camera.rotation = Quaternion.Lerp(_camera.rotation, _quaternionDir, Time.deltaTime * rotationSpeed);
            eventTimer += Time.deltaTime;
        }
        */

        //_camera.eulerAngles += Vector3.forward * (rotationSpeed * Time.deltaTime);

        //_camera.Rotate(Vector3.forward * (rotationSpeed * Time.deltaTime));
    }

    public override void EndEvent()
    {
        Debug.Log("Rotation Event ends!");

        // Enlève fumée
        isRunning = false;
        GameManager.instance.partyManager.cameraManager.enabled = true;
        GameManager.instance.partyManager.cameraManager.ResetZoom();
    }
}