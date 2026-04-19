using UnityEngine;
using System.Collections.Generic;

public class AnimalEncounterManager : MonoBehaviour
{
    public Transform spawnPoint;
    public Timer timer;

    public Queue<AnimalData> animalQueue;
    public AnimalData currentAnimal;
    private GameObject currentInstance;
    public DayManager dayManager;

    private bool canHeal = false;

    public void StartDay(DayData dayData)
    {
        animalQueue = new Queue<AnimalData>(dayData.animals);
        SpawnNextAnimal();
    }


    void SpawnNextAnimal()
    {
        if (animalQueue.Count == 0)
        {
            Debug.Log("Day complete!");

            StartNextDay(); 
            return;
        }

        currentAnimal = animalQueue.Dequeue();

        currentInstance = Instantiate(
    currentAnimal.prefab,
    spawnPoint.position,
    Quaternion.Euler(currentAnimal.spawnRotation)
);

        canHeal = true;

        timer.StartTimer(currentAnimal.healTime);
    }

    void Update()
    {
        // 🔑 press N to simulate healing
        if (canHeal && Input.GetKeyDown(KeyCode.N))
        {
            Debug.Log("Pressed N → Animal healed manually");
            OnAnimalHealed();
        }
    }

    public void OnAnimalHealed()
    {
        canHeal = false;

        timer.StopTimer();

        if (currentInstance != null)
            Destroy(currentInstance);

        SpawnNextAnimal();
    }

    void StartNextDay()
    {
        if (dayManager.HasNextDay())
        {
            dayManager.GoToNextDay();

            Debug.Log("Starting next day...");

            var nextDay = dayManager.GetCurrentDay();
            StartDay(nextDay);
        }
        else
        {
            Debug.Log("Game complete! 🎉");

           
        }
    }
}