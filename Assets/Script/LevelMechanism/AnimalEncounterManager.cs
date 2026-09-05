using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public Transform VFXPoint;
    public Timer timer;
    public DialogueManager dialogueManager;
    public BellInteraction bell;
    public DayManager dayManager;
    public VFXSpawner vfxSpawner;

    public Queue<AnimalData> animalQueue;
    public AnimalData currentAnimal;
    private GameObject currentInstance;
    public LightingStateManager lightingStateManager;
    private EncounterState state;
    [Header("Day End UI")]
    public GameObject dayEndCanvas;
    private bool dayStarted = false;

    public ReputationUI reputationUI;
    public float reputationBonus = 0.052f;
    public bool isNight;
    [Header("Multi-Heal Settings")]
    private int currentHeals = 0;
    private int requiredHeals = 1;
    private bool nightTriggered = false;
    void Start()
    {
        reputationUI.Initialize(0f);
    }

    public void StartDay(DayData dayData)
    {

        if (dayStarted) return;

        dayStarted = true;

        bool isDayOne = dayManager.GetCurrentDay() == dayData && dayManager.HasNextDay() == true && dayManager.GetCurrentDay() == dayManager.days[0];

        animalQueue = new Queue<AnimalData>(dayData.animals);

        SpawnNextAnimal();
    }
    void Explode()
    {
        AudioManager.Instance.PlaySFX("MagicalPuff");
        vfxSpawner.PlayBoom(VFXPoint.position);
    }

    void SpawnNextAnimal()
    {
        if (animalQueue.Count == 0)
        {
            StartNextDay();
            return;
        }

        currentAnimal = animalQueue.Dequeue();

        StartCoroutine(SpawnWithVFX());
    }
    IEnumerator SpawnWithVFX()
    {
        currentHeals = 0;
        requiredHeals = currentAnimal.requiredHeals;

        if (!nightTriggered && currentAnimal.isSnailLevel)
        {
            nightTriggered = true;
            lightingStateManager?.SwitchToNight();
        }

        Explode();

        yield return new WaitForSeconds(1f);

        currentInstance = Instantiate(
            currentAnimal.prefab,
            spawnPoint.position,
            Quaternion.Euler(currentAnimal.spawnRotation)
        );

        state = EncounterState.WaitingForHeal;

        reputationUI.StartDecay(reputationBonus, currentAnimal.healTime);

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
        if (state != EncounterState.WaitingForHeal)
            return;

        currentHeals++;

        Debug.Log($"Healed {currentHeals}/{requiredHeals}");

        Explode();

        if (currentHeals < requiredHeals)
        {
            // NOT finished yet
            return;
        }

        // Fully healed
        timer.StopTimer();
        timer.OnTimerFinished -= OnFail;

        reputationUI.CommitCurrentToPermanent();

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

            dayStarted = false; 

            var nextDay = dayManager.GetCurrentDay();
            StartDay(nextDay);
        }
        else
        {
            if (dayEndCanvas != null)
                dayEndCanvas.SetActive(true);
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            Debug.Log("Game complete! 🎉");
        }
    }
   
    IEnumerator PostHealFlow()
    {
            state = EncounterState.PostHeal;

            yield return new WaitForSecondsRealtime(currentAnimal.stayAfterHeal);
            DestroyAnimal();
            bell.ResetBell();

            // Wait 3 seconds after the animal is healed before continuing
            yield return new WaitForSecondsRealtime(3f);

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
        DestroyAnimal();

        yield return StartCoroutine(WaitForBell());

       

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

        int index = UnityEngine.Random.Range(0, currentAnimal.failDialogues.Length);
        return currentAnimal.failDialogues[index];
    }

    void DestroyAnimal()
    {
        if (currentInstance != null)
            Destroy(currentInstance);
    }


}