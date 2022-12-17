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
    [SerializeField] private Light spotLight;

    [SerializeField] private GameObject bonusPart;
    [SerializeField] private TextMeshPro bonusName;
    [SerializeField] private TextMeshPro mainText;
    [SerializeField] private PlayerData[] allPlayerData;

    [Tooltip("In right order")] [SerializeField]
    private BonusName[] allBonusNames;

    [SerializeField] private CameraManager cam;
    [SerializeField] private CameraZoom showPresenterZoom;
    [SerializeField] private CameraZoom showPlayersZoom;
    [SerializeField] private CameraZoom showFinaleScreenZoom;
    [SerializeField] private CameraZoom finaleViewZoom;

    [SerializeField] private GameObject finaleMenu;
    [SerializeField] private GameObject finaleMenuFirstSelected;
    
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
        
        spotLight.gameObject.SetActive(false);
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
        
        yield return new WaitForSeconds(1f);

        // Caméra s'approche du présentateur
        cam.SetZoom(showPresenterZoom);
        
        yield return new WaitForSeconds(5f);

        // Caméra s'oriente vers les joueurs
        cam.SetZoom(showPlayersZoom);

        yield return new WaitForSeconds(5f);

        // Caméra s'oriente vers l'écran
        cam.SetZoom(showFinaleScreenZoom);

        yield return new WaitForSeconds(5f);

        // Friendly fire
        ResetBonusDataText();

        mainText.text = DisplayBonusName(allBonusNames[0]);
        yield return new WaitForSeconds(2f);
        mainText.text = "";

        bonusPart.SetActive(true);
        bonusName.text = DisplayBonusName(allBonusNames[0]);
        yield return new WaitForSeconds(1f);
        var friendlyFirePlayerData = GetFriendlyFireHitWinner();
        dict[friendlyFirePlayerData[0]].bonusDataText.text = friendlyFirePlayerData[0].friendlyFireHit.ToString();

        SetLight(friendlyFirePlayerData[0]);

        yield return new WaitForSeconds(1f);
        for (var i = 1; i < friendlyFirePlayerData.Length; i++)
        {
            var player = friendlyFirePlayerData[i];
            dict[player].bonusDataText.text = player.friendlyFireHit.ToString();
        }
        yield return new WaitForSeconds(1f);
        spotLight.gameObject.SetActive(false);

        bonusPart.SetActive(false);

        yield return new WaitForSeconds(2f);

        // Ratio precision
        ResetBonusDataText();
        
        mainText.text = DisplayBonusName(allBonusNames[1]);
        yield return new WaitForSeconds(2f);
        mainText.text = "";
        
        bonusPart.SetActive(true);
        bonusName.text = DisplayBonusName(allBonusNames[1]);
        yield return new WaitForSeconds(1f);
        var precisionRatioPlayerData = GetPrecisionRatioWinner();
        dict[precisionRatioPlayerData[0]].bonusDataText.text =
            precisionRatioPlayerData[0].precisionRatio.Item3.ToString("F2");

        SetLight(precisionRatioPlayerData[0]);
        
        yield return new WaitForSeconds(1f);
        for (var i = 1; i < precisionRatioPlayerData.Length; i++)
        {
            var player = precisionRatioPlayerData[i];
            dict[player].bonusDataText.text = player.precisionRatio.Item3.ToString("F2");
        }
        yield return new WaitForSeconds(1f);
        spotLight.gameObject.SetActive(false);

        bonusPart.SetActive(false);

        yield return new WaitForSeconds(2f);

        // Crown
        ResetBonusDataText();
        
        mainText.text = DisplayBonusName(allBonusNames[2]);
        yield return new WaitForSeconds(2f);
        mainText.text = "";
        
        bonusPart.SetActive(true);
        bonusName.text = DisplayBonusName(allBonusNames[2]);
        yield return new WaitForSeconds(2f);
        var crownTimerWinner = GetCrownDurationWinner();
        dict[crownTimerWinner[0]].bonusDataText.text = crownTimerWinner[0].crownTimer.ToString("F1");
        
        SetLight(crownTimerWinner[0]);
        
        yield return new WaitForSeconds(1f);
        for (var i = 1; i < crownTimerWinner.Length; i++)
        {
            var player = crownTimerWinner[i];
            dict[player].bonusDataText.text = player.crownTimer.ToString("F1");
        }
        yield return new WaitForSeconds(1f);
        spotLight.gameObject.SetActive(false);

        bonusPart.SetActive(false);
        yield return new WaitForSeconds(2f);
        
        cam.SetZoom(finaleViewZoom);
        
        // Give finale results!
        
        yield return new WaitForSeconds(5f);
        finaleMenu.SetActive(true);
        GameManager.instance.eventSystem.SetSelectedGameObject(finaleMenuFirstSelected);
    }

    private void SetLight(PlayerController winner)
    {
        spotLight.color = GameManager.instance.colors[winner.playerIndex - 1];
        spotLight.transform.position = new Vector3(winner.transform.position.x,
            spotLight.transform.position.y, winner.transform.position.z);
        spotLight.gameObject.SetActive(true);
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