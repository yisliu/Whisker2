using UnityEngine;

public class cookingEffectGarden : MonoBehaviour
{
    [Header("Prefab Input")] 
    public GameObject objectA;
    public GameObject objectB;
    [Header("Prefab Output")] 
    public GameObject objectR;

    public Transform spawnPoint;
    [Header("Soil")] 
    public soilManager oven;

    public float burnTime = 6f;
    private GameObject currentA = null;
    private GameObject currentB = null;
    private bool isCooking = false;
    private bool isCooked = false;
    private float burnTimer = 0f;

    private void OnTriggerEnter(Collider other)
    {
        if (isCooking || isCooked)
        {
            return;
        }

        if (other.gameObject.CompareTag(objectA.tag) && currentA == null)
        {
            currentA = other.gameObject;
        }

        else if (other.gameObject.CompareTag(objectB.tag) && currentB == null)
        {
            currentB = other.gameObject;
        }

        if (currentA != null && currentB != null)
        {
            StartCooking();
        }
    }

    private void Update()
    {
        if (isCooking)
        {
            
        }

        if (isCooked)
        {
            burnTimer += Time.deltaTime;
            if (burnTimer >= burnTime)
            {
                Burn();
            }
        }
        
    }

    private void StartCooking()
    {
        isCooking = true;
        Destroy(currentA);
        Destroy(currentB);
        currentA = null;
        currentB = null;
        oven.StartGrowing();
        Invoke(nameof(FinishCooking), oven.growTime);
    }

    private void FinishCooking()
    {
        isCooking = false;
        isCooked = true;
        burnTimer = 0f;
        Vector3 spawnP = spawnPoint != null?spawnPoint.position:transform.position;
        Instantiate(objectR, spawnP, Quaternion.identity);

    }

    private void Burn()
    {
        isCooked = false;
        burnTimer = 0f;
        oven.ResetSoil();
    }
}
