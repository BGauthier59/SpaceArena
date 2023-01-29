using UnityEngine;

[CreateAssetMenu(fileName = "New Text", menuName = "Loading Text", order = 5)]
public class LoadingTextScriptable : ScriptableObject
{
    [TextArea(4, 4)] public string frenchText;
    [TextArea(4, 4)] public string englishText;
}
