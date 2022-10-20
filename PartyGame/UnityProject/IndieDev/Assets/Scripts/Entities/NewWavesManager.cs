using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class NewWavesManager : MonoBehaviour
{
    [SerializeField] private Wave[] waves;
    [SerializeField] private Entrance[] entrances;
    [SerializeField] private EnemyData[] enemyData;

    private Wave currentWave;
    [SerializeField] private float spawningDelay = 0.25f;
    private int round;
    public int difficulty;

    public bool isSpawning;
    private float durationBeforeNextWave;
    private float timerBeforeNextWave;

    private RandomEventManager randomEventManager;

    [Serializable]
    public class Wave
    {
        public float durationBeforeNextActivation;
        public ushort difficultyIncrementation;
        public int2 minMaxEntrances;
        [HideInInspector] public int entrancesCount;
        public bool fullRandomEntrances;

        [Tooltip("Only is fullRandomEntrances is false")]
        public SelectableEntrancesStruct[] selectableEntrancesStructs;

        public int[][] selectableEntrancesIndex;

        [HideInInspector] public int difficultyPerEntrance;
        public List<(Entrance, EnemiesGroup)> selectedEntrances;
        [HideInInspector] public float delay;

        public IEnumerator Spawning()
        {
            Debug.Log(selectedEntrances.Count);
            foreach (var (entrance, enemiesGroup) in selectedEntrances)
            {
                Debug.Log(enemiesGroup.count);
                for (int i = 0; i < enemiesGroup.count; i++)
                {
                    PoolOfObject.Instance.SpawnFromPool(enemiesGroup.enemyData.enemy, entrance.entrance.position, Quaternion.identity);
                    yield return new WaitForSeconds(delay);
                }
            }

            GameManager.instance.partyManager.wavesManager.isSpawning = false;
        }

        [Serializable]
        public struct SelectableEntrancesStruct
        {
            public int[] indexes;
        }
    }

    [Serializable]
    public class Entrance
    {
        public Transform entrance;
        public PoolType[] availableEnemies;
        public ushort minRound;
        public bool isAvailable;

        [Tooltip("Set the maximum of enemies that can spawn at this entrance")]
        public ushort maxCount;
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

        [Tooltip("Set the maximum of enemies of this type that can spawn at one entrance")]
        public ushort maxCount;
    }

    public void Initialization()
    {
        isSpawning = true;
        round = -1;
        randomEventManager = GameManager.instance.partyManager.randomEventManager;

        foreach (var w in waves)
        {
            w.selectableEntrancesIndex = new int[w.selectableEntrancesStructs.Length][];

            for (int i = 0; i < w.selectableEntrancesStructs.Length; i++)
            {
                w.selectableEntrancesIndex[i] = new int[w.selectableEntrancesStructs[i].indexes.Length];

                for (int j = 0; j < w.selectableEntrancesStructs[i].indexes.Length; j++)
                {
                    w.selectableEntrancesIndex[i][j] = w.selectableEntrancesStructs[i].indexes[j];
                }
            }
        }
    }

    private void Update()
    {
        if (isSpawning) return;
        if (randomEventManager.isStartingEvent) return;

        if (timerBeforeNextWave >= durationBeforeNextWave)
        {
            StartNewWave();
        }
        else timerBeforeNextWave += Time.deltaTime;
    }

    public void StartNewWave()
    {
        round++;
        timerBeforeNextWave = 0f;
        isSpawning = true;
        currentWave = null;

        if (round > waves.Length - 1)
        {
            Debug.LogWarning("There's no wave any more.");
            return;
        }
        
        var data = waves[round];
        difficulty += data.difficultyIncrementation;
        durationBeforeNextWave = data.durationBeforeNextActivation;
        
        // Set new wave data
        var newWave = new Wave()
        {
            entrancesCount = Random.Range(data.minMaxEntrances.x, data.minMaxEntrances.y + 1),
            fullRandomEntrances = data.fullRandomEntrances,
            selectableEntrancesIndex = data.selectableEntrancesIndex,
            delay = spawningDelay
        };

        if (!newWave.fullRandomEntrances && newWave.entrancesCount != newWave.selectableEntrancesIndex.Length)
        {
            newWave.entrancesCount = newWave.selectableEntrancesIndex.Length;
        }

        newWave.difficultyPerEntrance = (int)(difficulty / (float)newWave.entrancesCount);
        currentWave = newWave;
        SetWave();
    }

    private void SetWave()
    {
        foreach (var e in entrances) e.isAvailable = true;
        currentWave.selectedEntrances = new List<(Entrance, EnemiesGroup)>();

        if (!currentWave.fullRandomEntrances) SetDataIfNotFullRandom();

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

            Debug.Log($"Couple added : {couple.Item1.entrance.name} / {couple.Item2.enemyData.enemy.ToString()}");
            currentWave.selectedEntrances.Add(couple);
        }

        if (currentWave.selectedEntrances.Count == 0)
        {
            Debug.LogWarning("There's no any valid entrance in this wave.");
            return;
        }
        
        StartCoroutine(currentWave.Spawning());
    }

    private Dictionary<int, int> indexSortedIndexReal = new Dictionary<int, int>();
    private List<int> allLengths = new List<int>();

    private void SetDataIfNotFullRandom()
    {
        indexSortedIndexReal.Clear();
        var counter = 0;
        for (int i = 0; i < currentWave.selectableEntrancesIndex.Length; i++)
        {
            for (int j = 0; j < currentWave.selectableEntrancesIndex[i].Length; j++)
            {
                indexSortedIndexReal.Add(counter, currentWave.selectableEntrancesIndex[i][j]);
                counter++;
            }
        }

        allLengths.Clear();
        for (int i = 0; i < currentWave.selectableEntrancesIndex.Length; i++) allLengths.Add(i);

        return;
        
        // Vérification
        foreach (var kv in indexSortedIndexReal)
        {
            Debug.Log($"Dictionary : {kv.Key} ; {kv.Value}");
        }

        // Vérification
        foreach (var l in allLengths)
        {
            Debug.Log($"Lengths : {l}");
        }
    }

    private Entrance SetEntrance()
    {
        var securityEntrance = 0;
        Entrance entrance;

        if (currentWave.fullRandomEntrances)
        {
            do
            {
                securityEntrance++;
                if (securityEntrance >= 100) return null;
                entrance = entrances[Random.Range(0, entrances.Length)];
            } while (!entrance.isAvailable || round < entrance.minRound);
        }
        else
        {
            var randomLengthIndex = Random.Range(0, allLengths.Count - 1);
            var lengthValue = allLengths[randomLengthIndex];

            var minIndex = 0;
            for (int i = 0; i < lengthValue; i++)
            {
                minIndex += currentWave.selectableEntrancesIndex[i].Length;
            }
            var maxIndex = minIndex + currentWave.selectableEntrancesIndex[lengthValue].Length - 1;
            var randomIndex = Random.Range(minIndex, maxIndex + 1);
            
            entrance = entrances[indexSortedIndexReal[randomIndex]];
            allLengths.Remove(lengthValue);
        }

        return entrance;
    }

    private EnemiesGroup SetEnemiesGroup(Entrance ent)
    {
        var enemy = new EnemyData();
        var security = 0;

        PoolType groupType;
        do
        {
            groupType = ent.availableEnemies[Random.Range(0, ent.availableEnemies.Length)];
            foreach (var data in enemyData)
            {
                if (groupType != data.enemy) continue;
                enemy = data;
            }

            security++;
            if (security >= 100) return null;
        } while (enemy.minRound > round);

        var group = new EnemiesGroup
        {
            enemyData = enemy,
            count = (int)(currentWave.difficultyPerEntrance / (float)enemy.difficulty)
        };
        group.count = Mathf.Min(group.count, group.enemyData.maxCount);
        group.count = Mathf.Min(group.count, ent.maxCount);
        if (group.count == 0)
        {
            group.count = 1;
            Debug.Log("Count set to 1.");
        }

        return group;
    }
}