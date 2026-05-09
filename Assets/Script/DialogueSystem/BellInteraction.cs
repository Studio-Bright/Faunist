using UnityEngine;
using System.Collections;

public class BellInteraction : MonoBehaviour, IInteractable
{
    public DialogueManager dialogueManager;
    public DialogueData introDialogue;     

    public AudioSource bellAudio;

    public float delayBeforeRing = 10f;

    public PlayerMovementCC playerMovement;

    public DayManager dayManager;
    public AnimalEncounterManager encounterManager;

    private bool canInteract = false;
    private bool hasPlayed = false;
    private bool hasStartedDay = false;

    public System.Action onBellRung;

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
        

        DialogueData dialogueToPlay = null;

        if (!hasStartedDay)
        {
            dialogueToPlay = introDialogue;
        }
       

        if (dialogueToPlay == null)
        {
            HandleBellLogic(player);
            return;
        }

        dialogueManager.StartDialogue(dialogueToPlay, () =>
        {
            HandleBellLogic(player);
        });
    }
    
   

    public void ResetBell()
    {
        hasPlayed = false;
        canInteract = true; 
    }

    void HandleBellLogic(PlayerInteraction player)
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