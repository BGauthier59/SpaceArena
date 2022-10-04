using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using static UnityEngine.Random;

public class WavesManager : MonoBehaviour
{
    [SerializeField] private Entrance[] entrances;
    [SerializeField] private EnemyData[] enemyData;

    private bool isWaitingForNextWave;
    [SerializeField] private float durationBeforeNextWave;
    private float timerBeforeNextWave;

    private Wave currentWave;
    [SerializeField] private float spawningDelay;

    [SerializeField] private uint currentRound;
    private int lastDifficulty;
    public int difficulty;
    public int difficultyGap;

    [Serializable]
    public class Entrance
    {
        public Transform entrance;
        public PoolType[] availableEnemies;
        public ushort minRound;
        public bool isAvailable;
    }

    [Serializable]
    public class Wave
    {
        public int entrancesCount;
        public List<(Entrance, EnemiesGroup)> selectedEntrances;
        public float delay;

        public IEnumerator Spawning(Transform entrance, EnemiesGroup group)
        {
            for (int i = 0; i < group.count; i++)
            {
                PoolOfObject.Instance.SpawnFromPool(group.enemyData.enemy, entrance.position, Quaternion.identity);
                yield return new WaitForSeconds(delay);
            }
        }
    }

    [Serializable]
    public class EnemiesGroup
    {
        public EnemyData enemyData;
        public int count;
    }

    [Serializable]
    public struct EnemyData
    {
        public PoolType enemy;
        public int difficulty;

        [Tooltip("Set the minimum round this enemy can spawn at")]
        public ushort minRound;

        [Tooltip("Set the maximum enemies of this type can spawn on one entrance")]
        public ushort maxCount;
    }

    private void Start()
    {
        isWaitingForNextWave = false;
        currentRound = 0;
    }

    private void Update()
    {
        if (!isWaitingForNextWave) return;

        if (timerBeforeNextWave >= durationBeforeNextWave)
        {
            StartNewWave();
        }
        else timerBeforeNextWave += Time.deltaTime;
    }

    public void StartNewWave()
    {
        // Va créer une nouvelle wave et set une difficulté supplémentaire ?
        currentRound++;
        timerBeforeNextWave = 0f;
        isWaitingForNextWave = false;

        lastDifficulty = difficulty;
        difficulty += difficultyGap;

        var maxDif = Mathf.Max((int)(lastDifficulty * .01f), 1);
        var newWave = new Wave
        {
            entrancesCount = Range(1, entrances.Length),
            delay = spawningDelay
        };

        newWave.entrancesCount = Mathf.Min(maxDif, newWave.entrancesCount);

        currentWave = newWave;
        SetWave();
        isWaitingForNextWave = true;
        // Va set les entrées de cette nouvelle wave
    }

    private void SetWave()
    {
        foreach (var e in entrances) e.isAvailable = true;

        currentWave.selectedEntrances = new List<(Entrance, EnemiesGroup)>();
        
        for (int i = 0; i < currentWave.entrancesCount; i++)
        {
            (Entrance, EnemiesGroup) couple;

            couple.Item1 = SetEntrance();
            if (couple.Item1 == null)
            {
                Debug.LogWarning("Can't add this couple because of not valid entrance.");
                continue;
            }
            
            couple.Item1.isAvailable = false;
            couple.Item2 = SetEnemiesGroup(couple.Item1);
            if (couple.Item2 == null)
            {
                Debug.LogWarning("Can't add this couple because of not valid enemies group.");
                continue;
            }

            currentWave.selectedEntrances.Add(couple);
        }

        if (currentWave.selectedEntrances.Count == 0)
        {
            Debug.LogWarning("There's no any valid entrance in this wave.");
            return;
        }

        foreach (var c in currentWave.selectedEntrances)
        {
            StartCoroutine(currentWave.Spawning(c.Item1.entrance, c.Item2));
        }
    }

    private Entrance SetEntrance()
    {
        var securityEntrance = 0;
        Entrance entrance;
        do
        {
            entrance = entrances[Range(0, entrances.Length)];

            securityEntrance++;
            if (securityEntrance >= 100) return null;
            
        } while (!entrance.isAvailable || currentRound <= entrance.minRound);

        return entrance;
    }

    private EnemiesGroup SetEnemiesGroup(Entrance ent)
    {
        var difficultyPerEntrance = (int)(lastDifficulty / (float)currentWave.entrancesCount);
        var enemy = new EnemyData();
        var security = 0;

        PoolType groupType;
        do
        {
            groupType = ent.availableEnemies[Range(0, ent.availableEnemies.Length)];
            foreach (var data in enemyData)
            {
                if (groupType != data.enemy) continue;
                enemy = data;
            }

            security++;
            if (security >= 100) return null;
            
        } while (enemy.minRound > currentRound);

        var group = new EnemiesGroup
        {
            enemyData = enemy,
            count = (int)(difficultyPerEntrance / (float)enemy.difficulty)
        };
        if (group.count > group.enemyData.maxCount) group.count = group.enemyData.maxCount;

        return group;
    }
}