using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue System/Dialogue")]
public class DialogueData : ScriptableObject
{
    [TextArea(2, 5)]
    public string[] lines;

    public Sprite characterIcon;

    public float autoAdvanceDelay = 2f;
}