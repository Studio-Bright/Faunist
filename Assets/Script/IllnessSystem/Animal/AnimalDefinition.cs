using System.Collections.Generic;
using UnityEngine;

public class AnimalDefinition : MonoBehaviour
{
    public List<AnimalSymptomSetup> symptomSetups;

    public AnimalSymptomSetup GetSetup(SymptomData symptom)
    {
        return symptomSetups.Find(x => x.symptom == symptom);
    }
}