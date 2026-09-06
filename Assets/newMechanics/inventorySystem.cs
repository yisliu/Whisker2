using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; private set; }
    private GameObject[] slots = new GameObject[4];
    private int activeSlot = 0;
    public Transform holdPoint;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetActiveSlot(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetActiveSlot(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetActiveSlot(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SetActiveSlot(3);
        }
    }

    public bool AddItem(GameObject item)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null)
            {
                slots[i] = item;
                Rigidbody rb = item.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = true;
                }
                Collider col = item.GetComponent<Collider>();
                if (col != null)
                {
                    col.enabled = false;
                }
                item.SetActive(false);
                //InventoryUI.Instance.UpdateSlot(i, GetSprite(item));
                if (InventoryUI.Instance != null)
                {
                    InventoryUI.Instance.UpdateSlot(i, GetSprite(item));
                }
                else
                {
                    Debug.LogWarning("InventoryUI.Instance is null - add InventoryUI to the scene.");
                }
                UpdateHoldPoint();
                return true;
            }
        }

        return false;
    }

    public GameObject TakeActiveItem()
    {
        GameObject item = slots[activeSlot];
        if (item == null)
        {
            return null;
        }

        slots[activeSlot] = null;
        InventoryUI.Instance.UpdateSlot(activeSlot, null);
        UpdateHoldPoint();
        return item;
    }

    public GameObject GetActiveItem()
    {
        return slots[activeSlot];
    }

    public void RemoveItem(GameObject item)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == item)
            {
                slots[i] = null;
                InventoryUI.Instance.UpdateSlot(i, null);
                break;
            }
        }

        UpdateHoldPoint();
    }

    public bool IsFull()
    {
        foreach (var s in slots)
        {
            if (s == null)
            {
                return false;
            }
        }

        return true;
    }

    void SetActiveSlot(int slot)
    {
        activeSlot = slot;
        InventoryUI.Instance.SetHighlight(activeSlot);
        UpdateHoldPoint();
    }

    void UpdateHoldPoint()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null)
            {
                continue;
            }
            slots[i].SetActive(i == activeSlot);
            if (i == activeSlot)
            {
                slots[i].transform.SetParent(holdPoint);
                slots[i].transform.localPosition = Vector3.zero;
                slots[i].transform.localRotation = Quaternion.identity;
            }
        }
    }

    Sprite GetSprite(GameObject item)
    {
        var sr = item.GetComponent<SpriteRenderer>();
        if (sr)
        {
            return sr.sprite;
        }

        var img = item.GetComponentInChildren<SpriteRenderer>();
        return img ? img.sprite : null;
    }
/*
    public GameObject FindItemByTag(string tag)
    {
        foreach (var slot in slots)
        {
            if (slot != null && slot.CompareTag(tag))
            {
                return slot;
            }
        }

        return null;
    }
*/
    public GameObject FindItemByTag(string tag)
    {
        Debug.Log($"Looking for tags: '{tag}' | slots:{slots[0]?.tag}, {slots[1]?.tag}, {slots[2]?.tag}, {slots[3]?.tag}");
        foreach (var slot in slots)
        {
            if (slot != null && slot.CompareTag(tag))
            {
                return slot;
            }

            //return null;
        }

        return null;
    }
/*
    public GameObject FindItemByID(int id)
    {
        foreach (var slot in slots)
        {
            if (slot != null && slot.CompareTag(tag))
            {
                return slot;
            }

            return null;
        }
    }
    
    */
}
