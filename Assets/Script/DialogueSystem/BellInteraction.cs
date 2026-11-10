using UnityEngine;
using System.Collections;

public class BellInteraction : PickupItem
{
    [Header("Dialogue")]
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private DialogueData introDialogue;

    [Header("Bell Timing")]
    [SerializeField] private float delayBeforeRing = 10f;

    [Header("Player")]
    [SerializeField] private PlayerMovementCC playerMovement;

    [Header("Day")]
    [SerializeField] private DayManager dayManager;
    [SerializeField] private AnimalEncounterManager encounterManager;

    [Header("Bell")]
    [SerializeField] private BellAnimation bellAnimation;
    [SerializeField] private BellCall bellCall;

    private bool canUse = false;
    private bool hasPlayed = false;
    private bool hasStartedDay = false;

    public bool isRinging = false;

    private bool introCompleted = false;

    private Quaternion originalRotation;

    public System.Action onBellRung;


    // =====================================================
    // INITIALIZATION
    // =====================================================

    private void Start()
    {
        originalRotation = transform.rotation;

        if (playerMovement != null)
        {
            playerMovement.SetPreBellState();
        }

        StartCoroutine(RingAfterDelay());
    }


    // =====================================================
    // DELAYED BELL
    // =====================================================

    private IEnumerator RingAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeRing);

        RingBell();
    }


    private void RingBell()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX("BellRinging");
        }

        isRinging = true;
        canUse = true;

        if (bellCall != null)
        {
            bellCall.TurnOnBellCanvas();
        }

        if (bellAnimation != null)
        {
            bellAnimation.PlayRing();
        }

        if (playerMovement != null)
        {
            playerMovement.SetNormalState();
        }

        Debug.Log("Bell is ready.");
    }


    // =====================================================
    // PICKUP
    // =====================================================

    public override void OnPickup()
    {
        base.OnPickup();

        isRinging = false;

        if (bellAnimation != null)
        {
            bellAnimation.StopRing();
        }
    }


    // =====================================================
    // DROP
    // =====================================================

    public override void OnDrop(Vector3 position)
    {
        base.OnDrop(position);

        transform.rotation = originalRotation;

        if (bellAnimation != null)
        {
            bellAnimation.EnableAnimator();
            bellAnimation.PlayRing();
        }
    }


    // =====================================================
    // UNIVERSAL INTERACTION
    // =====================================================

    public override void Interact(PlayerInteractionContext context)
    {
        base.Interact(context);
    }


    public override void AlternateInteract(PlayerInteractionContext context)
    {
        UseBell();
    }


    // =====================================================
    // BELL USE
    // =====================================================

    private void UseBell()
    {
        if (bellCall != null)
        {
            bellCall.TurnOffBellCanvas();
        }

        if (!canUse || hasPlayed)
            return;

        hasPlayed = true;


        if (!introCompleted && introDialogue != null)
        {
            dialogueManager.StartDialogue(introDialogue, HandleBellLogic);
        }
        else
        {
            HandleBellLogic();
        }
    }


    // =====================================================
    // BELL LOGIC
    // =====================================================

    private void HandleBellLogic()
    {
        if (!hasStartedDay)
        {
            hasStartedDay = true;

            introCompleted = true;

            var day = dayManager.GetCurrentDay();

            encounterManager.StartDay(day);

            Debug.Log("Day started!");
        }
        else
        {
            onBellRung?.Invoke();
        }
    }


    // =====================================================
    // RESET
    // =====================================================

    public void ResetBell()
    {
        hasPlayed = false;
        isRinging = false;
    }
}