using UnityEngine;

public class shelfToggle : MonoBehaviour
{

    public GameObject[] shelves;

    public void ShowShelves()
    {
        foreach (var shelf in shelves)
        {
            shelf.SetActive(true);
        }
    }

    public void HideShelves()
    {
        foreach (var shelf in shelves)
        {
            shelf.SetActive(false);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HideShelves();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
