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
    [SerializeField] private GameObject scorePart;
    [SerializeField] private Transform centerPointScorePart;
    [SerializeField] private TextMeshPro bonusName;
    [SerializeField] private TextMeshPro mainText;
    [SerializeField] private PlayerBonusData[] allPlayerBonusData;
    [SerializeField] private PlayerScoreData[] allPlayerScoreData;

    [Tooltip("In right order")] [SerializeField]
    private BonusData[] allBonus;

    [SerializeField] private CameraManager cam;
    [SerializeField] private CameraZoom showPresenterZoom;
    [SerializeField] private CameraZoom showPlayersZoom;
    [SerializeField] private CameraZoom showFinaleScreenZoom;
    [SerializeField] private CameraZoom finaleViewZoom;

    [SerializeField] private GameObject finaleMenu;
    [SerializeField] private GameObject finaleMenuFirstSelected;

    [Serializable]
    private struct BonusData
    {
        public string frenchName;
        public string englishName;
        public uint points;
    }

    [Serializable]
    private struct PlayerBonusData
    {
        public GameObject bonusDataObj;
        public TextMeshPro bonusDataText;
    }

    [Serializable]
    private struct PlayerScoreData
    {
        public GameObject scoreDataObj;
        public TextMeshPro scoreDataText;
    }

    void Start()
    {
        SceneInitialization();
        StartCoroutine(EndOfGameCinematic());
    }

    private void SceneInitialization()
    {
        var pos = playerInitialPos.position + Vector3.left * (GameManager.instance.allPlayers.Count);

        foreach (var pc in GameManager.instance.allPlayers)
        {
            pc.FinaleSceneInitialization();
            pc.transform.position = pos;
            pc.transform.rotation = Quaternion.Euler(Vector3.up * 180);
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
        #region Setup

        var bonusDictionary = new Dictionary<PlayerController, PlayerBonusData>();
        var scoreDictionary = new Dictionary<PlayerController, PlayerScoreData>();

        for (var i = 0; i < GameManager.instance.allPlayers.Count; i++)
        {
            var pc = GameManager.instance.allPlayers[i];
            bonusDictionary.Add(pc, allPlayerBonusData[i]);
            scoreDictionary.Add(pc, allPlayerScoreData[i]);
        }

        foreach (var data in allPlayerBonusData) data.bonusDataObj.SetActive(false);
        foreach (var data in allPlayerScoreData) data.scoreDataObj.SetActive(false);
        foreach (var kvp in bonusDictionary) kvp.Value.bonusDataObj.SetActive(true);
        foreach (var kvp in scoreDictionary) kvp.Value.scoreDataObj.SetActive(true);

        #endregion

        yield return new WaitForSeconds(1f);

        cam.SetZoom(showPresenterZoom);
        yield return new WaitForSeconds(5f);

        cam.SetZoom(showPlayersZoom);
        yield return new WaitForSeconds(5f);

        cam.SetZoom(showFinaleScreenZoom);
        yield return new WaitForSeconds(5f);

        #region First bonus

        ResetBonusDataText();
        mainText.text = DisplayBonusName(allBonus[0]);
        yield return new WaitForSeconds(2f);
        mainText.text = "";

        bonusPart.SetActive(true);
        bonusName.text = DisplayBonusName(allBonus[0]);
        yield return new WaitForSeconds(1f);
        var friendlyFirePlayerData = GetFriendlyFireHitWinner();
        friendlyFirePlayerData[0].manager.score += (int) allBonus[0].points;
        bonusDictionary[friendlyFirePlayerData[0]].bonusDataText.text =
            friendlyFirePlayerData[0].friendlyFireHit.ToString();

        SetLight(friendlyFirePlayerData[0]);
        yield return new WaitForSeconds(1f);

        foreach (var kvp in bonusDictionary)
        {
            if (kvp.Key == friendlyFirePlayerData[0]) continue;
            kvp.Value.bonusDataText.text = kvp.Key.friendlyFireHit.ToString();
        }

        yield return new WaitForSeconds(1f);
        spotLight.gameObject.SetActive(false);
        bonusPart.SetActive(false);
        yield return new WaitForSeconds(2f);

        #endregion

        #region Second bonus

        ResetBonusDataText();

        mainText.text = DisplayBonusName(allBonus[1]);
        yield return new WaitForSeconds(2f);
        mainText.text = "";

        bonusPart.SetActive(true);
        bonusName.text = DisplayBonusName(allBonus[1]);
        yield return new WaitForSeconds(1f);
        var precisionRatioPlayerData = GetPrecisionRatioWinner();
        precisionRatioPlayerData[0].manager.score += (int) allBonus[1].points;

        bonusDictionary[precisionRatioPlayerData[0]].bonusDataText.text =
            precisionRatioPlayerData[0].precisionRatio.Item3.ToString("F2");

        SetLight(precisionRatioPlayerData[0]);

        yield return new WaitForSeconds(1f);

        foreach (var kvp in bonusDictionary)
        {
            if (kvp.Key == precisionRatioPlayerData[0]) continue;
            kvp.Value.bonusDataText.text = kvp.Key.precisionRatio.Item3.ToString("F2");
        }

        yield return new WaitForSeconds(1f);
        spotLight.gameObject.SetActive(false);
        bonusPart.SetActive(false);
        yield return new WaitForSeconds(2f);

        #endregion

        #region Third bonus

        ResetBonusDataText();

        mainText.text = DisplayBonusName(allBonus[2]);
        yield return new WaitForSeconds(2f);
        mainText.text = "";

        bonusPart.SetActive(true);
        bonusName.text = DisplayBonusName(allBonus[2]);
        yield return new WaitForSeconds(2f);
        var crownTimerWinner = GetCrownDurationWinner();
        crownTimerWinner[0].manager.score += (int) allBonus[2].points;

        bonusDictionary[crownTimerWinner[0]].bonusDataText.text = crownTimerWinner[0].crownTimer.ToString("F1");

        SetLight(crownTimerWinner[0]);

        yield return new WaitForSeconds(1f);

        foreach (var kvp in bonusDictionary)
        {
            if (kvp.Key == crownTimerWinner[0]) continue;
            kvp.Value.bonusDataText.text = kvp.Key.crownTimer.ToString("F1");
        }

        yield return new WaitForSeconds(1f);
        spotLight.gameObject.SetActive(false);
        bonusPart.SetActive(false);
        yield return new WaitForSeconds(2f);

        #endregion

        #region Finale results

        cam.SetZoom(finaleViewZoom);
        yield return new WaitForSeconds(2f);

        centerPointScorePart.localPosition =
            Vector3.right * 3.5f * (Mathf.Abs(GameManager.instance.allPlayers.Count - 4));
        ResetScoreDataText();
        scorePart.SetActive(true);

        yield return new WaitForSeconds(4f);

        foreach (var kvp in scoreDictionary)
        {
            kvp.Value.scoreDataText.text = kvp.Key.manager.score.ToString();
        }

        yield return new WaitForSeconds(2f);

        var winner = GetWinner();
        SetLight(winner);

        yield return new WaitForSeconds(2f);

        #endregion

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

    #region Bonus points

    private string DisplayBonusName(BonusData bonusData)
    {
        string name;
        switch (GameManager.instance.settings.currentLanguage)
        {
            case Language.French:
                name = bonusData.frenchName;
                break;
            case Language.English:
                name = bonusData.englishName;
                break;
            default:
                Debug.LogError("Language not valid.");
                return null;
        }

        return name;
    }

    private void ResetBonusDataText()
    {
        foreach (var data in allPlayerBonusData)
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
            top = (int) pc.friendlyFireHit;
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

    #endregion

    #region Score

    private void ResetScoreDataText()
    {
        foreach (var data in allPlayerScoreData)
        {
            data.scoreDataText.text = "";
        }
    }

    private PlayerController GetWinner()
    {
        PlayerController winner = null;
        var top = -1;
        foreach (var pc in GameManager.instance.allPlayers)
        {
            if (pc.manager.score <= top) continue;
            winner = pc;
            top = pc.manager.score;
        }

        return winner;
    }

    #endregion
}