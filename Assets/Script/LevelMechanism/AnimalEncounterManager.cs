using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class AnimalEncounterManager : MonoBehaviour
{
    public enum EncounterState
    {
        Idle,
        WaitingForHeal,
        PostHeal,
        Failed,
        WaitingForBell,
        ShowingDialogue,
        Transitioning
    }

    public Transform spawnPoint;
    public Timer timer;
    public DialogueManager dialogueManager;
    public BellInteraction bell;
    public DayManager dayManager;

    public Queue<AnimalData> animalQueue;
    public AnimalData currentAnimal;
    private GameObject currentInstance;

    private EncounterState state;

    private bool dayStarted = false;

    public void StartDay(DayData dayData)
    {
        if (dayStarted) return;

        dayStarted = true;
        animalQueue = new Queue<AnimalData>(dayData.animals);

        SpawnNextAnimal();
    }


    void SpawnNextAnimal()
    {
        if (animalQueue.Count == 0)
        {
            StartNextDay();
            return;
        }

        currentAnimal = animalQueue.Dequeue();

        currentInstance = Instantiate(
            currentAnimal.prefab,
            spawnPoint.position,
            Quaternion.Euler(currentAnimal.spawnRotation)
        );

        state = EncounterState.WaitingForHeal;

        timer.OnTimerFinished += OnFail;
        timer.StartTimer(currentAnimal.healTime);
    }

    void Update()
    {
        if (state != EncounterState.WaitingForHeal) return;

        if (Input.GetKeyDown(KeyCode.N))
        {
            Debug.Log("Pressed N → Animal healed manually");
            OnAnimalHealed();
        }
    }

    public void OnAnimalHealed()
    {
        if (state != EncounterState.WaitingForHeal) return;

        timer.StopTimer();
        timer.OnTimerFinished -= OnFail;

        StartCoroutine(PostHealFlow());
    }

    void OnFail()
    {
        if (state != EncounterState.WaitingForHeal) return;

        timer.OnTimerFinished -= OnFail;

        state = EncounterState.Failed;

        StartCoroutine(FailFlow());
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
    IEnumerator PostHealFlow()
    {
        state = EncounterState.PostHeal;

        yield return new WaitForSecondsRealtime(currentAnimal.stayAfterHeal);

        bell.ResetBell();

        state = EncounterState.WaitingForBell;

        yield return StartCoroutine(WaitForBell());

        DestroyAnimal();

        yield return ShowHintDialogue();

        CleanupAndNext();
    }

    IEnumerator FailFlow()
    {
        state = EncounterState.Failed;

        DialogueData failDialogue = GetRandomFailDialogue();

        if (failDialogue != null)
        {
            yield return StartCoroutine(PlayDialogue(failDialogue));
        }

        // 🔻 reputation placeholder
        Debug.Log("REPUTATION LOSS HERE");

        bell.ResetBell();

        state = EncounterState.WaitingForBell;

        yield return StartCoroutine(WaitForBell());

        DestroyAnimal();

        yield return ShowHintDialogue();

        CleanupAndNext(); ;
    }

    IEnumerator PlayDialogue(DialogueData data)
    {
        bool finished = false;

        state = EncounterState.ShowingDialogue;

        dialogueManager.StartDialogue(data, () =>
        {
            finished = true;
        });

        yield return new WaitUntil(() => finished);
    }
    IEnumerator ShowHintDialogue()
    {
        if (currentAnimal.postHealDialogue == null)
            yield break;

        yield return PlayDialogue(currentAnimal.postHealDialogue);
    }

    void CleanupAndNext()
    {
        state = EncounterState.Transitioning;

        StartCoroutine(NextStep());
    }

    IEnumerator NextStep()
    {
        yield return new WaitForSecondsRealtime(currentAnimal.delayBeforeNext);

        SpawnNextAnimal();
    }

    IEnumerator WaitForBell()
    {
        bool rung = false;

        System.Action action = () => rung = true;

        bell.onBellRung += action;

        yield return new WaitUntil(() => rung);

        bell.onBellRung -= action;
    }

    DialogueData GetRandomFailDialogue()
    {
        if (currentAnimal.failDialogues == null || currentAnimal.failDialogues.Length == 0)
            return null;

        int index = Random.Range(0, currentAnimal.failDialogues.Length);
        return currentAnimal.failDialogues[index];
    }

    void DestroyAnimal()
    {
        if (currentInstance != null)
            Destroy(currentInstance);
    }
}