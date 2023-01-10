using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    private EventSystem eventSystem;
    [SerializeField] private GameObject firstSelected;

    private PartyManager partyManager;

    void Start()
    {
        GameManager.instance.TranslateTexts();

        eventSystem = GameManager.instance.eventSystem;
        partyManager = GameManager.instance.partyManager;

        GameManager.instance.EnableMainControllerOnly();

        Time.timeScale = 0f;

        eventSystem.SetSelectedGameObject(firstSelected);
    }

    public void OnOptions()
    {
        SceneManager.LoadSceneAsync(GameManager.instance.optionMenuIndex, LoadSceneMode.Additive);
    }

    public void OnReturn()
    {
        LeavePause();
        GameManager.instance.EnableAllControllers();
        SceneManager.UnloadSceneAsync(GameManager.instance.pauseMenuIndex);
    }

    public void OnQuit()
    {
        if (partyManager == null)
        {
            Debug.LogError("Party manager is null!");
            return;
        }
        
        LeavePause();
        SceneManager.UnloadSceneAsync(GameManager.instance.pauseMenuIndex);
        partyManager.OnQuit();
    }

    private void LeavePause()
    {
        GameManager.instance.isPaused = false;
        Time.timeScale = 1f;
    }
}