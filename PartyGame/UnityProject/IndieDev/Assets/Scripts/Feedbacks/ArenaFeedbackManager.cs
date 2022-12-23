using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ArenaFeedbackManager : MonoBehaviour
{
    private uint excitementScore;

    [Tooltip("From the lowest to the highest score")] [SerializeField]
    private ArenaFeedback[] allArenaFeedbacks;

    [SerializeField] private float excitementDuration;
    private float excitementTimer;
    [SerializeField] private uint decreasingExcitementValue;

    [Header("VFX")] [SerializeField] private ParticleSystem[] flashEffects;

    [SerializeField] private TextMeshProUGUI excitementTextDebug;

    public uint highestExcitementScore
    {
        get
        {
            uint highest = 0;
            foreach (var feedback in allArenaFeedbacks)
            {
                if (feedback.excitementNeeded < highest) continue;
                highest = feedback.excitementNeeded;
            }
            Debug.Log("Cast highest feedbacks!");
            return highest;
        }
    }

    [Serializable]
    private struct ArenaFeedback
    {
        [TextArea(2, 2)] public string description;
        public uint excitementNeeded;
        public UnityEvent excitementFeedbacks;
    }

    public Action<uint> OnExcitementGrows;

    public void Initialization()
    {
        OnExcitementGrows = null;
        OnExcitementGrows += ExcitementGrows;
        OnExcitementGrows += CheckExcitation;
    }

    private void ExcitementGrows(uint excitement)
    {
        excitementTimer = 0f;
        excitementScore += excitement;
        excitementTextDebug.text = $"DEBUG Excitement : {excitementScore}";
    }

    private void ExcitementDecreases()
    {
        var value = (int) (excitementScore - decreasingExcitementValue);
        if (value < 0) value = 0;
        excitementScore = (uint) value;
        excitementTextDebug.text = $"DEBUG Excitement : {excitementScore}";
    }

    private void CheckExcitation(uint excitement)
    {
        for (int i = allArenaFeedbacks.Length - 1; i >= 0; i--)
        {
            var feedback = allArenaFeedbacks[i];
            if (feedback.excitementNeeded > excitementScore) continue;
            feedback.excitementFeedbacks?.Invoke();
            return;
        }
    }

    public void CheckTimer(float delta)
    {
        if (excitementTimer >= excitementDuration)
        {
            excitementTimer = 0f;
            ExcitementDecreases();
        }
        else excitementTimer += delta;
    }

    public void ForceExcitementToValue(uint value)
    {
        Debug.LogWarning($"You forces excitement to {value}");
        excitementScore = value;
        excitementTextDebug.text = $"DEBUG Excitement : {excitementScore}";
    }

    #region Excitement Feedbacks

    public void StartFlashEffect()
    {
        foreach (var ps in flashEffects)
        {
            if (!ps.isPlaying) ps.Play();
        }
    }

    #endregion
}