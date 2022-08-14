using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseManager : MonoBehaviour
{
    #region Instance

    public static BaseManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            DestroyImmediate(gameObject);
            return;
        }

        instance = this;
    }

    #endregion

    public List<BaseElementManager> allBaseElements = new List<BaseElementManager>();

    [Header("GUI")] 
    public Gradient baseLifeGradient;
    public Vector3 baseElementInfoOffset;
    
    private void Start()
    {
        
    }
}
