using UnityEngine;

public class cookingEffectCafe : MonoBehaviour
{
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Prefab Inputs")]
    public GameObject[] requiredIngredients;

    [Header("Prefab Outputs")] 
    public GameObject objectR;

    public Transform spawnPoint;
    [Header("Oven")] 
    public ovenManager oven;

    public float burnTime = 6f;
    private GameObject[] currentIngredients;
    private bool isCooking = false;
    private bool isCooked = false;
    private float burnTimer = 0f;
    void Start()
    {
        currentIngredients = new GameObject[requiredIngredients.Length];
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isCooking || isCooked)
        {
            return;
        }

        for (int i = 0; i < requiredIngredients.Length; i++)
        {
            if (currentIngredients[i] != null)
            {
                continue;
            }

            if (other.gameObject.CompareTag(requiredIngredients[i].tag))
            {
                currentIngredients[i] = other.gameObject;
                break;
            }
        }

        if (AllIngredientsFilled())
        {
            StartCooking();
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (isCooked)
        {
            burnTimer += Time.deltaTime;
            if (burnTimer >= burnTime)
            {
                Burn();
            }
        }
    }

    private bool AllIngredientsFilled()
    {
        for (int i = 0; i < currentIngredients.Length; i++)
        {
            if (currentIngredients[i] == null)
            {
                return false;
            } 
        }
        return currentIngredients.Length > 0;
    }

    private void StartCooking()
    {
        isCooking = true;
        for (int i = 0; i < currentIngredients.Length; i++)
        {
            Destroy(currentIngredients[i]);
            currentIngredients[i] = null;
        }
        oven.StartCooking();
        Invoke(nameof(FinishCooking), oven.cookTime);
    }

    private void FinishCooking()
    {
        isCooking = false;
        isCooked = true;
        burnTimer = 0f;
        Vector3 spawnP = spawnPoint != null ? spawnPoint.position : transform.position;
        Instantiate(objectR, spawnP, Quaternion.identity);
        oven.OnCookingComplete();
    }

    private void Burn()
    {
        isCooked = false;
        burnTimer = 0f;
        oven.OnBurn();
    }
}
