using UnityEngine;
using System.Collections;

public class BellInteraction : MonoBehaviour, IInteractable
{
    public DialogueManager dialogueManager;
    public DialogueData dialogue;

    public AudioSource bellAudio;

    public float delayBeforeRing = 10f;

    public PlayerMovementCC playerMovement;

    public DayManager dayManager;
    public AnimalEncounterManager encounterManager;

    private bool canInteract = false;
    private bool hasPlayed = false;

    void Awake()
    {
        playerMovement.SetPreBellState(); 
    }

    void Start()
    {
        StartCoroutine(RingAfterDelay());
    }

    void Update()
    {
        if (!canInteract)
        {
            playerMovement.SetPreBellState();
        }
    }
    IEnumerator RingAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeRing);
        RingBell();
    }

    void RingBell()
    {
        if (bellAudio != null)
            bellAudio.Play();

        canInteract = true;
        playerMovement.SetNormalState();
        Debug.Log("Bell is ringing! Player can interact now.");
    }

    public void Interact(PlayerInteraction player)
    {
        if (!canInteract || hasPlayed) return;

        hasPlayed = true;

        player.DisablePlayerControl();

        dialogueManager.StartDialogue(dialogue, () =>
        {
            StartCoroutine(StartDayAfterDelay(player));
        });
    }

    IEnumerator StartDayAfterDelay(PlayerInteraction player)
    {
        yield return new WaitForSeconds(1f);

        player.EnablePlayerControl();

        var day = dayManager.GetCurrentDay();
        encounterManager.StartDay(day);

        Debug.Log("Day started after dialogue.");
    }
}