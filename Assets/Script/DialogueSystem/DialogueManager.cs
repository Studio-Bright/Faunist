using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject dialogueUI;
    public TextMeshProUGUI dialogueText;
    public Image characterImage;

    [Header("Settings")]
    public float typingSpeed = 0.03f;

    [Header("Player")]
    public PlayerMovementCC playerMovement; // assign in inspector

    private string[] lines;
    private int currentLine;

    private bool isTyping;
    private bool canContinue;
    private bool isDialogueActive;

    private System.Action onDialogueEndCallback;

    void Update()
    {
        if (!isDialogueActive) return;

        if (canContinue)
        {
            NextLine();
        }
    }

    public void StartDialogue(DialogueData dialogue, System.Action onComplete = null)
    {
        if (isDialogueActive) return; 

        isDialogueActive = true;
        onDialogueEndCallback = onComplete;

        dialogueUI.SetActive(true);

        lines = dialogue.lines;
        characterImage.sprite = dialogue.characterIcon;

        currentLine = 0;

        if (playerMovement != null)
            playerMovement.SetPreBellState();

        
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        canContinue = false;

        dialogueText.text = "";

        foreach (char c in lines[currentLine])
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;

        yield return new WaitForSeconds(5f);

        canContinue = true;

        StartCoroutine(AutoAdvance());
    }

    IEnumerator AutoAdvance()
    {
        yield return new WaitForSeconds(2f);

        if (canContinue)
        {
            NextLine();
        }
    }

    public void NextLine()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            dialogueText.text = lines[currentLine];
            isTyping = false;
            canContinue = true;
            return;
        }

        currentLine++;

        if (currentLine < lines.Length)
        {
            StartCoroutine(TypeLine());
        }
        else
        {
            EndDialogue();
        }
    }

    void EndDialogue()
    {
        StopAllCoroutines();

        dialogueUI.SetActive(false);

        // 🔥 Restore player speed
        if (playerMovement != null)
            playerMovement.SetNormalState();

       

        isDialogueActive = false;

        onDialogueEndCallback?.Invoke();
    }
}