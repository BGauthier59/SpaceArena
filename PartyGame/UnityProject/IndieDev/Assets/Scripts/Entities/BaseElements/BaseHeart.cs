using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseHeart : BaseElementManager
{
    
    public override void OnDestroyed()
    {
        base.OnDestroyed();
        
        Debug.Log("End of party");
    }

    public override void OnFixed()
    {
        // Can't be fixed...
    }
}
