using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class WantedNoticeEvent : RandomEvent
{
    public static Predicate<(PlayerController, PlayerController)> OnPlayerKilledPredicate;
    private PlayerController wantedPlayer;
    [SerializeField] private int points;
    
    public override void StartEvent()
    {
        OnPlayerKilledPredicate = OnPlayerKilled;
        isRunning = true;
    }

    private void SetWantedPlayer()
    {
        wantedPlayer = GameManager.instance.allPlayers[Random.Range(0, GameManager.instance.allPlayers.Count)];
    }

    private bool OnPlayerKilled((PlayerController killed, PlayerController killer) couple)
    {
        if (couple.killed != wantedPlayer) return false;
        couple.killer.manager.GetPoint(points, couple.killer.transform.position);
        return true;
    }

    public override void EndEvent()
    {
        OnPlayerKilledPredicate = null;
        wantedPlayer = null;
        isRunning = false;
    }

    public override void SetAdditionalInfo()
    {
        SetWantedPlayer();

        var text = GameManager.instance.settings.currentLanguage switch
        {
            Language.French => "Joueur",
            Language.English => "Player",
            _ => throw new ArgumentOutOfRangeException()
        };

        additionalText.text = $"{text} {wantedPlayer.playerIndex}";
        additionalText.color = GameManager.instance.colors[wantedPlayer.playerIndex - 1];
    }
}
