using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public static InventoryUI Instance { get; private set; }

    [System.Serializable]

    public struct SlotUI
    {
        public Image itemIcon;
        public GameObject selectionHighlight;
    }

    public SlotUI[] slots = new SlotUI[4];

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        SetHighlight(0);
    }

    public void UpdateSlot(int index, Sprite sprite)
    {
        if (sprite != null)
        {
            slots[index].itemIcon.sprite = sprite;
            slots[index].itemIcon.enabled = true;
        }
        else
        {
            slots[index].itemIcon.enabled = false;
        }
    }

    public void SetHighlight(int index)
    {
        //for (int i = 0; i < slots.Length; i++)
        //{
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i].selectionHighlight.SetActive(i == index);
            }
        //}
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
