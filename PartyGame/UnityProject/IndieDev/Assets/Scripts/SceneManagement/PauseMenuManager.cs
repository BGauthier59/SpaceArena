using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    private EventSystem eventSystem;
    [SerializeField] private GameObject firstSelected;
    
    void Start()
    {
        GameManager.instance.isPaused = true;
        eventSystem = EventSystem.current;
        eventSystem.SetSelectedGameObject(firstSelected);
    }

    public void OnQuit()
    {
        SceneManager.UnloadSceneAsync(GameManager.instance.pauseMenuIndex);
        GameManager.instance.SetTimeScale();
        GameManager.instance.isPaused = false;
    }

    
}
