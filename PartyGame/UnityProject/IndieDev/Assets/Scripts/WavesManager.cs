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


    /*
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
    */
    
    /*
     Wave system
     
     Toutes les X secondes, spawn une wave
     
     Une wave = sélection de plusieurs entrées, et spawn d'ennemis sur ces entrées
     
     Spawn d'ennemis sur les entrées : nombre aléatoire et type d'ennemis aléatoire
     
     */
    
    
    /*

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
        waves.Clear();
        entrancesUsed.Clear();
        
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
        Debug.Log(numberOfEnemies);
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
                //Debug.Log("Je spawn");
                var enemyToSpawn = enemyGroup;
                for (int i = 0; i < enemyToSpawn.enemyCount - 1; i++)
                {
                    PoolOfObject.Instance.SpawnFromPool(enemyToSpawn.enemyType.enemyName, entrances[wave.entrance].position, Quaternion.identity);
                    //Debug.Log(entrances[wave.entrance].position);
                    yield return new WaitForSeconds(enemySpawnDelay);
                }
            }
        }

        waitingForNextWave = true;
    }
    
    */
}