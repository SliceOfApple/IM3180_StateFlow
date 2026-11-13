using JetBrains.Annotations;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using static Utility;

/// <summary>
/// For music control. A <b>singleton</b>, only one instance of this class can exist at any time.<br/>
/// Refer to the singleton with <see cref="Instance"/>.
/// </summary>
public class MusicManager : MonoBehaviour
{
    private static MusicManager _musicManager; // Singleton.
    public static MusicManager Instance => _musicManager;
    public double ElapsedTime => AudioSettings.dspTime - startTime;

    /// <summary>
    /// <see cref="AudioSource"/> that will play music.
    /// </summary>
    [HideInInspector]
    public AudioSource audioSource,
                       audioSourceSFX;
    public AudioClip upbeat,
                     downbeat;
    private AudioMixer audioMixer;

    public GameObject beatIndicatorLeft,
                      beatIndicatorRight;
    public Sprite sprite_BeatOn,
                  sprite_BeatOff;

    public float beatIndicatorFlashDuration = 0.1f;
    private float _flashTime;
    private bool _isBeatFlashing = false;

    public double BPM = 156,
                  secPerBeat = 0d;
    public double audioLatency; // Variable value in case of audio latency.

    // Accuracy variables.
    private double ThresholdPerfect,
                   ThresholdGood,
                   ThresholdOK,
                   ThresholdEarlyLate,
                   ThresholdMiss;
    public event Action<int> BeatCount;

    public double flatDelay; // For processing, track or unknown delays. Level-specific variable.

    /// <summary>
    /// Offset of the initial <see cref="AudioSettings.dspTime"/> value when this class was instantiated. 
    /// <br/>To reset this value in the case of a new level, pause the <see cref="AudioListener"/> and then replace this value with the latest dspTime value.
    /// </summary>
    public double startTime = 0;
    public int lastBeat = 0;

    public TMP_Text beatText;

    /// <summary>
    /// Returns true if the next segment is uptime, <u>and</u> the current segment is not.
    /// </summary>
    private bool IsNextSegmentUptime
    {
        get
        {
            GameManager gm = GameManager.Instance;
            Segment currentSegment, nextSegment;

            // For the first segment, if it's uptime we will return true to warn the player.
            //if (gm.segmentSelector == 0 && gm.segments[0].isUptime) return true;

            if (gm.IsMusicSeekingNext)
            {
                currentSegment = gm.segments[gm.oldSegmentSelector];
                nextSegment = gm.segments[gm.segmentSelector];
            }
            else
            {
                if (gm.segmentSelector + 1 >= gm.segments.Count) return false;
                currentSegment = gm.segments[gm.segmentSelector];
                nextSegment = gm.segments[gm.segmentSelector + 1];
            }

            return !currentSegment.isUptime && nextSegment.isUptime;
        }
    }

    //public bool IsNextSegmentUptime = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
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

        audioMixer = Resources.Load<AudioMixer>("MainAudioMixer");

        audioSource = GetComponent<AudioSource>();
        audioSourceSFX = GetComponents<AudioSource>()[1];

        AudioListener.pause = true;
        startTime = AudioSettings.dspTime;

        _flashTime = beatIndicatorFlashDuration;

        audioLatency = ((double) PlayerPrefs.GetInt("Delay", 0)) / 1000;

        //GameManager.instance.LoadBeatmap("Assets/Beatmaps/BeethovenVirus.xml");

        //if (GameManager.instance != null) GameManager.instance.LoadBeatmap("Assets/Beatmaps/BeethovenVirus.xml");

        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    public void CountBeat(int count)
    {
        BeatCount?.Invoke(count);
    }

