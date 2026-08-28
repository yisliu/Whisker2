using UnityEngine;

public class cabinentOpener : MonoBehaviour
{
    [Header("Sprites")] 
    public SpriteRenderer cabinentRenderer;

    public Sprite openSprite;
    public Sprite closedSprite;

    [Header("Shelves")] 
    public GameObject[] shelves;
    
    [Header("UI")]
    public GameObject promptCanvas;
    private bool playerInRange;

    private bool isOpen;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cabinentRenderer.sprite = closedSprite;
        foreach (var shelf in shelves)
        {
            shelf.SetActive(false);
        }

        if (promptCanvas != null)
        {
            promptCanvas.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerInRange || !Input.GetKeyDown(KeyCode.O))
        {
            return;
        }

        if (!isOpen)
        {
            Open();
        }
        else
        {
            Close();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        playerInRange = true;
        if (promptCanvas != null)
        {
            promptCanvas.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }
        playerInRange = false;
        if (promptCanvas != null)
        {
            promptCanvas.SetActive(false);
        }

        if (isOpen)
        {
            Close();
        }
    }

    void Open()
    {
        isOpen = true;
        cabinentRenderer.sprite = openSprite;
        foreach (var shelf in shelves)
        {
            shelf.SetActive(true);
        }

        if (promptCanvas != null)
        {
            promptCanvas.SetActive(false);
        }
    }

    void Close()
    {
        isOpen = false;
        cabinentRenderer.sprite = closedSprite;
        foreach (var shelf in shelves)
        {
            shelf.SetActive(false);
        }

        if (playerInRange && promptCanvas != null)
        {
            promptCanvas.SetActive(false);
        }
    }
}
