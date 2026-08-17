using UnityEngine;
using UnityEngine.UI;

public class settingsManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Toggle fullscreen;
    [SerializeField] private GameObject settingsPanel;
    void Start()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("Volume", 1f);
        fullscreen.isOn = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        ApplyVolume(volumeSlider.value);
        ApplyFullscreen(fullscreen.isOn);
    }

    public void OnVolumeChanged(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("Volume", volume);
    }

    public void OnFullscreenChanged(bool value)
    {
        Screen.fullScreen = value;
        PlayerPrefs.SetInt("Fullscreen", value ? 1 : 0);
    }

    public void Close()
    {
        settingsPanel.SetActive(false);
        if (pauseManager.instance != null)
        {
            pauseManager.instance.CloseSettings();
        }
    }

    void ApplyVolume(float value) => AudioListener.volume = value;
    void ApplyFullscreen(bool value) => Screen.fullScreen = value;

    // Update is called once per frame
    void Update()
    {
        
    }
}
