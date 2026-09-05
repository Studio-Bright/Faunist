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
    public BellAnimation bellAnimation;

    private bool canUse = false;
    private bool hasPlayed = false;
    private bool hasStartedDay = false;
    public bool isRinging = false;
    private bool introCompleted = false;

    private Quaternion originalRotation;

    public System.Action onBellRung;

    public BellCall bellCall;
    void Start()
    {
        originalRotation = transform.rotation;

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

        isRinging = true;
        canUse = true;

        bellCall.TurnOnBellCanvas();

        bellAnimation.PlayRing();

        playerMovement.SetNormalState();

        Debug.Log("Bell is ready.");
    }
    public override void OnPickup()
    {
        base.OnPickup();

        isRinging = false;

        bellAnimation.StopRing();
    }
    public override void OnDrop(Vector3 position)
    {
        base.OnDrop(position);

        transform.rotation = originalRotation;

        bellAnimation.EnableAnimator();
        bellAnimation.PlayRing();
    }

    public override void Use(PlayerInteraction player)
    {
        bellCall.TurnOffBellCanvas();
        if (!canUse || hasPlayed)
            return;

        hasPlayed = true;

        // Only play intro dialogue once
        if (!introCompleted && introDialogue != null)
        {
            dialogueManager.StartDialogue(introDialogue, () =>
            {
                HandleBellLogic();
                
            });
        }
        else
        {
            HandleBellLogic();
        }
    }
    public void ResetBell()
    {
        hasPlayed = false;
        isRinging = false;

        // ❌ do NOT reset introCompleted
        // introCompleted stays locked for whole run
    }

    void HandleBellLogic()
    {
        if (!hasStartedDay)
        {
            hasStartedDay = true;
            introCompleted = true; // permanently mark intro as played

            var day = dayManager.GetCurrentDay();
            encounterManager.StartDay(day);

            Debug.Log("Day started!");
        }
        else
        {
            onBellRung?.Invoke();
        }
    }
}