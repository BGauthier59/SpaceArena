using System;
using System.Collections.Generic;
using UnityEngine;

public class PoolOfObject : MonoBehaviour
{
    [Serializable]
    public class Pool
    {
        public PoolType type;
        public GameObject prefab;
        public int size;
    }

    public static PoolOfObject Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            DestroyImmediate(gameObject);
            return;
        }

        Instance = this;
        
        Initialization();
    }

    public List<Pool> pools;
    public Dictionary<PoolType, Queue<GameObject>> poolDictionary;

    private void Initialization()
    {
        poolDictionary = new Dictionary<PoolType, Queue<GameObject>>();

        foreach (var pool in pools)
        {
            var objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                var obj = Instantiate(pool.prefab, transform);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.type, objectPool);
        }
    }

    public GameObject SpawnFromPool(PoolType type, Vector3 position, Quaternion rotation)
    {
        var objToSpawn = poolDictionary[type].Dequeue();
        objToSpawn.transform.position = position;
        objToSpawn.transform.rotation = rotation;
        objToSpawn.SetActive(true);

        poolDictionary[type].Enqueue(objToSpawn);

        return objToSpawn;
    }
}

public enum PoolType
{
    Bullet, Bullet_Impact, Damage, EnemyDeath, Alien, Hunter, Breaker, HeartBreaker, HunterProjectile, Grenade, Explosion, ControllableTurretProjectile, splashVFX
}