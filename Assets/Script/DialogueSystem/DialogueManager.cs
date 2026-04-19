using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public DialogueData dialogue1;


    public GameObject dialogueUI;
    public TextMeshProUGUI dialogueText;
    public Image characterImage;

    public float typingSpeed = 0.03f;

    private string[] lines;
    private int currentLine;
    private bool isTyping;
    private bool canContinue;

    public System.Action onDialogueEnd;
    private System.Action onDialogueEndCallback;

    void Update()
    {
        if (dialogueUI.activeSelf && canContinue && Input.GetMouseButtonDown(0))
        {
            NextLine();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            StartDialogue(dialogue1);
        }
    }

    public void StartDialogue(DialogueData dialogue, System.Action onComplete = null)
    {
        onDialogueEndCallback = onComplete;

        dialogueUI.SetActive(true);

        lines = dialogue.lines;
        characterImage.sprite = dialogue.characterIcon;

        currentLine = 0;

        Time.timeScale = 0f;

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
            yield return new WaitForSecondsRealtime(typingSpeed);
        }

        isTyping = false;

        yield return new WaitForSecondsRealtime(1f); // delay before next

        canContinue = true;

        // auto advance
        StartCoroutine(AutoAdvance());
    }

    IEnumerator AutoAdvance()
    {
        yield return new WaitForSecondsRealtime(2f);

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
        dialogueUI.SetActive(false);

        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        onDialogueEndCallback?.Invoke();
    }
}