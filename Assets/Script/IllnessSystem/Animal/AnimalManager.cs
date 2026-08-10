using System.Collections.Generic;
using UnityEngine;

public class AnimalManager : MonoBehaviour
{
    [SerializeField] private List<AnimalSymptomHandler> animals;

    [SerializeField] private IllnessGenerator illnessGenerator;

    public void AssignIllnesses()
    {
        List<IllnessData> illnesses =
            illnessGenerator.GeneratedIllnesses;

        foreach (var animal in animals)
        {
            IllnessData illness =
                illnesses[Random.Range(0, illnesses.Count)];

            animal.ApplyIllness(illness);
        }
    }
}