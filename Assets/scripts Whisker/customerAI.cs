using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class customerAI : MonoBehaviour
{
    [Header("Points")] 
    public Transform sPoint;
    public Transform ePoint;

    [Header("Order Settings")]
    public postitManager orderNote;
    public Sprite foodSprite;
    public Sprite beverageSprite;

    [Header("Timer")] 
    public float timer = 30f;

    [Header("UI Prompts & Displays")]
    public GameObject orderPromptCanvas;
    public GameObject timerCanvas;
    public Image timerFillImage;
    public Gradient timerColorGradient;

    [Header("Effects")] 
    public ParticleSystem happyP;
    public ParticleSystem sadP;

    [Header("Animation")]
    [SerializeField] private Animator anim;

    [Header("Dialogue")] 
    [SerializeField] private string[] greetingLines;
    [SerializeField] private string[] thankYouLines;
    [SerializeField] private string[] badLines;
    [SerializeField] private string customerName;
    [SerializeField] private Sprite customerPortrait;

    private NavMeshAgent agent;
    private float wTimer;
    private enum State { Walking, WaitingForOrder, WaitingForFood, Leaving }
    private State state = State.Walking;
    private Collider deliveryCollider;
    private bool playerInRange = false;

    void Start()
    {
        Debug.Log($"<color=cyan>[CustomerAI]</color> Start initialized on '{gameObject.name}'.");

        if (anim == null)
        {
            anim = GetComponentInChildren<Animator>();
            if (anim == null)
            {
                Debug.LogWarning($"[CustomerAI] No Animator component found on '{gameObject.name}' or its children.");
            }
        }

        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError($"<color=red>[CustomerAI ERROR]</color> No NavMeshAgent attached to '{gameObject.name}'! Customer cannot move.");
        }
        else if (!agent.isOnNavMesh)
        {
            Debug.LogError($"<color=red>[CustomerAI ERROR]</color> '{gameObject.name}' NavMeshAgent is NOT on a valid NavMesh! Check if ground is baked.");
        }

        wTimer = timer;

        deliveryCollider = GetComponentInChildren<finishOrder>()?.GetComponent<Collider>();
        if (deliveryCollider != null)
        {
            deliveryCollider.enabled = false;
        }
        else
        {
            Debug.LogWarning($"[CustomerAI] No delivery collider (finishOrder script) found under '{gameObject.name}'.");
        }

        if (orderPromptCanvas != null)
        {
            orderPromptCanvas.SetActive(false);
        }
        else
        {
            Debug.LogWarning($"[CustomerAI] 'orderPromptCanvas' is unassigned on '{gameObject.name}'.");
        }

        if (timerCanvas != null)
        {
            timerCanvas.SetActive(false);
        }

        if (sPoint != null && agent != null && agent.isOnNavMesh)
        {
            bool pathSet = agent.SetDestination(sPoint.position);
            Debug.Log($"<color=cyan>[CustomerAI]</color> Initial destination set to Service Point '{sPoint.name}'. Path valid: {pathSet}");
        }
        else if (sPoint == null)
        {
            Debug.LogWarning($"[CustomerAI] Service Point 'sPoint' is null on Start for '{gameObject.name}'. Waiting for SetPoints() call...");
        }
    }

    void Update()
    {
        if (agent == null)
        {
            return;
        }

        if (!agent.isActiveAndEnabled || !agent.isOnNavMesh)
        {
            Debug.LogWarning($"[CustomerAI] Update skipped on '{gameObject.name}': NavMeshAgent is inactive or off NavMesh.");
            return;
        }

        switch (state)
        {
            case State.Walking:
                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.8f)
                {
                    Debug.Log($"<color=green>[CustomerAI]</color> '{gameObject.name}' reached Service Point! State changed to WaitingForOrder.");
                    agent.isStopped = true;
                    state = State.WaitingForOrder;
                    TriggerGreeting();
                    UpdatePromptVisibility();
                }
                break;

            case State.WaitingForOrder:
                if (playerInRange)
                {
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        Debug.Log($"<color=green>[CustomerAI]</color> Space pressed while Player in range! Taking order from '{gameObject.name}'.");
                        TakeOrder();
                    }
                }
                break;

            case State.WaitingForFood:
                wTimer -= Time.deltaTime;

                if (timerFillImage != null && timer > 0f)
                {
                    float pct = Mathf.Clamp01(wTimer / timer);
                    timerFillImage.fillAmount = pct;

                    if (timerColorGradient != null)
                    {
                        timerFillImage.color = timerColorGradient.Evaluate(1f - pct);
                    }
                }

                if (wTimer <= 0f)
                {
                    Debug.LogWarning($"<color=yellow>[CustomerAI]</color> '{gameObject.name}' order timed out! Leaving dissatisfied.");
                    PlayEffect(sadP);
                    orderNote?.HideOrder();
                    if (timerCanvas != null) timerCanvas.SetActive(false);
                    StartLeaving();
                }
                break;

            case State.Leaving:
                if (!agent.pathPending && agent.remainingDistance < 0.5f)
                {
                    Debug.Log($"<color=cyan>[CustomerAI]</color> '{gameObject.name}' reached Exit Point. Destroying object...");
                    Destroy(gameObject);
                    return;
                }
                break;
        }

        UpdateAnimation();
    }

    void TakeOrder()
    {
        state = State.WaitingForFood;
        wTimer = timer;

        if (orderPromptCanvas != null)
        {
            orderPromptCanvas.SetActive(false);
        }

        if (timerCanvas != null)
        {
            timerCanvas.SetActive(true);
        }

        if (deliveryCollider != null)
        {
            deliveryCollider.enabled = true;
            finishOrder fo = deliveryCollider.GetComponent<finishOrder>();
            if (fo != null) fo.enabled = true;
        }

        if (orderNote != null && foodSprite != null)
        {
            orderNote.ShowOrder(foodSprite, beverageSprite);
        }
        else
        {
            Debug.LogWarning($"[CustomerAI] TakeOrder called on '{gameObject.name}', but 'orderNote' or 'foodSprite' is missing!");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"<color=yellow>[CustomerAI]</color> Trigger entered on '{gameObject.name}' by: {other.gameObject.name} (Tag: {other.tag})");

        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log($"<color=green>[CustomerAI]</color> Player entered interaction range for '{gameObject.name}'. Current State: {state}");
            UpdatePromptVisibility();
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && !playerInRange)
        {
            playerInRange = true;
            UpdatePromptVisibility();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            Debug.Log($"<color=yellow>[CustomerAI]</color> Player exited interaction range for '{gameObject.name}'.");
            UpdatePromptVisibility();
        }
    }

    private void UpdatePromptVisibility()
    {
        if (orderPromptCanvas == null) return;

        bool shouldShow = (state == State.WaitingForOrder) && playerInRange;
        orderPromptCanvas.SetActive(shouldShow);
        
        if (shouldShow)
        {
            Debug.Log($"<color=green>[CustomerAI]</color> Displaying Order Prompt Canvas for '{gameObject.name}'. Press Space to interact!");
        }
    }

    public void CompleteOrder()
    {
        if (state != State.WaitingForFood && state != State.WaitingForOrder)
        {
            Debug.LogWarning($"[CustomerAI] CompleteOrder called on '{gameObject.name}', but customer is in state {state}.");
            return;
        }

        Debug.Log($"<color=green>[CustomerAI]</color> CompleteOrder successful for '{gameObject.name}'!");

        playerInRange = false;

        if (orderPromptCanvas != null)
        {
            orderPromptCanvas.SetActive(false);
        }

        if (timerCanvas != null)
        {
            timerCanvas.SetActive(false);
        }

        orderNote?.HideOrder();
        customerDialogue.Instance?.StartDialogue(thankYouLines, customerName, customerPortrait);
        PlayEffect(happyP);
        StartLeaving();
    }

    public void TriggerGreeting()
    {
        if (state != State.Leaving)
        {
            Debug.Log($"<color=cyan>[CustomerAI]</color> Triggering greeting dialogue for '{gameObject.name}'.");
            customerDialogue.Instance?.StartDialogue(greetingLines,  customerName, customerPortrait);
        }
    }

    private void StartLeaving()
    {
        Debug.Log($"<color=cyan>[CustomerAI]</color> '{gameObject.name}' is now leaving.");
        state = State.Leaving;

        if (deliveryCollider != null)
        {
            deliveryCollider.enabled = false;
        }

        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = false;
            if (ePoint != null)
            {
                bool pathSet = agent.SetDestination(ePoint.position);
                Debug.Log($"<color=cyan>[CustomerAI]</color> Path set to Exit Point '{ePoint.name}'. Valid: {pathSet}");
            }
            else
            {
                Debug.LogError($"<color=red>[CustomerAI ERROR]</color> Exit Point 'ePoint' is null on '{gameObject.name}'! Customer cannot walk to exit.");
            }
        }
    }

    private void PlayEffect(ParticleSystem ps)
    {
        if (ps != null)
        {
            ps.Play();
        }
    }

    void UpdateAnimation()
    {
        if (anim == null || agent == null)
        {
            return;
        }

        Vector3 ve = agent.velocity;
        if (ve.magnitude > 0.1f)
        {
            anim.SetBool("isBack", ve.z > 0f);
        }

        anim.speed = (state == State.WaitingForFood || state == State.WaitingForOrder) ? 0f : 1f;
    }

    public void WrongOrder()
    {
        if (state != State.WaitingForFood)
        {
            return;
        }

        Debug.LogWarning($"[CustomerAI] Wrong order delivered to '{gameObject.name}'!");
        MoneyManager.Instance?.DeductMoney(5f);
        customerDialogue.Instance?.StartDialogue(badLines,  customerName, customerPortrait);
    }

    public void SetPoints(Transform servicePoint, Transform exitPoint)
    {
        sPoint = servicePoint;
        ePoint = exitPoint;

        Debug.Log($"<color=cyan>[CustomerAI]</color> SetPoints called on '{gameObject.name}'. ServicePoint: {(sPoint != null ? sPoint.name : "NULL")}, ExitPoint: {(ePoint != null ? ePoint.name : "NULL")}");

        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }

        if (agent != null && agent.isOnNavMesh && sPoint != null)
        {
            bool pathSet = agent.SetDestination(sPoint.position);
            Debug.Log($"<color=cyan>[CustomerAI]</color> SetDestination to Service Point '{sPoint.name}'. Path valid: {pathSet}");
        }
    }

    public void SetServicePoint(Transform point)
    {
        SetPoints(point, ePoint);
    }

    void OnDestroy()
    {
        if (Application.isPlaying)
        {
            Debug.Log($"<color=yellow>[CustomerAI]</color> OnDestroy triggered on '{gameObject.name}'. Notifying CustomerManager...");
            var manager = FindObjectOfType<customerManager>();
            if (manager != null)
            {
                manager.OnCustomerDone(this);
            }
            else
            {
                Debug.LogWarning($"[CustomerAI] OnDestroy could not find customerManager in scene.");
            }
        }
    }
}