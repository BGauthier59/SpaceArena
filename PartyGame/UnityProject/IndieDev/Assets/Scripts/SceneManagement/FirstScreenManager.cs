using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class FirstScreenManager : MonoBehaviour
{
    private EventSystem eventSystem;

    [SerializeField] private GameObject firstSelected;
    public int mainMenuIndex;

    private void Start()
    {
        Initialization();
    }

    private void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * 4f);
    }

    private void Initialization()
    {
        eventSystem = GameManager.instance.eventSystem;
        GameManager.instance.TranslateTexts();

        eventSystem.SetSelectedGameObject(firstSelected);
    }
    
    public void OnStartGame()
    {
        GameManager.instance.SetMainGamepad(Gamepad.current);
        GameManager.instance.EnableMainControllerOnly();
        StartCoroutine(LoadMainMenu());
    }

    private IEnumerator LoadMainMenu()
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
        RenderSettings.skybox.SetFloat("_Rotation", 0);
    }
}