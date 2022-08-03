using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using static UnityEngine.Random;
using Random = UnityEngine.Random;

public class WavesManager : MonoBehaviour
{
    private int waveDifficultyIndex = 80;
    [SerializeField] private Enemy[] enemies;
    [SerializeField] private Transform[] entrances;
    [SerializeField] private Transform enemiesParent;
    [SerializeField] private int waveTimer;
    [SerializeField] private float enemySpawnDelay;
    private List<Waves> waves;
    private List<int> entrancesUsed;

    [Serializable]
    public class Enemy
    {
        public string enemyName;
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

    private void Start()
    {
        waves = new List<Waves>();
        entrancesUsed = new List<int>();
        EnemyRandomizer();
    }

    private void EnemyRandomizer()
    {
        var enemyGroupList = new List<EnemyGroup>();
        var enemyGroupCount = 2;
        for (int i = 0; i < enemyGroupCount; i++)
        {
            var enemyGroup = new EnemyGroup();
            
            enemyGroup.enemyType = enemies[Range(0, enemies.Length)];
            int numberOfEnemies = waveDifficultyIndex / enemyGroup.enemyType.enemyDifficulty;
            enemyGroup.enemyCount = Range(numberOfEnemies - 1, numberOfEnemies + 2);
            enemyGroupList.Add(enemyGroup);
        }
        WaveRandomizer(enemyGroupList);
        StartCoroutine(SpawnWave());
    }

    private void WaveRandomizer(List<EnemyGroup> enemyGroups)
    {
        var wave = new Waves();
        wave.enemies = enemyGroups;
        SetEntrance(wave);
        waves.Add(wave);
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

    private IEnumerator SpawnWave()
    {
        foreach (var wave in waves)
        {
            foreach (var enemyGroup in wave.enemies)
            {
                var enemyToSpawn = enemyGroup;
                for (int i = 0; i < enemyToSpawn.enemyCount; i++)
                {
                    Debug.Log(enemyToSpawn.enemyType.enemyName);
                    PoolOfObject.Instance.SpawnFromPool(enemyToSpawn.enemyPoolType, entrances[wave.entrance].position, Quaternion.identity);
                    yield return new WaitForSeconds(enemySpawnDelay);
                }
            }
        }
    }

    private IEnumerator TimeBetweenWaves()
    {
        yield return new WaitForSeconds(waveTimer);
    }
}