using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class customerManager : MonoBehaviour
{
    public static customerManager Instance { get; private set; }

    [Header("Prefabs & Locations")]
    public GameObject[] customerPrefabs;
    public Transform[] spawnPoints;
    public Transform[] servicePoints;
    public Transform[] exitPoints;

    [Header("Session Settings")]
    public int totalCustomers = 10;
    
    private Queue<GameObject> customerQueue = new Queue<GameObject>();
    private GameObject[] activeCustomers;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[CustomerManager] Duplicate instance detected and destroyed.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Debug.Log("<color=cyan>[CustomerManager]</color> Singleton instance initialized successfully.");
    }

    void Start()
    {
        // Debug Check: Service Points
        if (servicePoints == null || servicePoints.Length == 0)
        {
            Debug.LogError("<color=red>[CustomerManager ERROR]</color> No service points assigned in the Inspector! Customer manager cannot function.");
            return;
        }

        // Debug Check: Customer Prefabs
        if (customerPrefabs == null || customerPrefabs.Length == 0)
        {
            Debug.LogError("<color=red>[CustomerManager ERROR]</color> No Customer Prefabs assigned in the Inspector! Cannot populate queue.");
            return;
        }

        activeCustomers = new GameObject[servicePoints.Length];
        
        // Build customer pool
        List<GameObject> list = new List<GameObject>();
        for (int i = 0; i < totalCustomers; i++)
        {
            GameObject selectedPrefab = customerPrefabs[Random.Range(0, customerPrefabs.Length)];
            if (selectedPrefab != null)
            {
                list.Add(selectedPrefab);
            }
            else
            {
                Debug.LogWarning($"[CustomerManager] Prefab at index in array was null while populating total customers.");
            }
        }

        // Shuffle queue
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            var temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }

        foreach (var c in list)
        {
            customerQueue.Enqueue(c);
        }

        Debug.Log($"<color=cyan>[CustomerManager]</color> Queue initialized with {customerQueue.Count} customers. Attempting initial spawn across {servicePoints.Length} service point(s)...");

        // Spawn initial customers for available counters
        for (int i = 0; i < servicePoints.Length; i++)
        {
            SpawnAtPoint(i);
        }
    }

    void SpawnAtPoint(int index)
    {
        if (customerQueue.Count == 0)
        {
            Debug.LogWarning($"<color=yellow>[CustomerManager]</color> SpawnAtPoint({index}) called, but the customer queue is empty.");
            return;
        }

        if (activeCustomers[index] != null)
        {
            Debug.LogWarning($"<color=yellow>[CustomerManager]</color> SpawnAtPoint({index}) called, but service point slot {index} is already occupied by {activeCustomers[index].name}.");
            return;
        }

        // Validate Service Point
        if (servicePoints[index] == null)
        {
            Debug.LogError($"<color=red>[CustomerManager ERROR]</color> Service point at index {index} is null in the Inspector array!");
            return;
        }

        GameObject prefab = customerQueue.Dequeue();
        
        // Get matching spawn point for this index (fallback to service point if array is empty/short)
        Transform currentSpawn = (spawnPoints != null && index < spawnPoints.Length && spawnPoints[index] != null) 
            ? spawnPoints[index] 
            : servicePoints[index];

        if (spawnPoints == null || index >= spawnPoints.Length || spawnPoints[index] == null)
        {
            Debug.LogWarning($"[CustomerManager] No valid SpawnPoint at index {index}. Falling back to Service Point position.");
        }

        // Get matching exit point for this index
        Transform currentExit = (exitPoints != null && index < exitPoints.Length && exitPoints[index] != null) 
            ? exitPoints[index] 
            : null;

        if (currentExit == null)
        {
            Debug.LogWarning($"[CustomerManager] Exit Point at index {index} is null or unassigned. Customers will have no exit target when done.");
        }

        // Instantiate
        GameObject customerObj = Instantiate(prefab, currentSpawn.position, currentSpawn.rotation);
        customerAI ai = customerObj.GetComponent<customerAI>();

        if (ai != null)
        {
            ai.SetPoints(servicePoints[index], currentExit);
            Debug.Log($"<color=green>[CustomerManager]</color> Spawned customer standard prefab '{customerObj.name}' at Spawn Index {index}. Heading to Service Point: {servicePoints[index].name}. Remaining queue: {customerQueue.Count}");
        }
        else
        {
            Debug.LogError($"<color=red>[CustomerManager ERROR]</color> Instantiated prefab '{customerObj.name}' does NOT have a 'customerAI' component attached!");
        }

        activeCustomers[index] = customerObj;
    }

    public void OnCustomerDone(customerAI customer)
    {
        if (!Application.isPlaying || this == null)
        {
            return;
        }

        Debug.Log($"<color=cyan>[CustomerManager]</color> OnCustomerDone received for customer '{customer.gameObject.name}'. Searching active slots...");

        for (int i = 0; i < activeCustomers.Length; i++)
        {
            if (activeCustomers[i] == customer.gameObject)
            {
                Debug.Log($"<color=green>[CustomerManager]</color> Active customer cleared at index {i}. Queueing next spawn check...");
                activeCustomers[i] = null;
                StartCoroutine(SpawnAndCheckSession(i));
                return;
            }
        }

        Debug.LogWarning($"[CustomerManager] OnCustomerDone called for '{customer.gameObject.name}', but customer was not found in the active list!");
    }

    IEnumerator SpawnAndCheckSession(int index)
    {
        yield return null;
        
        Debug.Log($"<color=cyan>[CustomerManager]</color> Checking session status for slot index {index}. Queue remaining: {customerQueue.Count}");
        
        SpawnAtPoint(index);
        
        if (customerQueue.Count == 0 && AllEmpty())
        {
            Debug.Log("<color=green>[CustomerManager]</color> Queue is empty AND all active slots are clear. Ending session!");
            EndSession();
        }
    }

    bool AllEmpty()
    {
        foreach (var c in activeCustomers)
        {
            if (c != null)
            {
                return false;
            }
        }

        return true;
    }

    void EndSession()
    {
        Debug.Log("<color=gold>=========================================</color>");
        Debug.Log("<color=gold>[CustomerManager] SHIFT DONE! Triggering Win Screen.</color>");
        Debug.Log("<color=gold>=========================================</color>");

        if (WinScreenManager.Instance != null)
        {
            WinScreenManager.Instance.TriggerWin();
        }
        else
        {
            Debug.LogError("[CustomerManager ERROR] WinScreenManager.Instance is null! Could not trigger win screen.");
        }
    }
}