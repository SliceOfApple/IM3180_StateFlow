using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.Windows;
using static Utility;

public class AudioSyncing : MonoBehaviour
{
    public static AudioSyncing _musicManager; // Singleton.
    public static AudioSyncing Instance => _musicManager;
    public double ElapsedTime => AudioSettings.dspTime - startTime;

    /// <summary>
    /// <see cref="AudioSource"/> that will play music.
    /// </summary>
    [HideInInspector]
    public AudioSource audioSource;
    public AudioClip onBeatSFX;

    public double BPM = 132,
                  secPerBeat = 0d;
    public double audioLatency = 0d; // Variable value in case of audio latency.
    public int hitQuota = 10;

    /// <summary>
    /// Offset of the initial <see cref="AudioSettings.dspTime"/> value when this class was instantiated. 
    /// <br/>To reset this value in the case of a new level, pause the <see cref="AudioListener"/> and then replace this value with the latest dspTime value.
    /// </summary>
    private double startTime = 0;
    private int lastBeat = 0;

    public TMP_Text beatText;
    private InputAction comboToggleButton,
                    rhythmUp,
                    rhythmDown,
                    rhythmLeft,
                    rhythmRight;
    public List<Double> Attempts;
    public int median;
    public TextMeshProUGUI Input;
    public TextMeshProUGUI Counter;
    /// <summary>
    /// Returns true if the next segment is uptime, <u>and</u> the current segment is not.
    /// </summary>


    public event Action Destroyed;

    private void OnDestroy()
    {
        Destroyed?.Invoke();
    }
    void Start()
    {
        if (_musicManager == null)
        {
            _musicManager = this;
        }
        else
        {
            throw new InvalidOperationException("Music manager is already instantiated.");
        }
        if (Keyboard.current != null)
        {
            InputSystem.EnableDevice(Keyboard.current);
        }
        audioSource = GetComponent<AudioSource>();
        secPerBeat = 60d / BPM;
        //AudioListener.pause = true;
        startTime = AudioSettings.dspTime;

        //GameManager.instance.LoadBeatmap("Assets/Beatmaps/BeethovenVirus.xml");

        //if (GameManager.instance != null) GameManager.instance.LoadBeatmap("Assets/Beatmaps/BeethovenVirus.xml");

        DontDestroyOnLoad(gameObject);
        //PlayMusic();
        comboToggleButton = InputSystem.actions.FindAction("ComboToggle");
        rhythmUp = InputSystem.actions.FindAction("RhythmUp");
        rhythmDown = InputSystem.actions.FindAction("RhythmDown");
        rhythmLeft = InputSystem.actions.FindAction("RhythmLeft");
        rhythmRight = InputSystem.actions.FindAction("RhythmRight");
        Input.text = "Press " + rhythmUp.GetBindingDisplayString(0) + ", " +
        rhythmDown.GetBindingDisplayString(0) + ", " +
        rhythmLeft.GetBindingDisplayString(0) + ", or " +
        rhythmRight.GetBindingDisplayString(0) + " to the beat";

        AudioListener.pause = false;
    }
    void Update()
    {
        GameManager gm = GameManager.Instance;
        double eventTimestamp = secPerBeat * 2; /*- 0.28;*/

        // Track the beats while audioSource_Music is playing.
        if (!AudioListener.pause)
        {
            //int currentBeat = (int) (elapsedTime / secPerBeat);
            int currentBeat = (int)Math.Ceiling((ElapsedTime - secPerBeat) / secPerBeat + audioLatency);

            //Debug.Log($"Comparing nextEventTimestamp: {eventTimestamp}s\n\nWith elapsedTime: {elapsedTime}s");

            if (currentBeat > lastBeat)
            {
                Debug.Log("Current Beat: " + currentBeat);
                audioSource.PlayOneShot(onBeatSFX);
                lastBeat = currentBeat;
                if (beatText != null) beatText.text = "Beat: " + currentBeat + "\n" + "Measure: " + Math.Ceiling(0.25 * currentBeat);
                //Debug.Log("Beat: " + currentBeat);
            }
        }
        if (rhythmRight.WasPressedThisFrame() || rhythmLeft.WasPressedThisFrame() || rhythmUp.WasPressedThisFrame() || rhythmDown.WasPressedThisFrame())
        {
            double time = CheckAccuracy(); /*- 0.22;*/
            Debug.Log(time);
            Attempts.Add(time);
            if (Attempts.Count >= hitQuota)
            {
                Attempts.Sort();
                if (Math.Abs(Attempts[(int) (hitQuota/2) - 1] - Attempts[(int) (hitQuota/2)]) > 0.8) median = (int)(Attempts[(int)(hitQuota / 2)] * 1000);
                else median = (int)((Attempts[(int)(hitQuota / 2)] + Attempts[(int)((hitQuota - 1) / 2)] * 500));
                SetDelay(median);
            }
            else
            {
                Counter.text = (hitQuota - Attempts.Count) + " more to complete syncing";
            }
        }
    }
    public void SetDelay(int median)
    {
        PlayerPrefs.SetInt("Delay", median);
        PlayerPrefs.Save();
        Destroy(gameObject.transform.parent.gameObject);
    }
    public double CheckAccuracy()
    {
        double beatsPassed = ElapsedTime / secPerBeat; // Plus one as offset due to zero-based indexing.

        //// Find the nearest beat time.
        double nearestBeatTime = Math.Round(beatsPassed) * secPerBeat;
        //// Calculate time difference between the current time and the nearest beat time.
        double timeDifference = (ElapsedTime - nearestBeatTime);
        return timeDifference;
    }
    public void PlayMusic()
    {
        audioSource.PlayScheduled(startTime + (secPerBeat * 5) - audioLatency); // Delay to allow the player to prepare for the first beat.
        Debug.Log("Audio source playing from local time: " + audioSource.time + "s.");
        AudioListener.pause = false;
    }
}
