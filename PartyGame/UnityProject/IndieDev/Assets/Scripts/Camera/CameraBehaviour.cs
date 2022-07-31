using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private float lerpSpeed;

    private Vector3 nextPos;

    void FixedUpdate()
    {
        if (GameManager.instance.allPlayers.Count == 0) return;
        
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

        nextPos = new Vector3(posX + offset.x, offset.y, posZ + offset.z);
        
        //transform.position = nextPos;
        //transform.position = Vector3.SmoothDamp(transform.position, nextPos, ref _v, lerpSpeed);
        transform.position = Vector3.Lerp(transform.position, nextPos, lerpSpeed * Time.deltaTime);
        //transform.position = Vector3.Lerp(transform.position, nextPos, Time.deltaTime);
    }
    
}
