using System.Collections.Generic;
using UnityEngine;

public class BaseManager : MonoBehaviour
{
    public List<BaseElementManager> allBaseElements = new List<BaseElementManager>();
    public Material colorVariantMaterial;
    public Color[] baseElementColor;
    public Color baseHeartColor;
    public Color disabledColor;
    public int reparationPoint;
    
    [Header("GUI")] 
    public Gradient baseLifeGradient;
    public Vector3 baseElementInfoOffset;
}
