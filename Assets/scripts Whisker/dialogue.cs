using UnityEngine;
using System.Collections;
using TMPro;

public class dialogue : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public string[] lines;
    public float textSpeed = 0.05f;
    [Tooltip("How many seconds dialogue stays on screen after finishing typing before closing automatically.")]
    public float autoCloseDelay = 3f;

    private int index;
    private Coroutine typingCoroutine;
    private Coroutine autoCloseCoroutine;

    void OnEnable()
    {
        // Automatically start dialogue when this GameObject becomes active
        if (lines != null && lines.Length > 0)
        {
            startDialogue();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            // If still typing, complete current line instantly
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
                textComponent.text = lines[index];
                
                // Reset auto-close timer after skipping typing
                StartAutoCloseTimer();
            }
            // If done typing, move to next line
            else
            {
                nextLine();
            }
        }
    }

    public void startDialogue()
    {
        // Cancel all active typing/closing routines before starting fresh
        StopAllCoroutines();
        
        index = 0;
        textComponent.text = string.Empty;
        gameObject.SetActive(true);
        
        typingCoroutine = StartCoroutine(typeLine());
    }

    IEnumerator typeLine()
    {
        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        typingCoroutine = null;
        StartAutoCloseTimer();
    }

    void nextLine()
    {
        if (autoCloseCoroutine != null) StopCoroutine(autoCloseCoroutine);

        if (index < lines.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            typingCoroutine = StartCoroutine(typeLine());
        }
        else
        {
            CloseDialogue();
        }
    }

    private void StartAutoCloseTimer()
    {
        if (autoCloseCoroutine != null) StopCoroutine(autoCloseCoroutine);
        autoCloseCoroutine = StartCoroutine(AutoCloseRoutine());
    }

    IEnumerator AutoCloseRoutine()
    {
        yield return new WaitForSeconds(autoCloseDelay);
        CloseDialogue();
    }

    public void CloseDialogue()
    {
        StopAllCoroutines();
        textComponent.text = string.Empty;
        gameObject.SetActive(false);
    }
}