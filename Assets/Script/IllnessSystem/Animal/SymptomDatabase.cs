using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Vet Sim/Symptom Database")]
public class SymptomDatabase : ScriptableObject
{
    public List<SymptomData> symptoms;
}