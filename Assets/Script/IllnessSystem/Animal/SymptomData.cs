using UnityEngine;

[CreateAssetMenu(menuName = "Vet Sim/Symptom")]
public class SymptomData : ScriptableObject
{
    public string symptomName;

    [Range(1, 100)]
    public int rarityWeight = 50;
}