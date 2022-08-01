using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    private EventSystem eventSystem;
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

    [Serializable]
    public struct LobbyArea
    {
        public GameObject area;
        public TextMeshProUGUI textArea;
        public Image[] coloredImages;
    }

    private void Start()
    {
        Initialization();
    }

    private void Initialization()
    {
        eventSystem = GameManager.instance.eventSystem;
        eventSystem.SetSelectedGameObject(firstSelected);

        arePlayersReady = false;
        canCheckPlayer = false;
        GameManager.instance.playerInputManager = playerInputManager;
        GameManager.instance.playerInputManager.DisableJoining();
    }

    public void OnStart()
    {
        playerNumberSelection.SetActive(true);
        playerSlider.value = playerSlider.minValue;
        eventSystem.SetSelectedGameObject(playerSlider.gameObject);
    }

    public void OnCancel()
    {
        eventSystem.SetSelectedGameObject(startButton);
        playerNumberSelection.SetActive(false);
    }

    public void OnPlay()
    {
        GameManager.instance.playersNumber = (int) playerSlider.value;
        lobby.SetActive(true);
        for (int i = 0; i < playerLobbyAreas.Length; i++)
        {
            if (GameManager.instance.playersNumber > i)
            {
                playerLobbyAreas[i].area.SetActive(true);

                foreach (var image in playerLobbyAreas[i].coloredImages)
                {
                    image.color = GameManager.instance.colors[i];
                }
                
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
        launchParty.interactable = false;

        GameManager.instance.EnableAllControllers();
        GameManager.instance.playerInputManager.EnableJoining();

        eventSystem.SetSelectedGameObject(returnFromLobby.gameObject);

        canCheckPlayer = true;
    }

    private void Update()
    {
        if (!canCheckPlayer) return;
        CheckPlayers();

        if (arePlayersReady) FinaleSetup();
    }

    private void CheckPlayers()
    {
        if (GameManager.instance.playerInputManager.playerCount != GameManager.instance.playersNumber) return;
        arePlayersReady = true;
    } // Si tous les joueurs sont là

    private void FinaleSetup()
    {
        arePlayersReady = false;
        canCheckPlayer = false;
        GameManager.instance.playerInputManager.DisableJoining();

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
    }

    public void OnCancelParty()
    {
        launchParty.interactable = false;
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

        eventSystem.SetSelectedGameObject(startButton.gameObject);
    }

    public void OnPlayerJoin(PlayerInput input)
    {
        var gamepad = input.GetDevice<Gamepad>();
        
        int playerIndex = GameManager.instance.playerInputManager.playerCount;
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
    }

    public void OnBeginParty(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void OnOptions()
    {
        SceneManager.LoadSceneAsync(GameManager.instance.optionMenuIndex, LoadSceneMode.Additive);
    }
}