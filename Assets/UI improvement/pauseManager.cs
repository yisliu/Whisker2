using UnityEngine;
using UnityEngine.SceneManagement;

public class pauseManager : MonoBehaviour
{
    public static pauseManager instance { get; private set; }

    [SerializeField] private GameObject pausePanel;

    [SerializeField] private GameObject settingsPanel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        pausePanel.SetActive(false);
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
        */
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingsPanel != null && settingsPanel.activeSelf)
            {
                CloseSettings();
            }
            else
            {
                TogglePause();
            }
        }
    }

    public void TogglePause()
    {
        bool pausing = !pausePanel.activeSelf;
        pausePanel.SetActive(pausing);
        Time.timeScale = pausing ? 0f : 1f;
    }
    public void Resume()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void OnSettings()
    {
        pausePanel.SetActive(false);
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Start Screen");
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
        pausePanel.SetActive(true);
    }
}
