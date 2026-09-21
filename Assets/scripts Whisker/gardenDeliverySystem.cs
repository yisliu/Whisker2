using UnityEngine;

public class gardenDeliverySystem : MonoBehaviour
{
    [Header("Order Settings")] 
    public string itemTag;

    public int requiredCount = 1;

    public float tipAmount = 10f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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

        int have = InventorySystem.Instance.CountItemsByTag(itemTag);
        if (have < requiredCount)
        {
            Debug.LogWarning($"[gardenOrder] Need: {requiredCount}x '{itemTag}', player has {have}.");
            ai.TriggerGreeting();
            return;
        }
        InventorySystem.Instance.RemoveItemsByTag(itemTag, requiredCount);
        MoneyManager.Instance?.AddMoney(tipAmount);
        ai.CompleteOrder();
        this.enabled = false;
    }
}
