using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helper
{
    public static void ModifyPositionY(this Transform tr, float value)
    {
        tr.position = new Vector3(tr.position.x, value, tr.position.z);
    }
}
