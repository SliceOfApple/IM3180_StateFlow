using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class TitleScreenMusicManager : MonoBehaviour
{
    private static TitleScreenMusicManager _instance;
    public static TitleScreenMusicManager Instance => _instance;

    // Song details
    public int bpm;
        private double spb;
    public float loopStart, // Loop start in beats
                 loopEnd; // Loop end in beats

    [HideInInspector]
    public AudioSource audioSource_Music,
                       audioSource_SFX;

    public AudioClip sfxMouseDown,
                     sfxMouseUp;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Preset mixer values.
        AudioMixer mainMixer = Resources.Load<AudioMixer>("MainAudioMixer");
        if (mainMixer != null)
        {
            mainMixer.SetFloat("MasterVol", Mathf.Log10(PlayerPrefs.GetFloat("MasterVolume", 1f)) * 20);
            mainMixer.SetFloat("MusicVol", Mathf.Log10(PlayerPrefs.GetFloat("MusicVolume", 1f)) * 20);
            mainMixer.SetFloat("SFXVol", Mathf.Log10(PlayerPrefs.GetFloat("EffectsVolume", 1f)) * 20);
        }

        if (_instance != null && _instance != this)
        {
            Debug.Log("[Title Screen Music] C");
            Destroy(gameObject);
            return;
        }
        Debug.Log("[Title Screen Music] A");

        spb = 60f / bpm;
        audioSource_Music = GetComponents<AudioSource>()[0];
        audioSource_SFX = GetComponents<AudioSource>()[1];

        if (AudioListener.pause) AudioListener.pause = false;
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (audioSource_Music.time >= loopEnd*spb)
        {
            audioSource_Music.time = (float)(loopStart*spb);
        }
    }

    public void SoftenAndStopMusic()
    {
        if (_instance != null) StartCoroutine(SoftenMusicCoroutine());
    }

    IEnumerator SoftenMusicCoroutine()
    {
        float initialVolume = audioSource_Music.volume;
        float duration = 0.5f; // Duration to soften the music
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            audioSource_Music.volume = Mathf.Lerp(initialVolume, 0f, elapsedTime / duration);
            yield return null;
        }
        audioSource_Music.Stop();
        audioSource_Music.volume = initialVolume; // Reset volume for next play
        DestroyInstance();
    }

    public void DestroyInstance() { _instance = null; Destroy(gameObject); Debug.Log("[Title Screen Music] Instance destroyed."); }
}
