using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ovenManager : MonoBehaviour
{

    [Header("Door")] 
    public Transform door;

    public float openAngle = 100f;

    public float animDuration = 0.3f;

    public Vector3 rotationAxis = Vector3.up;

    [Header("Cooking")] 
    public string requiredRawTag;

    public GameObject cookedFoodPrefab;

    public Transform depositPoint;

    public Transform outputPoint;

    public float cookTime = 4f;

    public float burnTime = 6f;

    [Header("UI")] 
    public Image cookTimerFill;

    public GameObject promptCanvas;

    public GameObject timerCanvas;

    private bool playerInRange = false;

    private bool isCooking = false;

    private bool isCooked = false;

    private float timer = 0f;

    private float burnTimer = 0f;

    private GameObject cookedInstance;

    private Quaternion closedRotation;
    private Quaternion openRotation;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        closedRotation = door.localRotation;
        openRotation = Quaternion.Euler(door.localEulerAngles + rotationAxis * openAngle);
        promptCanvas.SetActive(false);
        timerCanvas.SetActive(false);
        cookTimerFill.fillAmount = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (isCooking)
        {
            timer -= Time.deltaTime;
            cookTimerFill.fillAmount = timer / cookTime;
            cookTimerFill.color = Color.Lerp(Color.red, Color.green, timer / cookTime);

            if (timer <= 0f)
            {
                FinishCooking();
            }

            return;

        }

        if (isCooked)
        {
            burnTimer += Time.deltaTime;
            if (burnTimer >= burnTime)
            {
                Burn();
            }

            return;
        }

        if (playerInRange && Input.GetKeyDown(KeyCode.E) && !applianceInteraction.InputConsumed)
        {
            TryDeposit();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            UpdatePrompt();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            promptCanvas.SetActive(false);
        }
    }

    void TryDeposit()
    {
        GameObject activeItem = InventorySystem.Instance.GetActiveItem();
        if (activeItem == null || !activeItem.CompareTag(requiredRawTag))
        {
            return;
        }
        applianceInteraction.InputConsumed = true;
        InventorySystem.Instance.TakeActiveItem();
        Destroy(activeItem);
        isCooking = true;
        timer = cookTime;
        timerCanvas.SetActive(true);
        promptCanvas.SetActive(false);
        door.DOLocalRotate(closedRotation.eulerAngles, animDuration).SetEase(Ease.InBack);
    }

    void FinishCooking()
    {
        isCooking = false;
        isCooked = true;
        burnTimer = 0f;
        timerCanvas.SetActive(false);
        cookTimerFill.fillAmount = 1f;

        cookedInstance = Instantiate(cookedFoodPrefab, outputPoint.position, outputPoint.rotation);
        door.DOLocalRotate(openRotation.eulerAngles, animDuration).SetEase(Ease.OutBack);
        if (playerInRange)
        {
            UpdatePrompt();
        }
    }

    void Burn()
    {
        isCooked = false;
        burnTimer = 0f;
        if (cookedInstance != null)
        {
            Destroy(cookedInstance);
        }
        
        door.DOLocalRotate(closedRotation.eulerAngles, animDuration).SetEase(Ease.InBack);
        if (playerInRange)
        {
            UpdatePrompt();
        }
    }

    void UpdatePrompt()
    {
        if (isCooking || isCooked)
        {
            return;
        }
        
        GameObject activeItem = InventorySystem.Instance.GetActiveItem();
        bool canDeposit = activeItem != null && activeItem.CompareTag(requiredRawTag);
        promptCanvas.SetActive(canDeposit);
        
    }
}
