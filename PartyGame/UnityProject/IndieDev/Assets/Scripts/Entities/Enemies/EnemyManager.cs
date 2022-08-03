using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyManager : Entity
{
    #region Entity

    public override void TakeDamage(int damage)
    {
        var damageIndicator = PoolOfObject.Instance.SpawnFromPool(PoolType.Damage, transform.position, Quaternion.identity)
            .GetComponent<TextMeshProUGUI>();
        damageIndicator.rectTransform.SetParent(GameManager.instance.mainCanvas.transform);
        damageIndicator.transform.position = Camera.main.WorldToScreenPoint(transform.position);
        damageIndicator.text = damage.ToString();
        animator.Play("EnemyHit");
        base.TakeDamage(damage);
    }

    public override void Death()
    {
        base.Death();
        gameObject.SetActive(false);
        PoolOfObject.Instance.SpawnFromPool(PoolType.EnemyDeath, transform.position, Quaternion.identity);
    }

    public override void Heal(int heal)
    {
        base.Heal(heal);
    }

    #endregion

    #region Trigger & Collision

    private void OnTriggerEnter(Collider other)
    {
    }

    #endregion
    
}