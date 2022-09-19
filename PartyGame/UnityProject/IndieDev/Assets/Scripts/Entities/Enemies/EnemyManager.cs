using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyManager : Entity
{
    #region Entity

    public override void TakeDamage(int damage)
    {
        /* Pour l'instant : non
        var damageIndicator = PoolOfObject.Instance.SpawnFromPool(PoolType.Damage, transform.position, Quaternion.identity)
            .GetComponent<TextMeshProUGUI>();
        damageIndicator.rectTransform.SetParent(GameManager.instance.mainCanvas.transform);
        damageIndicator.transform.position = Camera.main.WorldToScreenPoint(transform.position);
        damageIndicator.text = damage.ToString();
        animator.Play("EnemyHit");
        */
        
        Debug.Log("Enemy hit");
        base.TakeDamage(damage);
    }

    public override void Death()
    {
        base.Death();
        gameObject.SetActive(false);
        
        // Pour l'instant : non
        //PoolOfObject.Instance.SpawnFromPool(PoolType.EnemyDeath, transform.position, Quaternion.identity);
    }
    
    public override void Heal(int heal)
    {
        base.Heal(heal);
    }

    public override void Start()
    {
        base.Start();
        
        if (EnemiesManager.instance == null) return;
        EnemiesManager.instance.AddEnemy(this);
    }

    #endregion

    #region Trigger & Collision

    private void OnTriggerEnter(Collider other)
    {
    }

    #endregion

    private void OnEnable()
    {
        if (EnemiesManager.instance == null) return;
        EnemiesManager.instance.AddEnemy(this);
    }

    private void OnDisable()
    {
        if (EnemiesManager.instance == null) return;
        EnemiesManager.instance.RemoveEnemy(this);
    }
}