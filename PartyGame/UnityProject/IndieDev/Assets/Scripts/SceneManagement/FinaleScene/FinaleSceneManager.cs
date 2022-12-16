using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinaleSceneManager : MonoBehaviour
{
    [SerializeField] private Transform playerInitialPos;
    [SerializeField] private float distanceBetweenPlayers;

    [SerializeField] private GameObject bonusPart;
    [SerializeField] private PlayerData[] allPlayerData;

    [Serializable]
    private struct PlayerData
    {
        public GameObject bonusDataObj;
        public TextMeshPro bonusDataText;
    }

    void Start()
    {
        SceneInitialization();
        StartCoroutine(EndOfGameCinematic());
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

    private IEnumerator EndOfGameCinematic()
    {
        var dict =
            new Dictionary<PlayerController, PlayerData>();

        // Set dictionary
        for (var i = 0; i < GameManager.instance.allPlayers.Count; i++)
        {
            var pc = GameManager.instance.allPlayers[i];
            dict.Add(pc, allPlayerData[i]);
        }

        foreach (var data in allPlayerData) data.bonusDataObj.SetActive(false);

        
        //
        
        yield return new WaitForSeconds(2f);

        bonusPart.SetActive(true);
        var friendlyFireWinner = GetFriendlyFireHitWinner();
        dict[friendlyFireWinner[0]].bonusDataText.text = friendlyFireWinner[0].friendlyFireHit.ToString();
        yield return new WaitForSeconds(2f);
        for (var i = 0; i < GameManager.instance.allPlayers.Count; i++)
        {
            var player = GameManager.instance.allPlayers[i];
            if (player == friendlyFireWinner[0]) continue;
            allPlayerData[i].bonusDataObj.SetActive(true);
            allPlayerData[i].bonusDataText.text = player.friendlyFireHit.ToString();
        }
        bonusPart.SetActive(false);

        yield return new WaitForSeconds(3f);

        bonusPart.SetActive(true);
        var precisionRatioWinner = GetPrecisionRatioWinner();
        dict[precisionRatioWinner[0]].bonusDataText.text = precisionRatioWinner[0].precisionRatio.Item3.ToString("F1");
        yield return new WaitForSeconds(2f);
        for (var i = 0; i < GameManager.instance.allPlayers.Count; i++)
        {
            var player = GameManager.instance.allPlayers[i];
            if (player == precisionRatioWinner[0]) continue;
            allPlayerData[i].bonusDataText.text = player.precisionRatio.Item3.ToString("F1");
        }
        bonusPart.SetActive(false);

        yield return new WaitForSeconds(3f);

        bonusPart.SetActive(true);
        var crownTimerWinner = GetCrownDurationWinner();
        dict[crownTimerWinner[0]].bonusDataText.text = crownTimerWinner[0].crownTimer.ToString("F1");
        yield return new WaitForSeconds(2f);
        for (var i = 0; i < GameManager.instance.allPlayers.Count; i++)
        {
            var player = GameManager.instance.allPlayers[i];
            if (player == crownTimerWinner[0]) continue;
            allPlayerData[i].bonusDataText.text = player.crownTimer.ToString("F1");
        }
        bonusPart.SetActive(false);

        // Caméra s'approche du présentateur

        // Caméra s'oriente vers les joueurs

        // Caméra s'oriente vers l'écran

        // Premier bonus : le joueur qui s'est pris le plus de coup de friendly fire

        // On affiche les bonus data et les players data

        // On met en évidence le joueur qui a gagné (spot de lumière et feedback écran)

        // On affiche les points bonus associés

        yield return new WaitForSeconds(1f);
    }

    private PlayerController[] GetFriendlyFireHitWinner()
    {
        var data = new List<PlayerController>();
        PlayerController winner = null;
        var top = -1;
        foreach (var pc in GameManager.instance.allPlayers)
        {
            if (pc.friendlyFireHit <= top) continue;
            winner = pc;
            data.Add(winner);
            top = (int)pc.friendlyFireHit;
        }

        foreach (var pc in GameManager.instance.allPlayers)
        {
            if (pc == winner) continue;
            data.Add(pc);
        }

        return data.ToArray();
    }

    private PlayerController[] GetPrecisionRatioWinner()
    {
        var data = new List<PlayerController>();
        PlayerController winner = null;
        var top = -1f;
        foreach (var pc in GameManager.instance.allPlayers)
        {
            if (pc.precisionRatio.Item3 <= top) continue;
            winner = pc;
            data.Add(winner);
            top = pc.precisionRatio.Item3;
        }

        foreach (var pc in GameManager.instance.allPlayers)
        {
            if (pc == winner) continue;
            data.Add(pc);
        }

        return data.ToArray();
    }

    private PlayerController[] GetCrownDurationWinner()
    {
        var data = new List<PlayerController>();
        PlayerController winner = null;
        var top = -1f;
        foreach (var pc in GameManager.instance.allPlayers)
        {
            if (pc.crownTimer <= top) continue;
            winner = pc;
            data.Add(winner);
            top = pc.crownTimer;
        }

        foreach (var pc in GameManager.instance.allPlayers)
        {
            if (pc == winner) continue;
            data.Add(pc);
        }

        return data.ToArray();
    }
}