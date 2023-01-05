using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Arena", menuName = "Arena", order = 4)]
public class ArenaScriptable : ScriptableObject
{
    public MainMenuManager.TranslatableName translatableName;
    public int arenaSceneIndex;
}
