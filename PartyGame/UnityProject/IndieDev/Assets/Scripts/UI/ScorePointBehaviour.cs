using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using TMPro;
using UnityEngine;

public class ScorePointBehaviour : MonoBehaviour
{
    public RectTransform rect;
    public TextMeshProUGUI scoreText;
    [SerializeField] private float lifeTime;
    private float timer;
    private bool onScreen = false;
    [SerializeField] private AnimationCurve speedOverLifetime;

    private void Update()
    {
        if (!onScreen) return;
        
        if (timer >= lifeTime)
        {
            timer = 0f;
            onScreen = false;
            gameObject.SetActive(false);
        }
        else
        {
            timer += Time.deltaTime;
            rect.position += Vector3.forward * (speedOverLifetime.Evaluate(timer) * Time.deltaTime);
        }
    }

    public void SetPosition(Transform tr)
    {
        rect.localScale = Vector3.one;
        var pos = tr.position + Vector3.forward;
        rect.position = pos;
        rect.localRotation = Quaternion.identity;
        onScreen = true;
    }

    public void SetText(int point, Color color)
    {
        scoreText.color = color;
        scoreText.text = point.ToString();
    }
}