using System;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PartyManager : MonoBehaviour
{
    private EventSystem eventSystem;

    [SerializeField] private Transform[] allSpawningPoints;
    [SerializeField] private Transform[] playerUIPositions;
    public PlayerUI[] PlayerUis;
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Instances")] public BaseManager baseManager;
    public EnemiesManager enemiesManager;
    public NewWavesManager wavesManager;
    public RandomEventManager randomEventManager;
    public CameraManager cameraManager;
    public ArenaFeedbackManager arenaFeedbackManager;
    public Light mainLight;

    [Header("Cinematic camera zooms")] public CameraZoom showScreenZoom;
    public CameraZoom lightDezoom;
    public CameraZoom screenLargeZoom;
    public CameraZoom showArenaZoom;

    [Header("Game Parameters")] [Tooltip("Duration in seconds")] [SerializeField]
    private float partyDuration;

    private float partyTimer;
    [SerializeField] private float durationBeforeCrownAppears;

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
    [SerializeField] private Image guiBackground;
    [SerializeField] private GameObject timerArea;
    [SerializeField] private GameObject endOfParty;
    [SerializeField] private GameObject newEndOfParty;
    [SerializeField] private NewScoreArea[] newScoreAreas;
    [SerializeField] private Button backToMainMenu;
    public CameraShake cameraShake;
    [SerializeField] private GameObject loadingPart;
    [SerializeField] private Animation loadingAnim;
    [SerializeField] private Animation displayNameAnim;
    [SerializeField] private Animation displayMessageAnim;
    [SerializeField] private TextMeshProUGUI arenaNameText;
    [SerializeField] private TextMeshProUGUI arenaCodeText;

    [SerializeField] private TextOnDisplay tutorialText;
    [SerializeField] private TextMeshProUGUI goText;
    private int loadingTextCurrentIndex = -1;
    [SerializeField] private TextMeshProUGUI loadingText;

    [SerializeField] private GameObject randomEventArea;

    [SerializeField] private float rotatingSkyboxSpeed;

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

        public void DisableText()
        {
            tmp.gameObject.SetActive(false);
        }
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
        GameManager.instance.TranslateTexts();

        BeginningGameCinematic();
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

        RenderSettings.skybox.SetFloat("_Rotation", Time.time * rotatingSkyboxSpeed);
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
        arenaFeedbackManager.Initialization();
    }

    #region Before Game Starts

    private async void BeginningGameCinematic()
    {
        loadingPart.SetActive(true);

        for (int i = 0; i < GameManager.instance.allPlayers.Count; i++)
        {
            var player = GameManager.instance.allPlayers[i];
            player.initPos = allSpawningPoints[i].position;
            player.transform.position = player.initPos;
            player.playerUI = PlayerUis[i];
            player.PartyBegins();
        }

        switch (GameManager.instance.settings.currentLanguage)
        {
            case Language.French:
                arenaNameText.text = GameManager.instance.currentPanel
                    .arenas[GameManager.instance.currentPanel.currentIndex]
                    .translatableName.frenchName;
                break;
            case Language.English:
                arenaNameText.text = GameManager.instance.currentPanel
                    .arenas[GameManager.instance.currentPanel.currentIndex]
                    .translatableName.englishName;
                break;
            default:
                Debug.LogError("Language is not valid.");
                break;
        }

        arenaCodeText.text = GameManager.instance.currentPanel.arenas[GameManager.instance.currentPanel.currentIndex]
            .codeName;

        loadingText.gameObject.SetActive(true);
        ChangeLoadingText();
        GameManager.instance.eventSystem.SetSelectedGameObject(loadingText.gameObject);
        GameManager.instance.EnableAllControllers();

        await Task.Delay(5000);

        GameManager.instance.DisableAllControllers();
        loadingText.gameObject.SetActive(false);
        GameManager.instance.eventSystem.SetSelectedGameObject(null);
        loadingAnim.Play(loadingAnim.clip.name);

        await Task.Delay(1000);

        loadingPart.SetActive(false);

        await Task.Delay(250);

        displayNameAnim.gameObject.SetActive(true);
        displayNameAnim.Play(displayNameAnim.clip.name);

        await Task.Delay(750);

        cameraManager.SetZoom(showScreenZoom);

        await Task.Delay(2500);

        displayNameAnim.gameObject.SetActive(false);
        cameraManager.SetZoom(screenLargeZoom);

        await Task.Delay(1000);

        tutorialText.SetText();

        await Task.Delay(1000);

        tutorialText.DisableText();
        cameraManager.SetZoom(showArenaZoom);

        await Task.Delay(2500);

        // Show base elements names

        foreach (var baseElement in baseManager.allBaseElements)
        {
            baseElement.PlayBaseElementNameAnim();
        }

        await Task.Delay(400);
        cameraManager.SetZoom(lightDezoom);
        await Task.Delay(1000);

        goText.gameObject.SetActive(true);
        goText.text = "3";
        await Task.Delay(1000);
        goText.text = "2";
        await Task.Delay(1000);
        goText.text = "1";
        await Task.Delay(1000);
        goText.text = "GO!";

        StartingGame();

        await Task.Delay(1000);
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

        // Feedbacks
        arenaFeedbackManager.OnExcitementGrows?.Invoke(arenaFeedbackManager.highestExcitementScore);
        arenaFeedbackManager.ForceExcitementToValue(5);
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

        timerText.text = Ex.ConvertSecondsInMinutes((int)partyTimer);

        foreach (var pc in GameManager.instance.allPlayers) pc.UpdateCrownTimer();

        randomEventManager.CheckPartyTimer(partyTimer);
        arenaFeedbackManager.CheckTimer(Time.deltaTime);
    }

    

    #endregion

    #region When Game Ends

    public void EndingGame(bool won)
    {
        gameWon = won;
        gameState = GameState.End;
        EndingGameCinematic();
    }

    private async void EndingGameCinematic()
    {
        for (int i = 0; i < GameManager.instance.allPlayers.Count; i++)
        {
            var player = GameManager.instance.allPlayers[i];
            player.SetPlayerWhenPartyEnds();
            DontDestroyOnLoad(player.gameObject);
        }

        enemiesManager.DeactivateAllEnemies();
        wavesManager.enabled = false;
        randomEventManager.CancelRandomEventManager();

        await Task.Delay(500);

        displayMessageAnim.Play(displayMessageAnim.clip.name);
        displayMessageAnim.gameObject.SetActive(true);

        await Task.Delay(500);

        cameraManager.SetZoom(lightDezoom);

        await Task.Delay(1000);

        timerArea.SetActive(false);
        cameraManager.SetZoom(showScreenZoom);

        await Task.Delay(1000);

        cameraManager.SetZoom(screenLargeZoom);

        if (!gameWon)
        {
            Debug.Log("You lost the game, meaning every player's score will be cut by half!");
            foreach (var pc in GameManager.instance.allPlayers)
            {
                pc.manager.score = (int)(pc.manager.score * .5f);
            }
        }

        DisplayScore();

        await Task.Delay(1000); // Pour l'instant, sinon on attend que le winner a eu ses pts

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

        arenaFeedbackManager.OnExcitementGrows?.Invoke(arenaFeedbackManager.highestExcitementScore);
    }

    public void OnFinishGame()
    {
        endOfParty.SetActive(false);
        OnSetNextArena();
    }

    private void OnSetNextArena()
    {
        GameManager.instance.currentPanel.currentIndex++;

        if (GameManager.instance.currentPanel.currentIndex == GameManager.instance.currentPanel.arenas.Length)
        {
            SceneManager.LoadScene(GameManager.instance.finaleSceneIndex);
            //OnQuit();
        }
        else
        {
            StopAllCoroutines();

            SceneManager.LoadScene(GameManager.instance.currentPanel.arenas
                [GameManager.instance.currentPanel.currentIndex].arenaSceneIndex);
        }
    }

    #endregion

    public void OnQuit()
    {
        StopAllCoroutines();
        //GameManager.instance.SetMainGamepad(Gamepad.all[0]);
        GameManager.instance.DisableAllControllers();

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

    public void ChangeLoadingText()
    {
        int randomValue = 0;
        do
        {
            if (GameManager.instance.allLoadingTexts.Length < 2) break;
            
            randomValue = Random.Range(0, GameManager.instance.allLoadingTexts.Length);
        } while (loadingTextCurrentIndex == randomValue);

        loadingTextCurrentIndex = randomValue;
        var randomText = GameManager.instance.allLoadingTexts[randomValue];
        loadingText.text = GameManager.instance.settings.currentLanguage switch
        {
            Language.French => randomText.frenchText,
            Language.English => randomText.englishText,
            _ => "Error!"
        };
    }

    public async void RandomEventSetDisplay(RandomEvent ev)
    {
        SetScreenRandomEvent(true);
        ev.randomEventText.SetText();
        await Task.Delay(2500);
        ev.SetAdditionalInfo();
        await Task.Delay(2500);
        ev.DisableAdditionalInfo();
        ev.randomEventText.DisableText();
        SetScreenRandomEvent(false);
    }

    public void SetScreenRandomEvent(bool active)
    {
        timerArea.SetActive(!active);
        randomEventArea.SetActive(active);
    }

    public void DisplayScoreFeedback(int point, int colorIndex, Vector3 pos)
    {
        var scorePoint = PoolOfObject.Instance.SpawnFromPool(PoolType.ScorePoint, Vector3.zero, Quaternion.identity);
        scorePoint.transform.SetParent(mainCanvas.transform);
        var scorePointBehaviour = scorePoint.GetComponent<ScorePointBehaviour>();
        scorePointBehaviour.SetText(point, GameManager.instance.colors[colorIndex]);
        scorePointBehaviour.SetPosition(pos);
    }

    #endregion

    #region Score Management

    private bool CanDisplayCrown()
    {
        return !(partyTimer >= partyDuration - durationBeforeCrownAppears);
    }

    public void OnScoresChange()
    {
        if (!CanDisplayCrown()) return;

        PlayerController currentWinner = null;
        var bestScore = 0f;
        foreach (var pc in GameManager.instance.allPlayers)
        {
            if (pc.manager.score >= bestScore)
            {
                bestScore = pc.manager.score;
                currentWinner = pc;
            }
        }

        if (currentWinner == null)
        {
            Debug.LogWarning("No winner found.");
            return;
        }

        if (currentWinner.crown.activeSelf) return;

        foreach (var pc in GameManager.instance.allPlayers)
        {
            pc.crown.SetActive(false);
            pc.playerUI.crown.enabled = false;
        }

        currentWinner.crown.SetActive(true);
        currentWinner.playerUI.crown.enabled = true;
        var color = GameManager.instance.colors[currentWinner.playerIndex - 1];
        color.a = guiBackground.color.a;
        color.r = Mathf.Max(color.r - .5f, 0);
        color.g = Mathf.Max(color.g - .5f, 0);
        color.b = Mathf.Max(color.b - .5f, 0);
        guiBackground.color = color;
        arenaFeedbackManager.OnExcitementGrows?.Invoke(arenaFeedbackManager.highestExcitementScore);
    }

    #endregion

    private void OnDisable()
    {
        RenderSettings.skybox.SetFloat("_Rotation", 0);
    }

    [Serializable]
    public class PlayerUI
    {
        public GameObject self;
        public Slider lifeSlider;
        public Slider reloadSlider;
        public Image powerUpSlider;
        public Image powerUpImage;
        public ParticleSystem powerUpFire, powerUpSparks, powerUpBurst, usingPowerUpFire;
        public Image reloadSliderColor, lifeSliderColor, inputImage;
        public Image crown;
        public Animator powerUpUI;
        public TextMeshProUGUI deathTimerUI;
    }
}