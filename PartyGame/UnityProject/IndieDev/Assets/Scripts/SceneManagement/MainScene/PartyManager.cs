using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PartyManager : MonoBehaviour
{
    private EventSystem eventSystem;

    [SerializeField] private Transform[] allSpawningPoints;
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Instances")] public BaseManager baseManager;
    public EnemiesManager enemiesManager;
    public WavesManager wavesManager;
    public RandomEventManager randomEventManager;
    public CameraManager cameraManager;
    public Light mainLight;

    [Header("Cinematic camera zooms")] public CameraZoom showScreenZoom;
    public CameraZoom lightDezoom;
    public CameraZoom screenLargeZoom;

    [Header("Game Parameters")] [Tooltip("Duration in seconds")] [SerializeField]
    private float partyDuration;

    private float partyTimer;

    public GameState gameState;
    public bool gameWon;

    public enum GameState
    {
        Beginning,
        InGame,
        End
    }

    [SerializeField] private bool hasPartyBegun;

    [Header("Interface")] public GameObject mainCanvas;
    [SerializeField] private GameObject timerArea;
    [SerializeField] private GameObject endOfParty;
    [SerializeField] private GameObject newEndOfParty;
    [SerializeField] private NewScoreArea[] newScoreAreas;
    [SerializeField] private Button backToMainMenu;
    public CameraShake cameraShake;

    [SerializeField] private TextOnDisplay tutorialText;
    [SerializeField] private TextMeshProUGUI goText;

    [SerializeField] private GameObject randomEventArea;
    
    [Serializable]
    public struct TextOnDisplay
    {
        public TextMeshPro tmp;
        public string frenchText;
        public string englishText;

        public void SetText()
        {
            switch (GameManager.instance.settings.currentLanguage)
            {
                case Language.French:
                    tmp.text = frenchText;
                    break;

                case Language.English:
                    tmp.text = englishText;
                    break;

                default:
                    Debug.LogError("Language is not valid!");
                    break;
            }

            tmp.gameObject.SetActive(true);
        }

        public void DisableText() => tmp.gameObject.SetActive(false);
    }

    [Serializable]
    public struct NewScoreArea
    {
        public GameObject area;
        public TextMeshPro scoreText;
    }

    private void Awake()
    {
        InitializationAwake();
    }

    private void Start()
    {
        StartCoroutine(BeginningGameCinematic());
    }

    private void Update()
    {
        switch (gameState)
        {
            case GameState.Beginning:
                break;
            case GameState.InGame:
                CheckTimerInGame();
                break;
            case GameState.End:
                break;
        }
    }

    private void InitializationAwake()
    {
        SetInstances();
        gameState = GameState.Beginning;
        hasPartyBegun = false;
    }

    private void SetInstances()
    {
        eventSystem = GameManager.instance.eventSystem;
        GameManager.instance.playerInputManager = null;
        GameManager.instance.partyManager = this;
        randomEventManager.Initialization();
        wavesManager.Initialization();
    }

    #region Before Game Starts

    private IEnumerator BeginningGameCinematic()
    {
        for (int i = 0; i < GameManager.instance.allPlayers.Count; i++)
        {
            var player = GameManager.instance.allPlayers[i];
            player.initPos = allSpawningPoints[i].position;
            player.transform.position = player.initPos;
            player.rd.material.color = GameManager.instance.colors[player.playerIndex - 1];
            player.PartyBegins();
        }

        yield return new WaitForSeconds(1f);

        cameraManager.SetZoom(showScreenZoom);

        yield return new WaitForSeconds(2f);

        cameraManager.SetZoom(screenLargeZoom);

        yield return new WaitForSeconds(1f);

        tutorialText.SetText();

        yield return new WaitForSeconds(2f);

        tutorialText.DisableText();
        cameraManager.SetZoom(lightDezoom);

        yield return new WaitForSeconds(3f);

        goText.gameObject.SetActive(true);
        goText.text = "3";
        yield return new WaitForSeconds(1f);
        goText.text = "2";
        yield return new WaitForSeconds(1f);
        goText.text = "1";
        yield return new WaitForSeconds(1f);
        goText.text = "GO!";

        StartingGame();

        yield return new WaitForSeconds(1f);
        goText.gameObject.SetActive(false);
    }

    private void StartingGame()
    {
        // Initializing players
        gameState = GameState.InGame;

        for (int i = 0; i < GameManager.instance.allPlayers.Count; i++)
        {
            var player = GameManager.instance.allPlayers[i];
            player.ActivatePlayer();
            player.SetGaugesState(true);
        }

        // Initializing timer
        partyTimer = partyDuration;
        timerArea.SetActive(true);

        // Starts game
        GameManager.instance.EnableAllControllers();
        hasPartyBegun = true;
        wavesManager.StartNewWave();
        randomEventManager.StartRandomEventManager();
        cameraManager.ResetZoom();
    }

    #endregion

    #region In Game

    private void CheckTimerInGame()
    {
        if (!hasPartyBegun) return;

        if (partyTimer <= 0)
        {
            partyTimer = 0f;
            EndingGame(true);
        }
        else partyTimer -= Time.deltaTime;

        timerText.text = ((int)partyTimer).ToString();
    }

    #endregion

    #region When Game Ends

    public void EndingGame(bool won)
    {
        gameWon = won;
        gameState = GameState.End;
        StartCoroutine(EndingGameCinematic());
    }

    private IEnumerator EndingGameCinematic()
    {
        for (int i = 0; i < GameManager.instance.allPlayers.Count; i++)
        {
            var player = GameManager.instance.allPlayers[i];
            player.DeactivatePlayer();
            player.SetGaugesState(false);
        }

        enemiesManager.DeactivateAllEnemies();
        wavesManager.enabled = false;

        yield return new WaitForSeconds(1f);

        cameraManager.SetZoom(lightDezoom);

        yield return new WaitForSeconds(1f);

        timerArea.SetActive(false);
        cameraManager.SetZoom(showScreenZoom);

        yield return new WaitForSeconds(1f);

        cameraManager.SetZoom(screenLargeZoom);
        DisplayScore();

        yield return new WaitForSeconds(1f); // Pour l'instant, sinon on attent que le winner a eu ses pts

        endOfParty.SetActive(true);
        eventSystem.SetSelectedGameObject(backToMainMenu.gameObject);
    }

    private void DisplayScore()
    {
        newEndOfParty.SetActive(true);
        for (int i = 0; i < newScoreAreas.Length; i++)
        {
            if (GameManager.instance.allPlayers.Count <= 1)
            {
                Debug.LogWarning("There's less players in game than it should be.");
                break;
            }

            if (GameManager.instance.playersNumber > i)
            {
                newScoreAreas[i].area.SetActive(true);
                newScoreAreas[i].scoreText.text = $"Pt: {GameManager.instance.allPlayers[i].manager.ReturnPoint()}";
            }
            else newScoreAreas[i].area.SetActive(false);
        }
    }

    public void OnFinishGame()
    {
        endOfParty.SetActive(false);
        OnQuit();
    }

    #endregion

    public void OnQuit()
    {
        StopAllCoroutines();
        GameManager.instance.SetMainGamepad(Gamepad.all[0]);
        GameManager.instance.EnableMainControllerOnly();

        for (int i = GameManager.instance.allPlayers.Count - 1; i >= 0; i--)
        {
            var player = GameManager.instance.allPlayers[i];
            Destroy(player.gameObject);
        }

        GameManager.instance.allPlayers.Clear();
        GameManager.instance.playersNumber = 0;
        //GameManager.instance.partyManager = null;
        SceneManager.LoadScene(GameManager.instance.mainMenuIndex);
    }

    #region Display

    public IEnumerator RandomEventSetDisplay(RandomEvent ev)
    {
        timerArea.SetActive(false);
        randomEventArea.SetActive(true);
        ev.randomEventText.SetText();
        yield return new WaitForSeconds(5f);
        ev.randomEventText.DisableText();
        randomEventArea.SetActive(false);
        timerArea.SetActive(true);
    }

    #endregion
}