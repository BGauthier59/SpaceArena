using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesManager : MonoBehaviour
{
    public static EnemiesManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            DestroyImmediate(this);
            return;
        }

        instance = this;
        allEnemies.Clear();
    }

    public List<EnemyManager> allEnemies = new List<EnemyManager>();

    public void AddEnemy(EnemyManager enemy)
    {
        if (!allEnemies.Contains(enemy)) allEnemies.Add(enemy);
    }

    public void RemoveEnemy(EnemyManager enemy)
    {
        if (allEnemies.Contains(enemy)) allEnemies.Remove(enemy);
    }

    public void DeactivateAllEnemies()
    {
        foreach (var enemy in allEnemies)
        {
            enemy.behaviour.enabled = false;
        }
    }
}