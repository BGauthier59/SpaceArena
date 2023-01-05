using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Connection Canvas")] private EventSystem eventSystem;
    [SerializeField] private GameObject firstSelected;

    [SerializeField] private PlayerInputManager playerInputManager;
    [SerializeField] private GameObject startButton;
    [SerializeField] private GameObject playerNumberSelection;
    [SerializeField] private Slider playerSlider;
    [SerializeField] private GameObject lobby;
    [SerializeField] private LobbyArea[] playerLobbyAreas;
    private bool canCheckPlayer;
    private bool arePlayersReady;
    [SerializeField] private Button launchParty;
    [SerializeField] private Button returnFromLobby;
    [SerializeField] private Transform[] playerInstancesPos;

    [Serializable]
    public struct LobbyArea
    {
        public GameObject area;
        public TextMeshProUGUI textArea;
        public Image[] coloredImages;
    }

    [Space(5)] [Header("Selection Canvas")] [SerializeField]
    private Canvas selectionCanvas;

    [SerializeField] private GameObject firstSelectedSelection;

    [SerializeField] private RectTransform arenaInfo;
    private Vector3 arenaInfoInitPos;
    private Vector3 arenaInfoInitPosLocked;
    [SerializeField] private Image arenaImageSelection;
    [SerializeField] private TextMeshProUGUI arenaTextSelection;

    [SerializeField] private Button cancelButtonSelection;
    private bool isArenaInfoMoving;
    [SerializeField] private float arenaMovingHalfDuration;
    private float arenaMovingTimer;
    public float arenaInfoPosXToReach;
    public const float SecurityGap = 100f;
    private bool isGoingOtherSize;
    private int arenaIndex;

    private ArenasPanel currentSelectedArenasPanel;
    [SerializeField] private ArenasPanel[] allArenasPanels;

    [SerializeField] private Animation loadingAnim;

    [Serializable]
    public struct ArenasPanel
    {
        public ArenaScriptable[] arenas;
        public TranslatableName panelName;
        public Sprite panelImage;
        [HideInInspector] public int currentIndex;
    }

    [Serializable]
    public struct TranslatableName
    {
        public string frenchName;
        public string englishName;
    }

    #region Initialization

    private void Start()
    {
        Initialization();
    }

    private void Initialization()
    {
        GameManager.instance.EnableMainControllerOnly();

        eventSystem = GameManager.instance.eventSystem;
        eventSystem.SetSelectedGameObject(firstSelected);

        arePlayersReady = false;
        canCheckPlayer = false;

        arenaInfoInitPos = arenaInfo.anchoredPosition;
        arenaInfoInitPosLocked = arenaInfoInitPos;

        GameManager.instance.playerInputManager = playerInputManager;
        GameManager.instance.playerInputManager.DisableJoining();
    }

    #endregion

    #region Main State

    public void OnStart()
    {
        playerNumberSelection.SetActive(true);
        playerSlider.value = playerSlider.minValue;
        eventSystem.SetSelectedGameObject(playerSlider.gameObject);
    } // Button Start on main state

    public void OnOptions()
    {
        SceneManager.LoadSceneAsync(GameManager.instance.optionMenuIndex, LoadSceneMode.Additive);
    } // Button Options on main state

    #endregion

    #region Player Number Selection

    public void OnCancel()
    {
        eventSystem.SetSelectedGameObject(startButton);
        playerNumberSelection.SetActive(false);
    } // Button Return on player number selection

    public void OnPlay()
    {
        GameManager.instance.playersNumber = (int) playerSlider.value;
        lobby.SetActive(true);
        for (int i = 0; i < playerLobbyAreas.Length; i++)
        {
            if (GameManager.instance.playersNumber > i)
            {
                playerLobbyAreas[i].area.SetActive(true);

                /*
                foreach (var image in playerLobbyAreas[i].coloredImages)
                {
                    image.color = GameManager.instance.colors[i];
                }
                */

                string message;

                switch (GameManager.instance.settings.currentLanguage)
                {
                    case Language.French:
                        message = "En attente...";
                        break;

                    case Language.English:
                        message = "Waiting...";
                        break;

                    default:
                        Debug.LogError("No language available!");
                        message = null;
                        break;
                }

                playerLobbyAreas[i].textArea.text = message;
            }
            else playerLobbyAreas[i].area.SetActive(false);
        }

        if (GameManager.instance.settings.multiplayerVersion)
        {
            launchParty.interactable = false;
        }
        else
        {
            Debug.LogWarning("Launch Party button shouldn't be interactable!");
            var launchPartyNavigation = new Navigation
            {
                mode = Navigation.Mode.Explicit,
                selectOnRight = returnFromLobby,
                selectOnLeft = returnFromLobby
            };
            launchParty.navigation = launchPartyNavigation;

            var returnFromLobbyNavigation = new Navigation
            {
                mode = Navigation.Mode.Explicit,
                selectOnRight = launchParty,
                selectOnLeft = launchParty
            };
            returnFromLobby.navigation = returnFromLobbyNavigation;
        }

        GameManager.instance.EnableAllControllers();
        GameManager.instance.playerInputManager.EnableJoining();

        eventSystem.SetSelectedGameObject(returnFromLobby.gameObject);

        canCheckPlayer = true;
    } // Button Go on player number selection

    #endregion

    #region Lobby

    private void Update()
    {
        if (isArenaInfoMoving) MovingArenaInfo();

        if (!canCheckPlayer) return;
        CheckPlayers();

        if (arePlayersReady) FinaleSetup();
    }

    private void CheckPlayers()
    {
        if (GameManager.instance.playerInputManager.playerCount != GameManager.instance.playersNumber) return;
        arePlayersReady = true;
    } // Check if every player is connected

    private void FinaleSetup()
    {
        arePlayersReady = false;
        canCheckPlayer = false;
        GameManager.instance.playerInputManager.DisableJoining();
        GameManager.instance.EnableMainControllerOnly();

        eventSystem.SetSelectedGameObject(launchParty.gameObject);

        var launchPartyNavigation = new Navigation
        {
            mode = Navigation.Mode.Explicit,
            selectOnRight = returnFromLobby,
            selectOnLeft = returnFromLobby
        };
        launchParty.navigation = launchPartyNavigation;

        var returnFromLobbyNavigation = new Navigation
        {
            mode = Navigation.Mode.Explicit,
            selectOnRight = launchParty,
            selectOnLeft = launchParty
        };
        returnFromLobby.navigation = returnFromLobbyNavigation;

        launchParty.interactable = true;
    } // When every player is connected

    public void OnCancelParty()
    {
        if (Gamepad.current != GameManager.instance.mainGamepad)
        {
            Debug.LogWarning("This is not the main gamepad");
            return;
        }

        lobby.SetActive(false);
        playerNumberSelection.SetActive(false);
        GameManager.instance.EnableMainControllerOnly();
        GameManager.instance.playerInputManager.DisableJoining();

        for (int i = GameManager.instance.allPlayers.Count; i > 0; i--)
        {
            var player = GameManager.instance.allPlayers[i - 1].gameObject;
            Destroy(player);
        }

        GameManager.instance.allPlayers.Clear();

        if (GameManager.instance.settings.multiplayerVersion)
        {
            launchParty.interactable = false;

            var launchPartyNavigation = new Navigation
            {
                mode = Navigation.Mode.None
            };
            launchParty.navigation = launchPartyNavigation;

            var returnFromLobbyNavigation = new Navigation
            {
                mode = Navigation.Mode.None
            };
            returnFromLobby.navigation = returnFromLobbyNavigation;
        }
        else
        {
            Debug.LogWarning("Launch Party button shouldn't be interactable!");
        }

        eventSystem.SetSelectedGameObject(startButton.gameObject);
    } // Button Cancel on lobby

    public void OnPlayerJoin(PlayerInput input)
    {
        var gamepad = input.GetDevice<Gamepad>();

        var playerIndex = GameManager.instance.playerInputManager.playerCount;
        input.transform.position = playerInstancesPos[playerIndex - 1].position;
        input.transform.rotation = Quaternion.Euler(Vector3.up * 180);
        string message;

        switch (GameManager.instance.settings.currentLanguage)
        {
            case Language.French:
                message = "Connecté !";
                break;

            case Language.English:
                message = "Joined!";
                break;

            default:
                Debug.LogError("No language available!");
                message = null;
                break;
        }

        if (gamepad == GameManager.instance.mainGamepad) message += " [Main]";

        playerLobbyAreas[playerIndex - 1].textArea.text = message;
    } // When a player joins

    public void OnBeginParty()
    {
        foreach (var pc in GameManager.instance.allPlayers)
        {
            pc.rd.gameObject.SetActive(false);
        }

        if (!GameManager.instance.settings.multiplayerVersion) GameManager.instance.playerInputManager.DisableJoining();

        OpenSelectionCanvas();
    } // Button Play on lobby

    #endregion

    #region Arena Selection

    private void OpenSelectionCanvas()
    {
        selectionCanvas.gameObject.SetActive(true);
        SelectNewArena(0);
        eventSystem.SetSelectedGameObject(firstSelectedSelection);
    }

    public void OnCancelSelection()
    {
        selectionCanvas.gameObject.SetActive(false);
        OnCancelParty();
    }

    public void MoveArena()
    {
        var joystick = Gamepad.current.leftStick.ReadValue();
        if (joystick.x > .5f) SelectOnLeft();
        else if (joystick.x < -.5f) SelectOnRight();
        else return;
        eventSystem.SetSelectedGameObject(null);
    }

    private void SelectOnRight()
    {
        if (isArenaInfoMoving) return;
        //GameManager.instance.mainGamepad.

        cancelButtonSelection.interactable = false;
        arenaInfoPosXToReach = arenaInfo.anchoredPosition.x + (Screen.width * .5f) + (arenaInfo.sizeDelta.x / 2) +
                               SecurityGap;
        isArenaInfoMoving = true;
        isGoingOtherSize = false;
        arenaIndex = Array.IndexOf(allArenasPanels, currentSelectedArenasPanel);
        arenaIndex--;

        if (arenaIndex == -1) arenaIndex = allArenasPanels.Length - 1;
    }

    private void SelectOnLeft()
    {
        if (isArenaInfoMoving) return;

        cancelButtonSelection.interactable = false;
        arenaInfoPosXToReach = arenaInfo.anchoredPosition.x -
                               ((Screen.width * .5f) + (arenaInfo.sizeDelta.x / 2) + SecurityGap);
        isArenaInfoMoving = true;
        isGoingOtherSize = false;
        arenaIndex = Array.IndexOf(allArenasPanels, currentSelectedArenasPanel);
        arenaIndex++;

        if (arenaIndex == allArenasPanels.Length) arenaIndex = 0;
    }

    private void MovingArenaInfo()
    {
        if (arenaMovingTimer > arenaMovingHalfDuration)
        {
            if (isGoingOtherSize)
            {
                //arenaInfo.anchoredPosition = arenaInfoInitPos;
                arenaInfo.anchoredPosition = arenaInfoInitPosLocked;
                arenaInfoInitPos = arenaInfo.anchoredPosition;

                arenaMovingTimer = 0f;

                cancelButtonSelection.interactable = true;
                eventSystem.SetSelectedGameObject(firstSelectedSelection);

                isArenaInfoMoving = false;
                return;
            }

            arenaInfo.anchoredPosition = arenaInfo.anchoredPosition.x < 0
                ? new Vector2(Screen.width * .5f + arenaInfo.sizeDelta.x * .5f + SecurityGap,
                    arenaInfo.anchoredPosition.y)
                : new Vector2(-(Screen.width * .5f + arenaInfo.sizeDelta.x * .5f + SecurityGap),
                    arenaInfo.anchoredPosition.y);

            arenaInfoPosXToReach = arenaInfoInitPosLocked.x;
            arenaInfoInitPos = arenaInfo.anchoredPosition;
            arenaMovingTimer = 0f;
            SelectNewArena(arenaIndex);
            isGoingOtherSize = true;
        }
        else
        {
            arenaInfo.anchoredPosition = Vector2.Lerp(arenaInfoInitPos,
                new Vector2(arenaInfoPosXToReach, arenaInfo.anchoredPosition.y),
                arenaMovingTimer / arenaMovingHalfDuration);

            arenaMovingTimer += Time.deltaTime;
        }
    }

    private void SelectNewArena(int index)
    {
        currentSelectedArenasPanel = allArenasPanels[index];

        arenaImageSelection.sprite = currentSelectedArenasPanel.panelImage;

        switch (GameManager.instance.settings.currentLanguage)
        {
            case Language.French:
                arenaTextSelection.text = currentSelectedArenasPanel.panelName.frenchName;
                break;
            case Language.English:
                arenaTextSelection.text = currentSelectedArenasPanel.panelName.englishName;
                break;
        }
    }

    public void OnPlaySelectedMap()
    {
        currentSelectedArenasPanel.currentIndex = 0;
        GameManager.instance.currentPanel = currentSelectedArenasPanel;

        GameManager.instance.DisableAllControllers();

        StartCoroutine(LoadingSelectedMap());
    }

    private IEnumerator LoadingSelectedMap()
    {
        loadingAnim.Play(loadingAnim.clip.name);

        yield return new WaitForSeconds(.5f);

        SceneManager.LoadScene(GameManager.instance.currentPanel.arenas
            [GameManager.instance.currentPanel.currentIndex].arenaSceneIndex);
    }

    #endregion
}