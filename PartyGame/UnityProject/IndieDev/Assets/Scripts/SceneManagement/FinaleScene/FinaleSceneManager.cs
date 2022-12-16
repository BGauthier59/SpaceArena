using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinaleSceneManager : MonoBehaviour
{
    [SerializeField] private Transform playerInitialPos;
    [SerializeField] private float distanceBetweenPlayers;

    void Start()
    {
        SceneInitialization();
    }

    private void SceneInitialization()
    {
        var pos = playerInitialPos.position + Vector3.left * (GameManager.instance.allPlayers.Count - 1);
        
        foreach (var pc in GameManager.instance.allPlayers)
        {
            pc.transform.position = pos;
            pc.transform.rotation = Quaternion.identity;
            pos += Vector3.right * distanceBetweenPlayers;
        }
    }

    public void OnQuit()
    {
        GameManager.instance.DisableAllControllers();

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