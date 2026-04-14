using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DayData", menuName = "Game/Day Data")]
public class DayData : ScriptableObject
{
    public List<AnimalData> animals;
}