using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class FirstScreenManager : MonoBehaviour
{
    public int mainMenuIndex;
    
    public void OnStartGame()
    {
        GameManager.instance.SetMainGamepad(Gamepad.current);
        GameManager.instance.EnableMainControllerOnly();
        StartCoroutine(LoadMainMenu());
    }

    IEnumerator LoadMainMenu()
    {
        // Initial Feedback

        var g = GameManager.instance.mainGamepad;
        var low = GameManager.instance.feedbacks.mainVibration.low;
        var high = GameManager.instance.feedbacks.mainVibration.high;
        var duration = GameManager.instance.feedbacks.mainVibration.rumbleDuration;
        
        g.SetMotorSpeeds(low, high);
        yield return new WaitForSeconds(duration);
        g.SetMotorSpeeds(0, 0);
        
        // Load scene
        
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(mainMenuIndex);
    }

    private void OnDisable()
    {
        if (GameManager.instance.mainGamepad == null) return;
        GameManager.instance.mainGamepad.SetMotorSpeeds(0, 0);
    }
}