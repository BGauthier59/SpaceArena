using System;
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
    [SerializeField] private WavesManager wavesManager;
    public RandomEventManager randomEventManager;
    public CameraBehaviour cameraBehaviour;
    public CameraZoom beginningZoom;
    
    [Header("Game Parameters")] 
    [Tooltip("Duration in seconds")] [SerializeField] private float partyDuration;
    private float partyTimer;

    [SerializeField] private float beforeGameStartsDuration;
    private float beforeGameStartsTimer;

    [SerializeField] private float whenGameEndsDuration;
    private float whenGameEndsTimer;

    public GameState gameState;
    public bool gameWon;

    public enum GameState
    {
        Beginning, InGame, End, Finished
    }

    [SerializeField] private bool hasPartyBegun;

    [Header("Interface")]
    [SerializeField] private GameObject endOfParty;
    [SerializeField] private ScoreArea[] scoreAreas;
    [SerializeField] private Button backToMainMenu;
    [SerializeField] private GameObject mainCanvas;
    [SerializeField] private CameraShake cameraShake;

    [Serializable]
    public struct ScoreArea
    {
        public GameObject area;
        public TextMeshProUGUI scoreText;
    }

    private void Awake()
    {
        InitializationAwake();
    }

    private void Start()
    {
        InitializationStart();
    }

    private void Update()
    {
        switch (gameState)
        {
            case GameState.Beginning:
                CheckTimerBeforeGameStarts();
                break;
            case GameState.InGame:
                CheckTimerInGame();
                break;
            case GameState.End:
                CheckTimerWhenGameEnds();
                break;
            case GameState.Finished:
                break;
        }
    }

    private void InitializationAwake()
    {
        gameState = GameState.Beginning;
        hasPartyBegun = false;
        eventSystem = GameManager.instance.eventSystem;
        GameManager.instance.playerInputManager = null;
        GameManager.instance.mainCanvas = mainCanvas;
        GameManager.instance.cameraShake = cameraShake;
        GameManager.instance.partyManager = this;
    }

    private void InitializationStart()
    {
        for (int i = 0; i < GameManager.instance.allPlayers.Count; i++)
        {
            var player = GameManager.instance.allPlayers[i];
            player.initPos = allSpawningPoints[i].position;
            player.transform.position = player.initPos;
            player.rd.material.color = GameManager.instance.colors[player.playerIndex - 1];
            player.PartyBegins();
        }
        
        cameraBehaviour.SetZoom(beginningZoom);
    }
    
    private void StartingGame()
    {
        // Initializing players
        gameState = GameState.InGame;

        for (int i = 0; i < GameManager.instance.allPlayers.Count; i++)
        {
            var player = GameManager.instance.allPlayers[i];
            player.ActivatePlayer();
        }
        
        // Initializing timer
        partyTimer = partyDuration;
        
        // Starts game
        GameManager.instance.EnableAllControllers();
        hasPartyBegun = true;
        wavesManager.StartNewWave();
        cameraBehaviour.ResetZoom();
    }

    #region Before Game Starts

    private void CheckTimerBeforeGameStarts()
    {
        if (beforeGameStartsTimer >= beforeGameStartsDuration)
        {
            // When Game Starts
            StartingGame();
        }
        else beforeGameStartsTimer += Time.deltaTime;
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
        
        timerText.text = ((int) partyTimer).ToString();
    }

    #endregion

    #region When Game Ends

    public void EndingGame(bool won)
    {
        gameWon = won;
        for (int i = 0; i < GameManager.instance.allPlayers.Count; i++)
        {
            var player = GameManager.instance.allPlayers[i];
            player.DeactivatePlayer();
        }
        EnemiesManager.instance.DeactivateAllEnemies();

        wavesManager.enabled = false;
        gameState = GameState.End;
    }

    private void CheckTimerWhenGameEnds()
    {
        if (whenGameEndsTimer >= whenGameEndsDuration)
        {
            // When Game Ends
            FinishingGame();
        }
        else whenGameEndsTimer += Time.deltaTime;
    }

    private void FinishingGame()
    {
        gameState = GameState.Finished;
        DisplayScore();
    }
    
    private void DisplayScore()
    {
        endOfParty.SetActive(true);
        for (int i = 0; i < scoreAreas.Length; i++)
        {
            if (i > GameManager.instance.allPlayers.Count - 1)
            {
                Debug.LogWarning("There's less players in game than it should be.");
                break;
            }
            
            if (GameManager.instance.playersNumber > i)
            {
                scoreAreas[i].area.SetActive(true);
                scoreAreas[i].scoreText.text = $"Pt: {GameManager.instance.allPlayers[i].points}";
            }
            else scoreAreas[i].area.SetActive(false);
        }
        eventSystem.SetSelectedGameObject(backToMainMenu.gameObject);
    }
    
    public void OnFinishGame()
    {
        endOfParty.SetActive(false);
        
        OnQuit();
    }

    #endregion
    
    public void OnQuit()
    {
        GameManager.instance.SetMainGamepad(Gamepad.all[0]);
        GameManager.instance.EnableMainControllerOnly();
        
        for (int i = GameManager.instance.allPlayers.Count - 1; i >= 0; i--)
        {
            var player = GameManager.instance.allPlayers[i];
            Destroy(player.gameObject);
        }

        GameManager.instance.allPlayers.Clear();
        GameManager.instance.playersNumber = 0;
        GameManager.instance.partyManager = null;
        SceneManager.LoadScene(GameManager.instance.mainMenuIndex);
    }
}