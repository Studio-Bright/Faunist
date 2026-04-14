using UnityEngine;

public class GameManager : MonoBehaviour
{
    public DayManager dayManager;
    public AnimalEncounterManager encounterManager;

    void Start()
    {
        var today = dayManager.GetCurrentDay();
        encounterManager.StartDay(today);
    }
}