    void Update()
    {
        GameManager gm = GameManager.Instance;
        double eventTimestamp = gm.NextEventTimestamp;

        // Track the beats while audioSource_Music is playing.
        if (audioSource.isPlaying)
        {
            //int currentBeat = (int) (elapsedTime / secPerBeat);
            int currentBeat = (int)Math.Ceiling((ElapsedTime - secPerBeat) / secPerBeat + audioLatency);

            //Debug.Log($"Comparing nextEventTimestamp: {eventTimestamp}s\n\nWith elapsedTime: {elapsedTime}s");

            if (ElapsedTime > gm.NextSegmentTimestamp) gm.UpdateNextSegmentTimestamp();
            if (ElapsedTime >= gm.NextEventTimestamp) { gm.NextEvent(); }
            if (gm.IsMusicSeekingNext && ElapsedTime >= gm.SeekTimestamp) // Check if a seek request was made by the GameManager from the previous event.
            {
                gm.IsMusicSeekingNext = false;
                gm.oldSegmentSelector = gm.segmentSelector;
                // Setting time vs timeSamples: https://docs.unity3d.com/ScriptReference/AudioSource-time.html
                audioSource.timeSamples = Mathf.FloorToInt((float)(gm.SeekTo * audioSource.clip.frequency));
                Debug.Log("[Music] Successfully seeked to " + audioSource.time + "s.");
            }
            if (currentBeat > lastBeat)
            {
                CountBeat(currentBeat);
                _isBeatFlashing = true;
                beatIndicatorLeft.GetComponent<Image>().sprite = sprite_BeatOn;
                beatIndicatorRight.GetComponent<Image>().sprite = sprite_BeatOn;

                Debug.Log("Current Beat: " + currentBeat);
                Debug.Log("Old Beat: " + lastBeat);
                int FlashingCountsRemaining = gm.FlashingCountsRemaining;

                if (FlashingCountsRemaining >= -1)
                {
                    if (FlashingCountsRemaining > 0)
                    {
                        // Bright red on 4, 2, dim red on 3, 1.
                        Color newColour = FlashingCountsRemaining % 2 == 0 ? new Color(1, 20 / 255, 20 / 255) : new Color(0.5f, 20 / 255, 20 / 255);
                        audioSourceSFX.PlayOneShot(downbeat, 1f);
                        gm.incomingIndicator.GetComponent<TMP_Text>().color = newColour;
                    }
                    else
                    {
                        //gm.IsWarningPlayer = false;
                        gm.incomingIndicator.GetComponent<TMP_Text>().color = new Color(0, 0, 0, 0);
                        //IsNextSegmentUptime = false;
                        gm.IsInUptime = true;
                    }
                    gm.FlashingCountsRemaining--;
                }
                gm.NextBeat();
                lastBeat = currentBeat;
                if (beatText != null) beatText.text = "Beat: " + currentBeat + "\n" + "Measure: " + Math.Ceiling(0.25 * currentBeat);
                //Debug.Log("Beat: " + currentBeat);
            }
            // Only warn the player if the current segment is not an uptime segment, but the next one is.
            if (IsNextSegmentUptime)
            {
                //Debug.Log($"Compare: {ElapsedTime}s / {gm.GetTimestampFromFourthLastBeatOfCurrentSegment()}s");
                if (gm.FlashingCountsRemaining < 1 && ElapsedTime >= gm.GetTimestampFromFourthLastBeatOfCurrentSegment())
                {
                    Debug.Log("[LEVEL] Warning player now.");
                    // Flash indicator.
                    gm.IsWarningPlayer = true;
                    gm.FlashingCountsRemaining = 4;
                }
            }

            // Beat flashing section.
            if (_isBeatFlashing)
            {
                _flashTime -= Time.deltaTime;
                if (_flashTime <= 0)
                {
                    _isBeatFlashing = false;
                    _flashTime = beatIndicatorFlashDuration;
                    beatIndicatorLeft.GetComponent<Image>().sprite = sprite_BeatOff;
                    beatIndicatorRight.GetComponent<Image>().sprite = sprite_BeatOff;
                }
            }
        }
    }
    /// <summary>
    /// Destroys the singleton instance of MusicManager.
    /// </summary>
    public void DestroyInstance() { Destroy(gameObject);  _musicManager = null; }

    public void SetAccuracies(double spb)
    {
        ThresholdPerfect = 0.125*spb;
        ThresholdGood = 0.25*spb;
        ThresholdOK = 0.3333*spb;
        ThresholdEarlyLate = 0.4*spb;
        ThresholdMiss = 1*spb;
    }

    public void PlayMusic()
    {
        audioSource.PlayScheduled(startTime + (secPerBeat * 5)); // Delay to allow the player to prepare for the first beat.
        //audioSource_Music.PlayScheduled(startTime + (secPerBeat * 5) + audioLatency); // Delay to allow the player to prepare for the first beat.
        Debug.Log("Audio source playing from local time: " + audioSource.time + "s.");
        AudioListener.pause = false;
    }

    public void ResumeMusic()
    {
        audioSource.UnPause();
        AudioListener.pause = false;
    }

    public void PauseMusic()
    {
        audioSource.Pause();
        AudioListener.pause = true;
    }

    public void StopMusic()
    {
        audioSource.Stop();
        //AudioListener.pause = true; // DO NOT PAUSE LISTENER WHEN LEAVING SCENE. ALL AUDIO GETS MUTED!!!
    }

    /// <summary>
    /// Checks the accuracy of the first arrow in <see cref="Instance"/>.
    /// </summary>
    /// <param name="earliestArrow"></param>
    /// <returns>
    /// Depending on the closeness to the beat, the returned <see cref="HitAccuracy"/> will vary.<br/>
    /// Check <see cref="SetAccuracies(double)"/>.
    /// </returns>
    public HitAccuracy CheckAccuracy(Arrow earliestArrow)
    {
        // Don't check accuracy if the arrow is too far away from the center.
        if (Vector3.Distance(earliestArrow.transform.position, earliestArrow.actualEndPos) > 1.5 * earliestArrow.speed * secPerBeat) return HitAccuracy.NA;

        Debug.Log($"Expected hit on {earliestArrow.timestamp}s, received on {ElapsedTime}s.");

        // To check accuracy, we need to find the time difference between the current time and the nearest beat time.
        double arrowTimestamp = earliestArrow.timestamp;
        double beatsPassed = ElapsedTime / secPerBeat; // Plus one as offset due to zero-based indexing.

        //// Find the nearest beat time.
        //double nearestBeatTime = Math.Round(beatsPassed) * secPerBeat;
        //// Calculate time difference between the current time and the nearest beat time.
        //double timeDifference = Math.Abs(elapsedTime - nearestBeatTime);

        double timeDifference = Math.Abs(ElapsedTime - earliestArrow.timestamp);

        if (timeDifference <= ThresholdPerfect) return HitAccuracy.PERFECT;
        if (timeDifference <= ThresholdGood) return HitAccuracy.GOOD;
        if (timeDifference <= ThresholdOK) return HitAccuracy.OK;
        if (timeDifference <= ThresholdEarlyLate) return ElapsedTime < arrowTimestamp ? HitAccuracy.EARLY : HitAccuracy.LATE;
        if (timeDifference <= ThresholdMiss) return HitAccuracy.MISS;

        return HitAccuracy.NA;
    }
}
