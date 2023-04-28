using UnityEngine;

[CreateAssetMenu]
public class Prompt : ScriptableObject
{
    public string promptName;
    [TextArea] public string promptText;
    [Space] 
    public bool queueImmediately;
    public bool canBeFinishedEarly;
    public bool fadeOnInaction;
    public int day = 1;
}