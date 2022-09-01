using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.Random;
using Random = UnityEngine.Random;

public class WavesManager : MonoBehaviour
{
    private int roundDifficultyIndex = 80;
    [SerializeField] private int waveDifficulty = 40;
    [SerializeField] private int enemyGroupDifficulty = 30;
    [SerializeField] private Enemy[] enemies;
    [SerializeField] private Transform[] entrances;
    [SerializeField] private Transform enemiesParent;
    [SerializeField] private int waveTimer;
    [SerializeField] private float enemySpawnDelay;
    private List<Waves> waves;
    private List<int> entrancesUsed;

    private bool waitingForNextWave;
    [SerializeField] private float durationBeforeNextWave;
    private float timerBeforeNextWave;
    

    [Serializable]
    public class Enemy
    {
        public PoolType enemyName;
        public int enemyDifficulty;
    }

    private class EnemyGroup
    {
        public Enemy enemyType;
        public int enemyCount;
        public PoolType enemyPoolType;
    }

    private class Waves
    {
        public int entrance;
        public List<EnemyGroup> enemies;
    }

    private void Awake()
    {
        waves = new List<Waves>();
        entrancesUsed = new List<int>();
    }

    private void Update()
    {
        if (!waitingForNextWave) return;

        if (timerBeforeNextWave >= durationBeforeNextWave)
        {
            NewRound();
        }
        else timerBeforeNextWave += Time.deltaTime;
    }

    public void NewRound()
    {
        waitingForNextWave = false;
        timerBeforeNextWave = 0f;
        
        var smallWaves = roundDifficultyIndex / waveDifficulty;
        for (int i = 0; i < smallWaves; i++)
        {
            WaveRandomizer();
        }
        
        StartCoroutine(SpawnRound());
    }
    
    private void WaveRandomizer()
    {
        // Ajoute une vague
        
        var wave = new Waves();
        wave.enemies = new List<EnemyGroup>();
        var groupsCount = roundDifficultyIndex / enemyGroupDifficulty;
        for (int i = 0; i < groupsCount - 1; i++)
        {
            wave.enemies.Add(EnemyRandomizer());
        }
        SetEntrance(wave);
        waves.Add(wave);
    }
    
    private EnemyGroup EnemyRandomizer()
    {
        // Définie un ennemi aléatoire
        
        var enemyGroup = new EnemyGroup
        {
            enemyType = enemies[Range(0, enemies.Length)]
        };
        int numberOfEnemies = roundDifficultyIndex / enemyGroup.enemyType.enemyDifficulty;
        enemyGroup.enemyCount = Range(numberOfEnemies - 1, numberOfEnemies + 2);
        return enemyGroup;
    }

    private void SetEntrance(Waves wave)
    {
        int entranceSet = Range(0, entrances.Length);
        foreach (var used in entrancesUsed)
        {
            if (entranceSet == used)
            {
                Debug.Log("Entrance already used");
                SetEntrance(wave);
                return;
            }
        }
        wave.entrance = entranceSet;
        entrancesUsed.Add( wave.entrance);
    }

    private IEnumerator SpawnRound()
    {
        foreach (var wave in waves)
        {
            foreach (var enemyGroup in wave.enemies)
            {
                Debug.Log("Je spawn");
                var enemyToSpawn = enemyGroup;
                for (int i = 0; i < enemyToSpawn.enemyCount - 1; i++)
                {
                    PoolOfObject.Instance.SpawnFromPool(enemyToSpawn.enemyType.enemyName, entrances[wave.entrance].position, Quaternion.identity);
                    Debug.Log(entrances[wave.entrance].position);
                    yield return new WaitForSeconds(enemySpawnDelay);
                }
            }
        }

        waitingForNextWave = true;
    }

    // Ratio ta coroutine :
    private IEnumerator TimeBetweenWaves()
    {
        yield return new WaitForSeconds(waveTimer);
    }
}