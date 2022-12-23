using TMPro;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI fpsCounter;
    private int count;
    private float timer;

    private void Update()
    {
        if (timer >= 1.0f)
        {
            timer -= 1.0f;
            count = 0;
        }
        else
        {
            timer += Time.deltaTime;
            count++;
            var fps = count / timer;
            fpsCounter.text = $"FPS : {fps:F0}";
        }
    }
}