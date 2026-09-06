using UnityEngine;
using UnityEngine.UI;

public class soilManager : MonoBehaviour
{
    [Header("Growth Sprites")] 
    public SpriteRenderer soilRenderer;
    public Sprite[] growthStages;

    [Header("UI")] 
    public Image growTimerFill;

    public GameObject promptCanvas;
    public GameObject timerCanvas;

    public float growTime = 4f;
    
    [Header("Effects")]
    public ParticleSystem growEffect;

    private bool playerInRange;

    private bool isGrowing;

    private float timer;

    private int currentStage = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        promptCanvas.SetActive(false);
        timerCanvas.SetActive(false);
        growTimerFill.fillAmount = 1f;
        if (growthStages != null && growthStages.Length > 0)
        {
            soilRenderer.sprite = growthStages[0];
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isGrowing)
        {
            timer -= Time.deltaTime;
            growTimerFill.fillAmount = timer / growTime;
            growTimerFill.color = Color.Lerp(Color.red, Color.green, timer / growTime);
            if (timer <= 0f)
            {
                OnGrowComplete();
            }

            return;
        }


    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }
        playerInRange = true;
        if (!isGrowing && !IsFullyGrown())
        {
            promptCanvas.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }
        playerInRange = false;
        promptCanvas.SetActive(false);
    }

    public void StartGrowing()
    {
        isGrowing = true;
        timer = growTime;
        promptCanvas.SetActive(false);
        timerCanvas.SetActive(true);
        if (growEffect != null)
        {
            growEffect.Play();
        }
    }

    void OnGrowComplete()
    {
        isGrowing = false;
        timerCanvas.SetActive(false);
        growTimerFill.fillAmount = 1f;
        if (growEffect != null)
        {
            growEffect.Stop();
        }
        currentStage++;
        if (growthStages != null && currentStage < growthStages.Length)
        {
            soilRenderer.sprite = growthStages[currentStage];
        }

        if (IsFullyGrown())
        {
            return;
        }

        if (playerInRange)
        {
            promptCanvas.SetActive(true);
        }
    }

    bool IsFullyGrown()
    {
        return growthStages == null || currentStage >= growthStages.Length - 1;
    }

    public void ResetSoil()
    {
        isGrowing = false;
        currentStage = 0;
        timer = 0f;
        timerCanvas.SetActive(false);
        promptCanvas.SetActive(false);
        growTimerFill.fillAmount = 1f;
        if (growthStages != null && growthStages.Length > 0)
        {
            soilRenderer.sprite = growthStages[0];
        }

        if (growEffect != null)
        {
            growEffect.Stop();
        }
    }
}
