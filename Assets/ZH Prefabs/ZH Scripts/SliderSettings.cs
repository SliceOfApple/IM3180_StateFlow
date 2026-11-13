using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SliderSettings : MonoBehaviour
{
    public Slider sliderMaster;
    public Slider sliderMusic;
    public Slider sliderEffects;

    public float defaultValue = 1.0f;

    private float oldMasterValue = 0f,
                  oldMusicValue = 0f,
                  oldSFXValue = 0f;

    public string prefKey1 = "MasterVolume";
    public string prefKey2 = "MusicVolume";
    public string prefKey3 = "EffectsVolume";

    public TextMeshProUGUI MasterText;
    public TextMeshProUGUI MusicText;
    public TextMeshProUGUI EffectsText;

    public int delay;

    public TextMeshProUGUI delayText;

    public AudioSource audioSource_SFX;
    private AudioMixer audioMixer;

    void Start()
    {
        audioMixer = Resources.Load<AudioMixer>("MainAudioMixer");

        sliderMaster.value = PlayerPrefs.GetFloat(prefKey1, defaultValue);
        MasterText.text = ((int)(sliderMaster.value * 100)).ToString();
        sliderMaster.onValueChanged.AddListener(SaveValue1);
        sliderMusic.value = PlayerPrefs.GetFloat(prefKey2, defaultValue);
        MusicText.text = ((int)(sliderMusic.value * 100)).ToString();
        sliderMusic.onValueChanged.AddListener(SaveValue2);
        sliderEffects.value = PlayerPrefs.GetFloat(prefKey3, defaultValue);
        sliderEffects.onValueChanged.AddListener(SaveValue3);
        EffectsText.text = ((int)(sliderEffects.value * 100)).ToString();
        delay = PlayerPrefs.GetInt("Delay", 0);
        delayText.text = delay.ToString();
    }

    private void SaveValue1(float newValue)
    {
        if (Mathf.Round(oldMasterValue * 100f) / 100f != Mathf.Round(newValue * 100f) / 100f)
        {
            if (audioSource_SFX != null) audioSource_SFX.Play();
            oldMasterValue = newValue;
        }
        PlayerPrefs.SetFloat(prefKey1, newValue);
        PlayerPrefs.Save();
        MasterText.text = ((int)(newValue * 100)).ToString();

        if (audioMixer != null) audioMixer.SetFloat("MasterVol", Mathf.Log10(newValue) * 20);
    }
    private void SaveValue2(float newValue)
    {
        //if (Mathf.Round(oldMusicValue * 100f) / 100f != Mathf.Round(newValue * 100f) / 100f)
        //{
        //    if (audioSource_SFX != null) audioSource_SFX.Play();
        //    oldMusicValue = newValue;
        //}
        PlayerPrefs.SetFloat(prefKey2, newValue);
        PlayerPrefs.Save();
        MusicText.text = ((int)(newValue * 100)).ToString();

        if (audioMixer != null) audioMixer.SetFloat("MusicVol", Mathf.Log10(newValue) * 20);
    }
    private void SaveValue3(float newValue)
    {
        if (Mathf.Round(oldSFXValue * 100f) / 100f != Mathf.Round(newValue * 100f) / 100f)
        {
            if (audioSource_SFX != null) audioSource_SFX.Play();
            oldSFXValue = newValue;
        }
        PlayerPrefs.SetFloat(prefKey3, newValue);
        PlayerPrefs.Save();
        EffectsText.text = ((int)(newValue * 100)).ToString();

        if (audioMixer != null) audioMixer.SetFloat("SFXVol", Mathf.Log10(newValue) * 20);
    }
}
