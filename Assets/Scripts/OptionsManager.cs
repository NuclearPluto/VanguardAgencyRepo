using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class OptionsManager : UIManager
{
    [SerializeField] public UnityEngine.Canvas mainMenuOverlay;
    public Slider soundEffectsSlider;
    public Slider musicSlider;

    private void Start() {
        float soundEffectsVolume = PlayerPrefs.GetFloat("SoundEffectsVolume", 1f);
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);

        soundEffectsSlider.value = soundEffectsVolume;
        musicSlider.value = musicVolume;
    }

    protected override void Awake() {
        base.Awake();
        containerRectTransform.anchoredPosition = new Vector3(0f, 300f, 0f);
    }
    
    public void BackToMenu() {
        mainMenuOverlay.gameObject.GetComponent<MainMenuManager>().Options();
    }

    public void FullScreen() {
        Screen.fullScreen = true;
    }

    public void Windowed() {
        Screen.fullScreen = false;
    }

    public void OnSoundEffectsVolumeChanged(float volume)
    {
        AudioManager.Instance.SetSoundEffectsVolume(volume);
        PlayerPrefs.SetFloat("SoundEffectsVolume", volume);
        PlayerPrefs.Save();
    }

    public void OnMusicVolumeChanged(float volume)
    {
        AudioManager.Instance.SetMusicVolume(volume);
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }

}
