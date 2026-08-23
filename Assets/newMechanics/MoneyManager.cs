using UnityEngine;
using TMPro;


public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance {get; private set;}
    public TMP_Text moneyText;
    private float currentMoney = 0f;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        UpdateUI();
    }

    public void AddMoney(float amount)
    {
        currentMoney += amount;
        UpdateUI();
    }

    public void DeductMoney(float amount)
    {
        currentMoney = Mathf.Max(0f, currentMoney - amount);
        UpdateUI();
    }

    void UpdateUI()
    {


            if (moneyText != null)
            {
                moneyText.text = "$" + currentMoney.ToString("F2");
            }

    }

// Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
