using UnityEngine;

[System.Serializable]
public class AnimalData
{
    public GameObject prefab;
    public float healTime;

    public Vector3 spawnRotation;

    public DialogueData postHealDialogue;
    public DialogueData[] failDialogues; 
    public float stayAfterHeal = 3f;
    public float delayBeforeNext = 2f;

   
}