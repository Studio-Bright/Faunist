using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IllnessGenerator : MonoBehaviour
{
    [SerializeField] private int illnessCount = 5;

    [SerializeField] private SymptomDatabase symptomDatabase;
    [SerializeField] private SymptomCompatibilityMatrix compatibilityMatrix;
    [SerializeField] private IllnessUI illnessUI;

    public AnimalManager animalManager;

    private IllnessDatabase database = new();

    private void Start()
    {
        GenerateIllnesses();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            GenerateIllnesses();
        }
    }
    public void GenerateIllnesses()
    {
        database.illnesses.Clear();

        int safety = 1000;

        while (database.illnesses.Count < illnessCount && safety-- > 0)
        {
            IllnessData illness = GenerateSingleIllness();

            if (!IsDuplicate(illness))
            {
                illness.illnessName = $"Illness {database.illnesses.Count + 1}";
                database.illnesses.Add(illness);
            }
        }
        IllnessSaveSystem.Save(database);

        if (illnessUI != null)
        {
            illnessUI.Show(database);
        }
        IllnessSaveSystem.Save(database);

        animalManager.AssignIllnesses();
    }

    private IllnessData GenerateSingleIllness()
    {
        IllnessData illness = new IllnessData();

        int symptomAmount = Random.Range(1, 4);

        List<SymptomData> selectedSymptoms = new();

        int attempts = 100;

        while (selectedSymptoms.Count < symptomAmount && attempts-- > 0)
        {
            SymptomData candidate = GetWeightedRandomSymptom();

            if (selectedSymptoms.Contains(candidate))
                continue;

            if (AreCompatible(candidate, selectedSymptoms))
            {
                selectedSymptoms.Add(candidate);
            }
        }

        illness.symptoms = selectedSymptoms;

        return illness;
    }

    private bool AreCompatible(
    SymptomData candidate,
    List<SymptomData> existingSymptoms)
    {
        int candidateIndex =
            symptomDatabase.symptoms.IndexOf(candidate);

        foreach (var existing in existingSymptoms)
        {
            int existingIndex =
                symptomDatabase.symptoms.IndexOf(existing);

            if (!compatibilityMatrix.IsCompatible(
                    candidateIndex,
                    existingIndex))
            {
                return false;
            }
        }

        return true;
    }

    private SymptomData GetWeightedRandomSymptom()
    {
        int totalWeight = symptomDatabase.symptoms.Sum(x => x.rarityWeight);

        int roll = Random.Range(0, totalWeight);

        foreach (var symptom in symptomDatabase.symptoms)
        {
            if (roll < symptom.rarityWeight)
                return symptom;

            roll -= symptom.rarityWeight;
        }

        return symptomDatabase.symptoms[0];
    }

    private bool IsDuplicate(IllnessData newIllness)
    {
        foreach (var existing in database.illnesses)
        {
            if (AreSymptomsIdentical(existing, newIllness))
                return true;
        }

        return false;
    }

    private bool AreSymptomsIdentical(IllnessData a, IllnessData b)
    {
        if (a.symptoms.Count != b.symptoms.Count)
            return false;

        var namesA = a.symptoms
            .Select(x => x.symptomName)
            .OrderBy(x => x);

        var namesB = b.symptoms
            .Select(x => x.symptomName)
            .OrderBy(x => x);

        return namesA.SequenceEqual(namesB);
    }

    public List<IllnessData> GeneratedIllnesses
    {
        get { return database.illnesses; }
    }
}