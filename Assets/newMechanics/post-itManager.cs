using UnityEngine;
using UnityEngine.UI;

public class postitManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject noteCanvas;
    public Image foodIcon;
    public Image beverageIcon;
    void Start()
    {
        noteCanvas.SetActive(false);
        if (beverageIcon != null)
        {
            beverageIcon.gameObject.SetActive(false);
        }
    }

    public void ShowOrder(Sprite foodSprite, Sprite beverageSprite = null)
    {
        noteCanvas.SetActive(true);
        foodIcon.sprite = foodSprite;
        
        if(beverageIcon != null && beverageSprite != null)
        {
            beverageIcon.sprite = beverageSprite;
            beverageIcon.gameObject.SetActive(true);
        }
    }

    public void HideOrder()
    {
        noteCanvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
