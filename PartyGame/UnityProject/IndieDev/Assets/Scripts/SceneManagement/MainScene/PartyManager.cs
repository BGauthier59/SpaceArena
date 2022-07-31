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

    [SerializeField] private PlayerController playerPrefab;
    [SerializeField] private Material[] playerMaterials;
    [SerializeField] private Transform[] allSpawningPoints;
    [SerializeField] private TextMeshProUGUI timerText;

    [Tooltip("Duration in seconds")] [SerializeField]
    private float partyDuration;
    private float partyTimer;

    [SerializeField] private GameObject endOfParty;
    [SerializeField] private ScoreArea[] scoreAreas;
    [SerializeField] private Button backToMainMenu;

    [Serializable]
    public struct ScoreArea
    {
        public GameObject area;
        public TextMeshProUGUI scoreText;
    }

    private void Start()
    {
        eventSystem = EventSystem.current;
        InitializePlayers();
    }

    private void Update()
    {
        CheckTimer();
    }

    public void InitializePlayers()
    {
        GameManager.instance.playerInputManager = null;

        for (int i = 0; i < GameManager.instance.allPlayers.Count; i++)
        {
            var player = GameManager.instance.allPlayers[i];
            
            player.playerInput.uiInputModule = player.dataGamepad.gamepad != Gamepad.all[0] ? 
                null : eventSystem.GetComponent<InputSystemUIInputModule>();
            
            player.transform.position = allSpawningPoints[i].position;
            player.rd.sharedMaterial = playerMaterials[i];
            player.PartyBegins();
        }

        partyTimer = partyDuration;
    }

    public void CheckTimer()
    {
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

    public void DisplayScore()
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
        
        OnQuit(1);
    }

    public void OnQuit(int index)
    {
        for (int i = GameManager.instance.allPlayers.Count - 1; i >= 0; i--)
        {
            var player = GameManager.instance.allPlayers[i];
            Destroy(player.gameObject);
        }

        GameManager.instance.allPlayers.Clear();
        GameManager.instance.playersNumber = 0;
        SceneManager.LoadScene(index);
    }
}