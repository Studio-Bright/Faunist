using UnityEngine;
using System.Collections.Generic;

public class DayManager : MonoBehaviour
{
    public List<DayData> days;

    private int currentDayIndex = 0;

    public DayData GetCurrentDay()
    {
        if (currentDayIndex >= days.Count)
        {
            Debug.Log("No more days!");
            return null;
        }

        return days[currentDayIndex];
    }

    public bool HasNextDay()
    {
        return currentDayIndex + 1 < days.Count;
    }

    public void GoToNextDay()
    {
        currentDayIndex++;
    }


}