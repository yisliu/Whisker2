using UnityEngine;

public class finishOrder : MonoBehaviour
{
    [Header("Order")] 
    public string itemTag;
    public string beverageTag;
    public Sprite foodSprite;
    public Sprite beverageSprite;
    public float tipAmount = 10f;

    private void OnMouseDown()
    {
        foodDelivered();
    }

    private void Update()
    {
        // Fallback Raycast: Detect direct clicks on the customer or delivery box
        if (Input.GetMouseButtonDown(0))
        {
            if (Camera.main == null) return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform == null) return;

                // Compare root objects safely to verify the click landed on this customer or their children
                bool isTargetHit = hit.transform == transform || hit.transform.root == transform.root;

                if (isTargetHit)
                {
                    foodDelivered();
                }
            }
        }
    }

    public void foodDelivered()
    {
        customerAI ai = GetComponentInParent<customerAI>();
        if (ai == null) return;

        GameObject food = InventorySystem.Instance?.FindItemByTag(itemTag);

        if (food == null)
        {
            Debug.LogWarning($"[finishOrder] Player clicked '{gameObject.name}' but missing required item tag: '{itemTag}'.");
            ai.TriggerGreeting();
            return;
        }

        if (!string.IsNullOrEmpty(beverageTag))
        {
            GameObject beverage = InventorySystem.Instance.FindItemByTag(beverageTag);
            if (beverage == null)
            {
                Debug.LogWarning($"[finishOrder] Player missing required beverage tag: '{beverageTag}'.");
                ai.TriggerGreeting();
                return;
            }
            InventorySystem.Instance.RemoveItem(beverage);
            Destroy(beverage);
        }

        // Successfully hand over food
        InventorySystem.Instance.RemoveItem(food);
        Destroy(food);
        MoneyManager.Instance?.AddMoney(tipAmount);

        // Complete customer order
        ai.CompleteOrder();

        // Disable this script instead of destroying the GameObject
        this.enabled = false;
    }
}