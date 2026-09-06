
using UnityEngine;
using UnityEngine.SceneManagement;
public class dialogueChoices : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static dialogueChoices Instance { get; private set; }
    [SerializeField] private GameObject choicePanel;
    [SerializeField] private string nextScene;
    [SerializeField] private string[] triggerLines;
    [SerializeField] private string speakerName;
    [SerializeField] private Sprite speakerPortrait;

    [SerializeField] private bool[] triggerIsPlayer;

    void Awake()
    {
        Instance = this;
        choicePanel.SetActive(false);
    }

    void Start()
    {
        if (customerDialogue.Instance != null && triggerLines.Length > 0)
        {
            customerDialogue.Instance.onDialogueComplete = () => choicePanel.SetActive(true);
            customerDialogue.Instance.StartDialogue(triggerLines, triggerIsPlayer, speakerName, speakerPortrait);
        }
    }

    public void OptionChosen(string[] responseLines)
    {
        choicePanel.SetActive(false);
        if (customerDialogue.Instance != null)
        {
            customerDialogue.Instance.onDialogueComplete = () => SceneManager.LoadScene(nextScene);
            customerDialogue.Instance.StartDialogue(responseLines, triggerIsPlayer, speakerName, speakerPortrait);
        }
        else
        {
            SceneManager.LoadScene(nextScene);
        }
    }
}