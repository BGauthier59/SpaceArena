using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReparationIconData : MonoBehaviour
{
    [SerializeField] private GameObject iconObject;
    [SerializeField] private GameObject iconBottom;

    public void EnableReparationIcon()
    {
        iconBottom.SetActive(true);
        iconObject.SetActive(true);
    }
    
    public void DisableReparationIcon()
    {
        iconBottom.SetActive(false);
        iconObject.SetActive(false);
    }
}
