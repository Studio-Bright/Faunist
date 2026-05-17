using UnityEngine;
using System.Collections;

public class BellInteraction : PickupItem
{
    public DialogueManager dialogueManager;
    public DialogueData introDialogue;

    public float delayBeforeRing = 10f;

    public PlayerMovementCC playerMovement;

    public DayManager dayManager;
    public AnimalEncounterManager encounterManager;

    private bool canUse = false;
    private bool hasPlayed = false;
    private bool hasStartedDay = false;

    public System.Action onBellRung;

    void Start()
    {
        playerMovement.SetPreBellState();
        StartCoroutine(RingAfterDelay());
    }

    IEnumerator RingAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeRing);

        RingBell();
    }

    void RingBell()
    {
        AudioManager.Instance.PlaySFX("BellRinging");

        canUse = true;

        playerMovement.SetNormalState();

        Debug.Log("Bell is ready.");
    }

    public override void Use(PlayerInteraction player)
    {
        if (!canUse || hasPlayed)
            return;

        hasPlayed = true;

        DialogueData dialogueToPlay = null;

        if (!hasStartedDay)
        {
            dialogueToPlay = introDialogue;
        }

        if (dialogueToPlay == null)
        {
            HandleBellLogic();
            return;
        }

        dialogueManager.StartDialogue(dialogueToPlay, () =>
        {
            HandleBellLogic();
        });
    }

    public void ResetBell()
    {
        hasPlayed = false;
        canUse = true;
    }

    void HandleBellLogic()
    {
        if (!hasStartedDay)
        {
            hasStartedDay = true;

            var day = dayManager.GetCurrentDay();

            encounterManager.StartDay(day);

            Debug.Log("Day started!");
        }
        else
        {
            Debug.Log("Bell rung for progression");

            onBellRung?.Invoke();
        }
    }
}