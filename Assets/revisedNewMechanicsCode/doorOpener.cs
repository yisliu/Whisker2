using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class doorOpener : MonoBehaviour
{
    [Header("Door")] 
    public Transform door;

    public float openAngle = 90f;

    public float animDuration = 0.3f;
    public Vector3 rotationAxis = Vector3.up;

    [Header("UI")] 
    public GameObject promptCanvas;
    [Header("Events")]
    public UnityEvent onOpen;
    public UnityEvent onClose;
    
    public bool isOpen { get; private set; }

    private bool playerInRange;
    private Quaternion closedRotation;
    private Quaternion openRotation;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (door == null)
        {
            Debug.LogError(gameObject.name + " has no door");
            enabled = false;
            return;
        }    
        closedRotation = door.localRotation;
        openRotation = closedRotation * Quaternion.Euler(rotationAxis * openAngle);
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

    public void Open()
    {
        isOpen = true;
        if (promptCanvas != null)
        {
            promptCanvas.SetActive(false);
        }

        door.DOLocalRotateQuaternion(openRotation, animDuration)
            .SetEase(Ease.OutBack)
            .OnComplete(() => onOpen?.Invoke());
    }

    public void Close()
    {
        isOpen = false;
        door.DOLocalRotateQuaternion(closedRotation, animDuration)
            .SetEase(Ease.InBack)
            .OnComplete(() => onClose?.Invoke());
        if (playerInRange && promptCanvas != null)
        {
            promptCanvas.SetActive(true);
        }
    }
}
