using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TrapdoorDevice : BaseElementManager
{
    [SerializeField] private Trapdoor[] allTrapdoors;

    [SerializeField] private AnimationClip openingAnim;
    [SerializeField] private AnimationClip closingAnim;
    
    [Serializable]
    public class Trapdoor
    {
        public Animation trapAnimation;
        public Renderer[] colorElements;
    }

    public override void Start()
    {
        base.Start();

        foreach (var t in allTrapdoors)
        {
            foreach (var rd in t.colorElements)
            {
                rd.material.SetColor("_EmissionColor", color * 2);
            }
        }
    }

    public override void TakeDamage(int damage, Entity attacker = null)
    {
        base.TakeDamage(damage, attacker);
    }

    protected override void OnDestroyed()
    {
        base.OnDestroyed();
        
        foreach (var td in allTrapdoors)
        {
            td.trapAnimation.Play(openingAnim.name);
        }
        
    }

    protected override void OnFixed()
    {
        base.OnFixed();

        foreach (var td in allTrapdoors)
        {
            td.trapAnimation.Play(closingAnim.name);
        }
    }

    public override void Update()
    {
        base.Update();
        
    }
}