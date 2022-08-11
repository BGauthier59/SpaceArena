using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NearbyBaseElement : MonoBehaviour
{
    [SerializeField] private GameObject elementName;

    private void Start()
    {
        elementName.SetActive(false);
    }

    #region Trigger & Collision

    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !elementName.activeSelf)
        {
            elementName.SetActive(true);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && elementName.activeSelf)
        {
            elementName.SetActive(false);
        }
    }

    #endregion
}
