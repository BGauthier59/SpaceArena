using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    private EventSystem eventSystem;
    [SerializeField] private GameObject firstSelected;
    
    void Start()
    {
        eventSystem = GameManager.instance.eventSystem;
        
        GameManager.instance.EnableMainControllerOnly();

        Time.timeScale = 0f;
        
        eventSystem.SetSelectedGameObject(firstSelected);
    }

    public void OnQuit()
    {
        GameManager.instance.isPaused = false;

        GameManager.instance.EnableAllControllers();
        Time.timeScale = 1f;
        SceneManager.UnloadSceneAsync(GameManager.instance.pauseMenuIndex);
        
    }

    
}
