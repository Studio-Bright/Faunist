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

        if (canContinue && Input.GetMouseButtonDown(0))
        {
            NextLine();
        }
    }

    public void StartDialogue(DialogueData dialogue, System.Action onComplete = null)
    {
        if (isDialogueActive) return; // prevent double triggering

        isDialogueActive = true;
        onDialogueEndCallback = onComplete;

        dialogueUI.SetActive(true);

        lines = dialogue.lines;
        characterImage.sprite = dialogue.characterIcon;

        currentLine = 0;

        // 🔥 DO NOT PAUSE GAME
        // Time.timeScale = 0f;

        // 🔥 Slow player instead
        if (playerMovement != null)
            playerMovement.SetPreBellState();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

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

        yield return new WaitForSeconds(1f);

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

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        isDialogueActive = false;

        onDialogueEndCallback?.Invoke();
    }
}