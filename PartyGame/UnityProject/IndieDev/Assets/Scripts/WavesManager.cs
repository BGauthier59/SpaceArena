using System;
using System.Collections;
using System.Collections.Generic;
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
    public struct EnemiesGroup
    {
        public EnemyData enemyData;
        public int count;
    }

    [Serializable]
    public struct EnemyData
    {
        public PoolType enemy;
        public int difficulty;
        [Tooltip("Set the minimum round this enemy can spawn at")] public ushort minRound;
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

    public void SetWave()
    {
        // Set les entrées utilisées par la wave
        // Pour chaque entrée, déterminer le groupe d'ennemis qui y spawn

        foreach (var e in entrances) e.isAvailable = true;
        
        currentWave.selectedEntrances = new List<(Entrance, EnemiesGroup)>();

        var difficultyPerEntrance = (int) (lastDifficulty / (float) currentWave.entrancesCount);

        for (int i = 0; i < currentWave.entrancesCount; i++)
        {
            (Entrance, EnemiesGroup) couple;

            // Set entrance
            do
            {
                couple.Item1 = entrances[Range(0, entrances.Length)];
            } while (!couple.Item1.isAvailable);

            // Set enemies group

            var enemy = new EnemyData();
            var security = 0;
            
            do
            {
                var groupType = couple.Item1.availableEnemies[Range(0, couple.Item1.availableEnemies.Length)];
                foreach (var data in enemyData)
                {
                    if (groupType != data.enemy) continue;
                    enemy = data;
                }
                
                security++;
                if (security >= 100)
                {
                    Debug.LogWarning("No available enemy seems to be found... Breaking.");
                    break;
                }
                
            } while (enemy.minRound > currentRound);
            
            var group = new EnemiesGroup
            {
                enemyData = enemy,
                count = (int) (difficultyPerEntrance / (float) enemy.difficulty)
            };

            couple.Item2 = group;
            
            currentWave.selectedEntrances.Add(couple);
        }

        foreach (var c in currentWave.selectedEntrances)
        {
            StartCoroutine(currentWave.Spawning(c.Item1.entrance, c.Item2));
        }
    }
}