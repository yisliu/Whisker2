

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using TMPro;


public class customerDialogue : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static customerDialogue Instance;
    [Header("UI References")] 
    public TextMeshProUGUI textComponent;

    public TextMeshProUGUI speakerNameText;
    public Image speakerPortrait;
    public GameObject dialoguePanel;

    [Header("Player")] 
    public string playerName = "Player";
    public Sprite playerPortrait;
    [Header("Settings")] 
    public float textSpeed = 0.05f;

    public System.Action onDialogueComplete;

    private string[] currentLines;
    private int lineIndex;
    private string remainingText;
    private string currentSegment;
    private bool isTyping;
    private bool waitingForInput;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
    }

    void Update()
    {
        if (dialoguePanel == null || !dialoguePanel.activeSelf)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (isTyping)
            {
                StopAllCoroutines();
                isTyping = false;
                FitTextToBox(currentSegment);
            }
            else if (waitingForInput)
            {
                waitingForInput = false;
                if (remainingText.Length > 0)
                {
                    textComponent.text = string.Empty;
                    StartCoroutine(TypeText(remainingText));
                }
                else
                {
                    NextLine();
                }
            }
        }
    }

    public void StartDialogue(string[] lines, string speaker, Sprite portrait)
    {
        if (lines == null || lines.Length == 0)
        {
            return;
        }
        
        StopAllCoroutines();
        currentLines = lines;
        lineIndex = 0;
        isTyping = false;
        waitingForInput = false;
        if (speakerNameText != null)
        {
            speakerNameText.text = speaker;
        }

        if (speakerPortrait != null)
        {
            speakerPortrait.sprite = portrait;
            speakerPortrait.gameObject.SetActive(portrait != null);
        }
        dialoguePanel.SetActive(true);
        textComponent.text = string.Empty;
        StartCoroutine(TypeText(currentLines[lineIndex]));
    }

    public void StartPlayerDialogue(string[] lines)
    {
        StartDialogue(lines, playerName, playerPortrait);
    }

    IEnumerator TypeText(string text)
    {
        currentSegment = text;
        isTyping = true;
        remainingText = string.Empty;
        string typed = string.Empty;
        for (int i = 0; i < text.Length; i++)
        {
            typed += text[i];
            textComponent.text = typed;
            textComponent.ForceMeshUpdate();
            if (textComponent.isTextOverflowing)
            {
                typed = typed.Substring(0, typed.Length - 1);
                textComponent.text = typed;
                remainingText = text.Substring(i);
                isTyping = false;
                waitingForInput = true;
                yield break;
            }
            yield return new WaitForSecondsRealtime(textSpeed);
        }
        isTyping = false;
        remainingText = string.Empty;
        waitingForInput = true;
    }

    void FitTextToBox(string text)
    {
        string fitted = string.Empty;
        textComponent.text = string.Empty;
        foreach (char c in text)
        {
            textComponent.text += c;
            textComponent.ForceMeshUpdate();
            if (textComponent.isTextOverflowing)
            {
                textComponent.text = fitted;
                remainingText = text.Substring(fitted.Length);
                waitingForInput = true;
                return;
            }
            fitted = textComponent.text;
        }

        remainingText = string.Empty;
        waitingForInput = true;
    }

    void NextLine()
    {
        if (lineIndex < currentLines.Length - 1)
        {
            lineIndex++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeText(currentLines[lineIndex]));
        }
        else
        {
            Close();
        }
    }

    void Close()
    {
        dialoguePanel.SetActive(false);
        System.Action callback = onDialogueComplete;
        onDialogueComplete = null;
        callback?.Invoke();
    }
}
