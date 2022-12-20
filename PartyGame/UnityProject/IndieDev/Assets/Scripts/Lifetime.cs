using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lifetime : MonoBehaviour
{
    [SerializeField] private float lifeSpan;
    void OnEnable()
    {
        StartCoroutine(LifeSpan());
    }

    private IEnumerator LifeSpan()
    {
        yield return new WaitForSeconds(lifeSpan);
        gameObject.SetActive(false);
    }
}
