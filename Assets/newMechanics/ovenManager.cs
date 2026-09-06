using UnityEngine;
using UnityEngine.UI;
//using DG.Tweening;

public class ovenManager : MonoBehaviour
{
    [Header("Door Sprites")] 
    public SpriteRenderer ovenRenderer;

    public Sprite openSprite;
    public Sprite closedSprite;

    [Header("UI")] 
    public Image cookTimerFill;

    public GameObject promptCanvas;
    public GameObject timerCanvas;
    public float cookTime = 4f;
    private bool playerInRange;
    private bool isCooking;
    private float timer;
    
    [Header("Effects")]
    public ParticleSystem smokeEffect;

    void Start()
    {
        promptCanvas.SetActive(false);
        timerCanvas.SetActive(false);
        cookTimerFill.fillAmount = 1f;
        ovenRenderer.sprite = openSprite;
    }

    void Update()
    {
        if (!isCooking)
        {
            return;
        }
        timer -= Time.deltaTime;
        cookTimerFill.fillAmount = timer/cookTime;
        cookTimerFill.color = Color.Lerp(Color.red, Color.green, timer/cookTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }
        playerInRange = true;
        if (!isCooking)
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

    public void StartCooking()
    {
        if (smokeEffect != null)
        {
            smokeEffect.Play();
        }
        isCooking = true;
        timer = cookTime;
        timerCanvas.SetActive(true);
        promptCanvas.SetActive(false);
        ovenRenderer.sprite = closedSprite;
    }

    public void OnCookingComplete()
    {
        if (smokeEffect != null)
        {
            smokeEffect.Stop();
        }
        isCooking = false;
        timerCanvas.SetActive(false);
        cookTimerFill.fillAmount = 1f;
        ovenRenderer.sprite = openSprite;
    }

    public void OnBurn()
    {
        if (smokeEffect != null)
        {
            smokeEffect.Stop();
        }
        isCooking = false;
        timerCanvas.SetActive(false);
        cookTimerFill.fillAmount = 1f;
        ovenRenderer.sprite = closedSprite;
        if (playerInRange)
        {
            promptCanvas.SetActive(true);
        }
    }
}
