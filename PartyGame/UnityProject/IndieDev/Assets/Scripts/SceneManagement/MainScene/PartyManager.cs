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

public class PartyManager : MonoBehaviour
{
    private EventSystem eventSystem;

    [SerializeField] private Transform[] allSpawningPoints;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private WavesManager wavesManager;

    [Header("Party Parameters")] 
    [Tooltip("Duration in seconds")] [SerializeField] private float partyDuration;
    private float partyTimer;
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

    private void Start()
    {
        Initialization();
    }

    private void Update()
    {
        CheckTimer();
    }

    private void Initialization()
    {
        hasPartyBegun = false;
        eventSystem = GameManager.instance.eventSystem;
        GameManager.instance.playerInputManager = null;
        GameManager.instance.mainCanvas = mainCanvas;
        GameManager.instance.cameraShake = cameraShake;

        // Initializing players

        for (int i = 0; i < GameManager.instance.allPlayers.Count; i++)
        {
            var player = GameManager.instance.allPlayers[i];
            
            player.transform.position = allSpawningPoints[i].position;
            player.rd.material.color = GameManager.instance.colors[player.playerIndex - 1];
            player.PartyBegins();
        }
        
        // Initializing timer

        partyTimer = partyDuration;
        
        // Starts timer

        GameManager.instance.EnableAllControllers();
        hasPartyBegun = true;
        wavesManager.NewRound();
    }

    public void CheckTimer()
    {
        if (!hasPartyBegun) return;
        
        if (partyTimer <= 0)
        {
            partyTimer = 0;
            DisplayScore();
            // End of game
        }
        else
        {
            partyTimer -= Time.deltaTime;
        }

        timerText.text = ((int) partyTimer).ToString();
    }

    private void DisplayScore()
    {
        endOfParty.SetActive(true);
        for (int i = 0; i < scoreAreas.Length; i++)
        {
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
        SceneManager.LoadScene(GameManager.instance.mainMenuIndex);
    }
}