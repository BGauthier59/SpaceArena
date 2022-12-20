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
            var alpha = Mathf.Lerp(1, 0, timer / lifeTime) + .1f;
            scoreText.color = new Color(scoreText.color.r, scoreText.color.g, scoreText.color.b, alpha);
        }
    }

    public void SetPosition(Vector3 pos)
    {
        rect.localScale = Vector3.one;
        var finalePos = pos + Vector3.forward;
        rect.position = finalePos;
        rect.localRotation = Quaternion.identity;
        onScreen = true;
    }

    public void SetText(int point, Color color)
    {
        scoreText.color = color;
        scoreText.text = point.ToString();
    }
}