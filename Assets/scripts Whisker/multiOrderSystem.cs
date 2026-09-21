using UnityEngine;

[System.Serializable]
public class IngredientRequirement
{
    public string tag;
    public int count = 1;
}

public class multiOrderSystem : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Order Requirement")]
    public IngredientRequirement[] ingredients;

    public float tipAmount = 10f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!Input.GetMouseButtonDown(0) || Camera.main == null)
        {
            return;
        }
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform != null)
        {
            bool isTarget = hit.transform == transform || hit.transform.root == transform.root;
            if (isTarget)
            {
                TryDeliver();
            }
        }
    }

    public void TryDeliver()
    {
        customerAI ai = GetComponentInParent<customerAI>();
        if (ai == null)
        {
            return;
        }

        foreach (var req in ingredients)
        {
            if (InventorySystem.Instance.CountItemsByTag(req.tag) < req.count)
            {
                Debug.LogWarning($"[multiIngredientOrder] Need: {req.count}x '{req.tag}'.");
                ai.TriggerGreeting();
                return;
            }
        }

        foreach (var req in ingredients)
        {
            InventorySystem.Instance.RemoveItemsByTag(req.tag, req.count);
        }
        
        MoneyManager.Instance?.AddMoney(tipAmount);
        ai.CompleteOrder();
        this.enabled = false;
    }
}
