using System;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Camera mainCamera;
    [SerializeField] private CameraZoom defaultZoom;
    [SerializeField] private CameraZoom currentZoom;

    private float zoomTimer;
    private Vector3 nextPos;

    [SerializeField] private float securityOffsetMinDistanceBetweenPlayers;
    private float _securityDistance;
    [SerializeField] private float securityYOffsetPerMeterAboveMinDistance;
    [SerializeField] private float securityZOffsetPerMeterAboveMinDistance;
    private Vector3 _securityOffset;

    private void Start()
    {
        _securityDistance = Mathf.Pow(securityOffsetMinDistanceBetweenPlayers, 2);
    }

    private void FixedUpdate()
    {
        if (GameManager.instance.allPlayers.Count == 0) return;
        if (!currentZoom) return;
        SetCamera();
    }

    public void SetZoom(CameraZoom zoom) => currentZoom = zoom;

    public void ResetZoom() => currentZoom = defaultZoom;

    private void SetCamera()
    {
        var maxDistance = 0f;
        foreach (var p1 in GameManager.instance.allPlayers)
        {
            foreach (var p2 in GameManager.instance.allPlayers)
            {
                if (p1 == p2) continue;
                var distance = Vector3.SqrMagnitude(p1.transform.position - p2.transform.position);
                if (distance > maxDistance) maxDistance = distance;
            }
        }

        if (maxDistance > _securityDistance)
        {
            var dist = Mathf.Sqrt(maxDistance);
            _securityOffset = Vector3.up * ((dist - securityOffsetMinDistanceBetweenPlayers) *
                                            securityYOffsetPerMeterAboveMinDistance)
                              + Vector3.back * ((dist - securityOffsetMinDistanceBetweenPlayers) *
                                                securityZOffsetPerMeterAboveMinDistance);
        }
        else _securityOffset = Vector3.zero;

        // Set position
        switch (currentZoom.target)
        {
            case PossibleTarget.Players:
                float posX = 0f;
                float posZ = 0f;

                foreach (var player in GameManager.instance.allPlayers)
                {
                    posX += player.transform.position.x;
                    posZ += player.transform.position.z;
                }

                var players = GameManager.instance.allPlayers.Count;
                posX /= players;
                posZ /= players;

                nextPos = new Vector3(posX, 0, posZ);
                break;

            case PossibleTarget.Center:
                nextPos = Vector3.zero;
                break;

            default:
                nextPos = Vector3.zero;
                Debug.LogError("Target is not valid.");
                break;
        }

        // Set offset
        nextPos += currentZoom.offset + _securityOffset;
        if (transform.position != nextPos)
        {
            transform.position =
                Vector3.Lerp(transform.position, nextPos, Time.fixedDeltaTime * currentZoom.offsetSpeed);
        }

        // Set rotation
        if (transform.eulerAngles != currentZoom.eulerAngles)
        {
            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, currentZoom.eulerAngles,
                Time.fixedDeltaTime * currentZoom.eulerAnglesSpeed);
        }

        // Set field of view
        if (Math.Abs(mainCamera.fieldOfView - currentZoom.fieldOfView) > .01f)
        {
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, currentZoom.fieldOfView,
                Time.fixedDeltaTime * currentZoom.fovSpeed);
        }

        // Set duration
        if (!currentZoom.hasDuration) return;

        if (zoomTimer >= currentZoom.zoomDuration)
        {
            zoomTimer = 0f;
            SetZoom(defaultZoom);
        }
        else zoomTimer += Time.deltaTime;
    }
}