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
    [SerializeField] private TextMeshPro bonusName;
    [SerializeField] private PlayerData[] allPlayerData;

    [Tooltip("In right order")] [SerializeField]
    private BonusName[] allBonusNames;

    [Serializable]
    private struct BonusName
    {
        public string frenchName;
        public string englishName;
    }

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
        foreach (var kvp in dict)
        {
            kvp.Value.bonusDataObj.SetActive(true);
        }

        //

        yield return new WaitForSeconds(2f);

        // Friendly fire
        ResetBonusDataText();
        bonusPart.SetActive(true);
        bonusName.text = DisplayBonusName(allBonusNames[0]);
        yield return new WaitForSeconds(2f);
        var friendlyFirePlayerData = GetFriendlyFireHitWinner();
        dict[friendlyFirePlayerData[0]].bonusDataText.text = friendlyFirePlayerData[0].friendlyFireHit.ToString();
        yield return new WaitForSeconds(2f);
        for (var i = 1; i < friendlyFirePlayerData.Length; i++)
        {
            var player = friendlyFirePlayerData[i];
            dict[player].bonusDataText.text = player.friendlyFireHit.ToString();
        }
        yield return new WaitForSeconds(2f);

        bonusPart.SetActive(false);

        yield return new WaitForSeconds(3f);

        // Ratio precision
        ResetBonusDataText();
        bonusPart.SetActive(true);
        bonusName.text = DisplayBonusName(allBonusNames[1]);
        yield return new WaitForSeconds(2f);
        var precisionRatioPlayerData = GetPrecisionRatioWinner();
        dict[precisionRatioPlayerData[0]].bonusDataText.text =
            precisionRatioPlayerData[0].precisionRatio.Item3.ToString("F2");
        yield return new WaitForSeconds(2f);
        for (var i = 1; i < precisionRatioPlayerData.Length; i++)
        {
            var player = precisionRatioPlayerData[i];
            dict[player].bonusDataText.text = player.precisionRatio.Item3.ToString("F2");
        }
        yield return new WaitForSeconds(2f);

        bonusPart.SetActive(false);

        yield return new WaitForSeconds(3f);

        // Crown
        ResetBonusDataText();
        bonusPart.SetActive(true);
        bonusName.text = DisplayBonusName(allBonusNames[2]);
        yield return new WaitForSeconds(2f);
        var crownTimerWinner = GetCrownDurationWinner();
        dict[crownTimerWinner[0]].bonusDataText.text = crownTimerWinner[0].crownTimer.ToString("F1");
        yield return new WaitForSeconds(2f);
        for (var i = 1; i < crownTimerWinner.Length; i++)
        {
            var player = crownTimerWinner[i];
            dict[player].bonusDataText.text = player.crownTimer.ToString("F1");
        }
        yield return new WaitForSeconds(2f);

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

    private string DisplayBonusName(BonusName bonusName)
    {
        string name;
        switch (GameManager.instance.settings.currentLanguage)
        {
            case Language.French:
                name = bonusName.frenchName;
                break;
            case Language.English:
                name = bonusName.englishName;
                break;
            default:
                Debug.LogError("Language not valid.");
                return null;
        }

        return name;
    }

    private void ResetBonusDataText()
    {
        foreach (var data in allPlayerData)
        {
            data.bonusDataText.text = "";
        }
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
            top = (int)pc.friendlyFireHit;
        }

        data.Add(winner);
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
            top = pc.precisionRatio.Item3;
        }

        data.Add(winner);
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
            top = pc.crownTimer;
        }

        data.Add(winner);
        foreach (var pc in GameManager.instance.allPlayers)
        {
            if (pc == winner) continue;
            data.Add(pc);
        }

        return data.ToArray();
    }
}