/*

using UnityEngine;
using DG.Tweening;

public class applianceInteraction : MonoBehaviour
{
    [Header("Door")] 
    public Transform door;

    public float openAngle = 100f;

    public float animDuration = 0.3f;

    public Vector3 rotationAxis = Vector3.up;
    
    [Header("Items Avaliable")]
    public GameObject[] avaliableItems;

    [Header("References")] 
    public Transform spawnPoint;

    public GameObject promptCanvas;
    public FridgeSelectionUI selectionUI;

    private bool playerInRange = false;

    private bool isOpen = false;

    private Quaternion closedRotation;
    private Quaternion openRotation;

    public static bool InputConsumed = false;

    [Header("Shelves")] 
    public GameObject[] shelves;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (var shelf in shelves)
        {
            shelf.SetActive(false);
        }
        closedRotation = door.localRotation;
        openRotation = Quaternion.Euler(door.localEulerAngles + rotationAxis*openAngle);
        promptCanvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!isOpen)
            {
                Open();
            }
            else
            {
                Close();
            }
            applianceInteraction.InputConsumed = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            promptCanvas.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            promptCanvas.SetActive(false);
            if (isOpen)
            {
                Close();
            }
        }
    }

    void Open()
    {
        isOpen = true;
        door.DOLocalRotate(openRotation.eulerAngles, animDuration)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                foreach (var shelf in shelves)
                    shelf.SetActive(true);
            });
        selectionUI.Show(avaliableItems, this);
        promptCanvas.SetActive(false);
    }

    public void Close()
    {
        isOpen = false;
        foreach(var shelf in shelves)
            shelf.SetActive(false);
        door.DOLocalRotate(closedRotation.eulerAngles, animDuration).SetEase(Ease.InBack);
        selectionUI.Hide();
        if (playerInRange)
        {
            promptCanvas.SetActive(true);
        }
    }

    public void OnItemSelected(GameObject prefab)
    {
        if (InventorySystem.Instance.IsFull())
        {
            return;
        }

        GameObject spawned = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
        bool added = InventorySystem.Instance.AddItem(spawned);

        if (!added)
        {
            Destroy(spawned);
        }

        Close();
    }
}
*/

using UnityEngine;
using DG.Tweening;

public class applianceInteraction : MonoBehaviour
{
    [Header("Door")]
    public Transform door;

    public float openAngle = 100f;
    public float animDuration = 0.3f;
    public Vector3 rotationAxis = Vector3.up;

    [Header("Shelves")]
    public GameObject[] shelves;

    [Header("Items")]
    public GameObject[] availableItems;
    public Transform spawnPoint;

    [Header("References")]
    public GameObject promptCanvas;

    public static bool InputConsumed = false;
    private bool playerInRange = false;
    public bool isOpen = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    void Start()
    {
        
        if (door == null)
        {
            Debug.LogError(gameObject.name + " has no door");
            return;
        }
        closedRotation = door.localRotation;
        openRotation = closedRotation * Quaternion.Euler(rotationAxis * openAngle);
        promptCanvas.SetActive(false);
        foreach (var shelf in shelves)
        {
            shelf.SetActive(false);
        }
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!isOpen)
            {
                Open();
            }
            else
            {
                TryGiveItem();
                Close();
            }
        }
    }

    void TryGiveItem()
    {
        if (availableItems == null || availableItems.Length == 0) return;
        if (InventorySystem.Instance == null || InventorySystem.Instance.IsFull()) return;

        GameObject prefab = availableItems[0];
        if (prefab == null) return;

        Vector3 pos = spawnPoint != null ? spawnPoint.position : transform.position;
        GameObject spawned = Instantiate(prefab, pos, Quaternion.identity);
        bool added = InventorySystem.Instance.AddItem(spawned);
        if (!added) Destroy(spawned);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            promptCanvas.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            promptCanvas.SetActive(false);
            if (isOpen)
            {
                Close();
            }
        }
    }

    void Open()
    {
        isOpen = true;
        promptCanvas.SetActive(false);
        door.DOLocalRotateQuaternion(openRotation, animDuration)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                foreach (var shelf in shelves)
                    shelf.SetActive(true);
            });
    }

    public void Close()
    {
        isOpen = false;
        foreach (var shelf in shelves)
        {
            shelf.SetActive(false);
        }

        door.DOLocalRotateQuaternion(closedRotation, animDuration).SetEase(Ease.InBack);
        if (playerInRange)
        {
            promptCanvas.SetActive(true);
        }
    }

    void LateUpdate()
    {
        InputConsumed = false;
    }
}