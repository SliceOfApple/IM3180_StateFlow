using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static Utility;

/// <summary>
/// Manages the level state, including the beatmap, arrows, and player accuracy. A <b>singleton</b>, only one instance of this class can exist at any time.<br/>
/// Refer to the singleton with <see cref="Instance"/>.
/// </summary>
public class GameManager : MonoBehaviour
{
    private static GameManager _gameManager; // Singleton.
    public static GameManager Instance => _gameManager;

    public int DamageOnPerfect;

    public TMP_Text hitIndicator;

    private Beatmap currentLevelBeatmap;
   
    // Arrow block AudioClips
    public AudioClip sfxBlockPerfect,
                     sfxBlockGood,
                     sfxBlockOk,
                     sfxBlockBad,
                     sfxBlockMiss;

    // Prefabs
    public GameObject leftArrowPrefab,
                      upArrowPrefab,
                      downArrowPrefab,
                      rightArrowPrefab;
    public GameObject accuracyTextPrefab;

    // UI Indicators
    public GameObject beatIndicator1, beatIndicator2;
    public GameObject incomingIndicator;
    public GameObject bossAttackBackgroundCover;
        private float bossAttackBackgroundMaxOpacity = 225f/255f;
    private GameObject UICanvas;
        private SpriteRenderer[] perfectArrowIndicators;

    // Arrow lanes and positions
    public GameObject lanes;
    public Transform[] lanePositions;

    // Game states
    public bool IsTutorial;
    public bool IsGamePaused = false;
    public bool IsInUptime = false;
    public bool IsMusicSeekingNext = false;
    public bool IsWarningPlayer = false;
    public int FlashingCountsRemaining = 0;

    // Active arrows created.
    public List<GameObject> arrows_left,
                            arrows_up,
                            arrows_down,
                            arrows_right;

    // Beatmap data.
    public string pathToBeatmap = string.Empty;

    public List<Segment> segments = new List<Segment>();
    public List<Beat> beats = new List<Beat>();
    public float LengthOfLoadedBeats 
    { 
        get 
        { 
            var totalLength = 0f;
            foreach (Beat beat in beats)
                totalLength += (float) beat.length;

            return totalLength;
        } 
    }

    // Variables for music looping.
    public double NextSegmentTimestamp { get; private set; } = 0;
    public double timestamp_beginningOfSegment { get; private set;  } = 0;
    public double NextEventTimestamp { get; private set; } = 0;
    public double SeekTimestamp { get; private set; } = 0;
    public double SeekTo = 0;
    public int oldSegmentSelector = 0, segmentSelector = 0;

    // Player accuracy stats.
    public int hitMiss = 0,
               hitEarly = 0,
               hitLate = 0,
               hitOk = 0,
               hitGood = 0,
               hitPerfect = 0;
    public int comboCount = 0;
    public int totalBeatsSent = 0;

    // Debugging variables
    public int arrowsHit = 0;
    public float totalDelay = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (_gameManager == null)
        {
            _gameManager = this;
        }
        else
        {
            throw new InvalidOperationException("Game manager is already instantiated.");
        }

        lanePositions = lanes.GetComponentsInChildren<Transform>(); // Index 0 Transform is the parent itself. Ignore it.
        List<Arrow> arrows_left = new List<Arrow>();
        List<Arrow> arrows_up = new List<Arrow>();
        List<Arrow> arrows_down = new List<Arrow>();
        List<Arrow> arrows_right = new List<Arrow>();

        DontDestroyOnLoad(gameObject);

        if (pathToBeatmap.Equals(string.Empty)) throw new Exception("Beatmap not set.");
        LoadBeatmap(Path.Join(Application.streamingAssetsPath, pathToBeatmap));

        UICanvas = GameObject.Find("GameElementCanvas");
        if (UICanvas != null) perfectArrowIndicators = UICanvas.GetComponentsInChildren<SpriteRenderer>();

        //LoadBeatmap("Assets/Beatmaps/BeethovenVirus.xml");
    }

    // Update is called once per frame
    void Update()
    {
        // Background-fading portion for boss attacks.
        Image background = bossAttackBackgroundCover.GetComponent<Image>();
        Color oldColor = background.color;

        Debug.Log(segments[segmentSelector].isUptime ? "Segment is uptime" : "Segment is not uptime");

        if (segments[segmentSelector].isUptime)
        {

            if (!background.enabled) 
            {
                SpawnAlien.Instance.FreezeMinions();
                background.enabled = true;
            }
            if (IsWarningPlayer && (background.color.a < bossAttackBackgroundMaxOpacity || perfectArrowIndicators[0].color.a < 1f))
            {
                Debug.Log("Fade background");
                //background.color = new Color(0f, 0f, 0f, Math.Clamp(oldColor.a + 1f * Time.deltaTime, 0f, bossAttackBackgroundMaxOpacity));
                background.color = new Color(0f, 0f, 0f, Math.Clamp(oldColor.a + 1f * Time.deltaTime, 0f, bossAttackBackgroundMaxOpacity));
                
                foreach (SpriteRenderer indicator in perfectArrowIndicators)
                {
                    Color oldIndicatorColor = indicator.color;
                    indicator.color = new Color(oldIndicatorColor.r, 
                                                oldIndicatorColor.g, 
                                                oldIndicatorColor.b, 
                                                Math.Clamp(oldIndicatorColor.a + 2f * Time.deltaTime, 0f, 1f));
                }
            }
        }
        else if (!segments[segmentSelector].isUptime)
        {
            if (background.color.a > 0 || perfectArrowIndicators[0].color.a > 0)
            {
                Debug.Log("Unfade background");
                //background.color = new Color(0f, 0f, 0f, Math.Clamp(oldColor.a + 1f * Time.deltaTime, 0f, bossAttackBackgroundMaxOpacity));
                background.color = new Color(0f, 0f, 0f, Math.Clamp(oldColor.a - 1f * Time.deltaTime, 0f, bossAttackBackgroundMaxOpacity));
                foreach (SpriteRenderer indicator in perfectArrowIndicators)
                {
                    Color oldIndicatorColor = indicator.color;
                    indicator.color = new Color(oldIndicatorColor.r, 
                                                oldIndicatorColor.g, 
                                                oldIndicatorColor.b, 
                                                Math.Clamp(oldIndicatorColor.a - 0.5f * Time.deltaTime, 0f, 1f));
                }
            }
            if (background.enabled && background.color.a <= 0f && perfectArrowIndicators[0].color.a <= 0f)
            {
                SpawnAlien.Instance.UnfreezeMinions();
                background.enabled = false;
                IsInUptime = false;
            }
        }
    }

    /// <summary>
    /// Destroys the singleton instance of GameManager.
    /// </summary>
    public void DestroyInstance() { Destroy(gameObject);  _gameManager = null; }

    /// <summary>
    /// Updates the player's scoreboard of different accuracies hit, and reflects the latest accuracy with a temporary floating text.
    /// This is a function so that <see cref="Arrow"/> can also update the scoreboard on a miss.
    /// </summary>
    /// <param name="acc"></param>
    public void UpdateScore(HitAccuracy acc, ArrowDirection dir, Vector3 pos)
    {
        GameObject accIndObj = Instantiate(accuracyTextPrefab, UICanvas.transform);
        TMP_Text accIndText = accIndObj.GetComponent<TMP_Text>();
        Vector3 arrowCoord = new Vector3(pos.x, pos.y, Camera.main.nearClipPlane);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            UICanvas.GetComponent<RectTransform>(),
            arrowCoord, 
            null, 
            out Vector2 localPoint
        );

        accIndObj.GetComponent<RectTransform>().anchoredPosition = localPoint;

        switch (acc)
        {
            case HitAccuracy.PERFECT:
                hitPerfect += 1;
                accIndText.text = "Perfect!!";
                accIndText.color = Color.hotPink;
                SetCurrency.Instance.ChangeCurrency(3);
                Boss.Instance.TakeDamage(DamageOnPerfect);
                if (sfxBlockPerfect != null) MusicManager.Instance.audioSourceSFX.PlayOneShot(sfxBlockPerfect, 0.5f);
                break;
            case HitAccuracy.GOOD:
                hitGood += 1;
                accIndText.text = "Good!";
                accIndText.color = Color.lightSeaGreen;
                SetCurrency.Instance.ChangeCurrency(2);
                if (sfxBlockGood != null) MusicManager.Instance.audioSourceSFX.PlayOneShot(sfxBlockGood, 0.5f);
                break;
            case HitAccuracy.OK:
                hitOk += 1;
                accIndText.text = "OK...";
                accIndText.color = Color.darkGreen;
                SetCurrency.Instance.ChangeCurrency(1);
                if (sfxBlockOk != null) MusicManager.Instance.audioSourceSFX.PlayOneShot(sfxBlockOk, 0.5f);
                break;
            case HitAccuracy.EARLY:
                hitEarly += 1;
                accIndText.text = "Early";
                accIndText.color = Color.orangeRed;
                if (sfxBlockBad != null) MusicManager.Instance.audioSourceSFX.PlayOneShot(sfxBlockBad, 0.5f);
                SetPlayerHealth.Instance.TakeDamage(1);
                break;
            case HitAccuracy.LATE:
                hitLate += 1;
                accIndText.text = "Late";
                accIndText.color = Color.orangeRed;
                if (sfxBlockBad != null) MusicManager.Instance.audioSourceSFX.PlayOneShot(sfxBlockBad, 0.5f);
                SetPlayerHealth.Instance.TakeDamage(1);
                break;
            case HitAccuracy.MISS:
                hitMiss += 1;
                accIndText.text = "! MISS !";
                accIndText.color = Color.firebrick;
                if (sfxBlockMiss != null) MusicManager.Instance.audioSourceSFX.PlayOneShot(sfxBlockMiss, 0.25f);
                SetPlayerHealth.Instance.TakeDamage(2);
                break;
        }

        // Increase combo count for successful Perfect, Good and OK hits.
        // Reset combo count for Early and Late hits and Misses.
        if ((int) acc < 3) comboCount += 1;
        else comboCount = 0;
    }

    public void CheckArrow(ArrowDirection dir)
    {
        // Check appropriate lane.
        List<GameObject> laneToCheck = null;
        laneToCheck = dir switch
        {
            ArrowDirection.LEFT => arrows_left,
            ArrowDirection.RIGHT => arrows_right,
            ArrowDirection.UP => arrows_up,
            ArrowDirection.DOWN => arrows_down,
            _ => throw new ArgumentOutOfRangeException(nameof(dir), $"Invalid arrow direction: {dir}") // This should never happen.
        };
        //if (dir == ArrowDirection.LEFT) laneToCheck = arrows_left;
        //if (dir == ArrowDirection.RIGHT) laneToCheck = arrows_right;
        //if (dir == ArrowDirection.UP) laneToCheck = arrows_up;
        //if (dir == ArrowDirection.DOWN) laneToCheck = arrows_down;

        if (laneToCheck.Count == 0) return; // No arrows in this lane to check.

        Arrow earliestArrow = laneToCheck[0].GetComponent<Arrow>();

        HitAccuracy accuracy = MusicManager.Instance.CheckAccuracy(earliestArrow);

        var absoluteDelay = MusicManager.Instance.ElapsedTime - earliestArrow.timestamp;
        totalDelay += (float)absoluteDelay;
        arrowsHit += 1;
        Debug.Log($"[ACCURACY] Average delay: {totalDelay / arrowsHit}");

        if (accuracy != HitAccuracy.NA)
        {
            GameObject accIndObj = Instantiate(accuracyTextPrefab);
            TMP_Text accIndText = accIndObj.GetComponent<TMP_Text>();

            laneToCheck.RemoveAt(0);

            UpdateScore(accuracy, dir, earliestArrow.gameObject.transform.position);
            Destroy(earliestArrow.gameObject);
        }
    }

    public void ToggleGame() => IsGamePaused = !IsGamePaused;

    public void SpawnArrow(double perfectHitTimestamp, ArrowDirection dir)
    {
        //int ArrowUUID = UnityEngine.Random.Range(100000, 999999);
        //Debug.Log($"Arrow function {ArrowUUID} called at " + MusicManager.Instance.ElapsedTime);
        int random = dir > 0 ? (int) dir : UnityEngine.Random.Range(1, 5);

        Debug.Log(random);

        // TODO - Replace this with better sprites instead of wack shapes.
        switch (random)
        {
            case 1:
                //Debug.Log($"Arrow {ArrowUUID} pre-spawn at " + MusicManager.Instance.ElapsedTime);
                GameObject leftArrowObj = Instantiate(leftArrowPrefab, lanePositions[1]);
                Arrow leftArrow = leftArrowObj.GetComponent<Arrow>();
                leftArrow.arrowDirection = ArrowDirection.LEFT;
                leftArrow.timestamp = perfectHitTimestamp;
                arrows_left.Add(leftArrowObj);
                //Debug.Log($"Arrow {ArrowUUID} spawned at " + MusicManager.Instance.ElapsedTime);
                break;
            case 2:
                //Debug.Log($"Arrow {ArrowUUID} pre-spawn at " + MusicManager.Instance.ElapsedTime);
                GameObject upArrowObj = Instantiate(upArrowPrefab, lanePositions[2]);
                Arrow upArrow = upArrowObj.GetComponent<Arrow>();
                upArrow.arrowDirection = ArrowDirection.UP;
                upArrow.timestamp = perfectHitTimestamp;
                arrows_up.Add(upArrowObj);
                //Debug.Log($"Arrow {ArrowUUID} spawned at " + MusicManager.Instance.ElapsedTime);
                break;
            case 3:
                //Debug.Log($"Arrow {ArrowUUID} pre-spawn at " + MusicManager.Instance.ElapsedTime);
                GameObject downArrowObj = Instantiate(downArrowPrefab, lanePositions[3]);
                Arrow downArrow = downArrowObj.GetComponent<Arrow>();
                downArrow.arrowDirection = ArrowDirection.DOWN;
                downArrow.timestamp = perfectHitTimestamp;
                arrows_down.Add(downArrowObj);
                //Debug.Log($"Arrow {ArrowUUID} spawned at " + MusicManager.Instance.ElapsedTime);
                break;
            case 4:
                //Debug.Log($"Arrow {ArrowUUID} pre-spawn at " + MusicManager.Instance.ElapsedTime);
                GameObject rightArrowObj = Instantiate(rightArrowPrefab, lanePositions[4]);
                Arrow rightArrow = rightArrowObj.GetComponent<Arrow>();
                rightArrow.arrowDirection = ArrowDirection.RIGHT;
                rightArrow.timestamp = perfectHitTimestamp;
                arrows_right.Add(rightArrowObj);
                //Debug.Log($"Arrow {ArrowUUID} spawned at " + MusicManager.Instance.ElapsedTime);
                break;
        }
        totalBeatsSent++;
        //Instantiate(arrowPrefab, lanePositions[UnityEngine.Random.Range(1, 5)]);
    }

    public void LoadBeatmap(string path)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(Beatmap));

        using (StreamReader reader = new StreamReader(path))
        {
            currentLevelBeatmap = (Beatmap)serializer.Deserialize(reader);

            int tempo = currentLevelBeatmap.SongProperty.tempo;
            var loadedSegments = currentLevelBeatmap.Structure.Segments;

            Debug.Log($@"Beatmap ""{currentLevelBeatmap.name}"" loaded.
                PROPERTY: Tempo - {tempo}; Pickup Beats - {currentLevelBeatmap.SongProperty.pickup}
                No. of Segments - {loadedSegments.Count}
            ");

            var spb = 60f / tempo;
            MusicManager.Instance.BPM = tempo;
            MusicManager.Instance.secPerBeat = spb;
            MusicManager.Instance.SetAccuracies(spb);

            foreach (Segment segment in loadedSegments)
            {
                Debug.Log($@"Segment ""{segment.name}"" 
                    isUptime: {segment.isUptime}
                    No. of Beats: {segment.Beats.Count}
                ");

                Debug.Log(segments.Count);
                segment.TotalLength = segment.Beats.Sum(beat => beat.length);

                segments.Add(segment);
            }
        }

        //currentLevelBeatmap.Load(path);

        //foreach (XmlNode node in currentLevelBeatmap.DocumentElement.ChildNodes)
        //{
        //    if ("#comment".Equals(node.Name)) continue;
        //    Debug.Log("Exploring parent node " + node.Name);

        //    foreach (XmlNode innerNode in node.ChildNodes)
        //    {
        //        if ("#comment".Equals(innerNode.Name)) continue;
        //        Debug.Log("Child node: " + innerNode.Name);
        //    }
        //    //if (node.Name == "Audio")
        //    //{
        //    //    audioDelay = float.Parse(node.Attributes["delay"].Value);
        //    //}
        //}
    }

    public void StartLevel()
    {
        MusicManager mm = MusicManager.Instance;
        beats.AddRange(segments[0].Beats);

        // If the first segment is an uptime, then load its beats. Otherwise, just advance timestamp.
        if (segments[0].isUptime)
        {
            FlashingCountsRemaining = 4;
            IsWarningPlayer = true;
            //mm.IsNextSegmentUptime = true;
        }
        double spb = mm.secPerBeat;
        //NextEventTimestamp = spb*2 - 0.5 /*0.05*/ + MusicManager.Instance.audioLatency;
        NextEventTimestamp = spb + mm.flatDelay + mm.audioLatency;
        timestamp_beginningOfSegment = NextEventTimestamp;
        NextSegmentTimestamp = NextEventTimestamp;
        mm.PlayMusic();
    }

    public void LoadBeatsFromSegmentIndex() { beats.AddRange(segments[segmentSelector].Beats); Debug.Log("Added beats: " +segments[segmentSelector].Beats.Count); }

    public void SetSeekTimeFromSegmentIndex()
    {
        double spb = MusicManager.Instance.secPerBeat;
        double timeToSeek = 0d;

        for (int i = 0; i < segmentSelector; i++)
        {
            timeToSeek += segments[i].TotalLength * spb;
        }
        //Debug.Log("Seek to " + timeToSeek + "s.");
        SeekTo = timeToSeek;
    }

    /// <summary>
    /// Fired on every beat from <see cref="MusicManager"/>.
    /// </summary>
    public void NextBeat()
    {
        //if (IsGamePaused) return;
        //if (beatIndicator1 == null || beatIndicator2 == null) return;

        //// TODO -- Replace these cheap ass indicators with something proper. Current implementation is just for testing purposes.
        //var ind1 = beatIndicator1.GetComponent<Image>();
        //var ind2 = beatIndicator2.GetComponent<Image>();
        //ind1.color = new Color(ind1.color.r, ind1.color.g, ind1.color.b, 1);
        //ind2.color = new Color(ind2.color.r, ind2.color.g, ind2.color.b, 1);
    }

    public void NextEvent()
    {
        double elapsedTime = MusicManager.Instance.ElapsedTime;
        double spb = MusicManager.Instance.secPerBeat;

        if (beats.Count == 0)
        {
            Debug.Log("[Level] No beats in level timeline.");
            //Debug.Log($"No beats found. Advancing segment to index {segmentSelector}.");
            //segmentSelector++;
            //LoadBeatsFromSegmentIndex();

            return;
        }

        Beat currentBeat = beats[0];
        string levelEvent = currentBeat.levelEvent ?? string.Empty;
        //if (levelEvent.Equals("seek")) SeekMusicToTarget(currentBeat.target);
        Debug.Log("[Level] Current segment is uptime? " + segments[segmentSelector].isUptime);
        if (currentBeat.type.Equals("note") && segments[segmentSelector].isUptime) SpawnArrow(NextEventTimestamp + 4 * spb, currentBeat.dir);

        NextEventTimestamp += currentBeat.length * spb;

        //if (currentBeat.type.Equals("note"))
        //{
        //    if (segments[segmentSelector].isUptime) SpawnArrow(NextEventTimestamp + 4*spb, currentBeat.dir);

        //    NextEventTimestamp += currentBeat.length * spb;

        //    //if (levelEvent.Equals("jump"))
        //    //{
        //    //    string segmentName = currentBeat.target;
        //    //    int targetSegmentIndex = -1;

        //    //    for (int i = 0; i < segments.Count; i++)
        //    //    {
        //    //        if (segments[i].name.Equals(segmentName))
        //    //        {
        //    //            Debug.Log($"Found segment {segments[i].name} to jump to.");
        //    //            targetSegmentIndex = i;
        //    //            break;
        //    //        }
        //    //    }
        //    //    // Do not use LINQ, it adds unnecessary overhead.
        //    //    //int targetSegmentIndex = segments.FindIndex(segment => segment.name.Equals(segmentName));

        //    //    segmentSelector = targetSegmentIndex;
        //    //    LoadBeatsFromSegmentIndex();
        //    //    SetSeekTimeFromSegmentIndex();
        //    //    SeekTimestamp = NextEventTimestamp + 4 * spb;
        //    //    IsMusicSeekingNext = true;
        //    //    //Debug.Log("Jumping to segment index " + targetSegmentIndex);
        //    //}
        //    //Debug.Log($"Spawned arrow on {NextEventTimestamp}s, perfect at {NextEventTimestamp + 4 * spb}s.");
        //    //NextEventTimestamp = elapsedTime + currentBeat.length * spb + audioDelay;
        //}
        //else
        //{
        //    NextEventTimestamp += currentBeat.length * spb;
        //    //NextEventTimestamp = elapsedTime + currentBeat.length * spb + audioDelay;
        //    Debug.Log($"Rest for {currentBeat.length} SPBs.");
        //}

        if (levelEvent.Equals("jump"))
        {
            string segmentName = currentBeat.target;
            int targetSegmentIndex = -1;

            for (int i = 0; i < segments.Count; i++)
            {
                if (segments[i].name.Equals(segmentName))
                {
                    Debug.Log($"[EVENT] Found segment '{segments[i].name}' to jump to.");
                    targetSegmentIndex = i;
                    break;
                }
            }
            // Do not use LINQ, it adds unnecessary overhead.
            //int targetSegmentIndex = segments.FindIndex(segment => segment.name.Equals(segmentName));

            oldSegmentSelector = segmentSelector;
            segmentSelector = targetSegmentIndex;
            LoadBeatsFromSegmentIndex();
            SetSeekTimeFromSegmentIndex();
            SeekTimestamp = NextEventTimestamp + 4 * spb;
            IsMusicSeekingNext = true;
            //MusicManager.Instance.IsNextSegmentUptime = !segments[oldSegmentSelector].isUptime && segments[segmentSelector].isUptime;
            //Debug.Log("Jumping to segment index " + targetSegmentIndex);
        }
        Debug.Log("[EVENT] Finished spawn. Next event: " + NextEventTimestamp);

        beats.RemoveAt(0);

        // BUG: There is a flaw here. If there is only one beat in the current segment, and the next segment is an uptime,
        // the game will still show the arrow blocker interface.
        if (beats.Count == 0)
        {
            // If there are no more beats in the current segment, load the next segment's beats into the beat list.
            oldSegmentSelector = segmentSelector;
            segmentSelector++;
            Debug.Log($"[EVENT] No beats found. Advancing segment to index {segmentSelector}.");
            if (!segments[oldSegmentSelector].isUptime && segments[segmentSelector].isUptime)
                Debug.Log($"[EVENT] Flash for next segment.");
            else
                Debug.Log($"[EVENT] Ignoring next segment flash.");
            //MusicManager.Instance.IsNextSegmentUptime = !segments[oldSegmentSelector].isUptime && segments[segmentSelector].isUptime;
            //Debug.Log($"[EVENT] To flash: {MusicManager.Instance.IsNextSegmentUptime}");
            LoadBeatsFromSegmentIndex();
        }
    }

    /// <summary>
    /// (Long-ass function name. Sorry, not sorry.)<br/><br/>
    /// Calculates the total length of beats <u>from the first segment until the current segment</u> indicated by <see cref="segmentSelector"/>, then returns the timestamp of the fourth last beat of the current segment.
    /// </summary>
    /// <returns>The timestamp the fourth last beat of the current segment occurs on.</returns>
    public double GetTimestampFromFourthLastBeatOfCurrentSegment()
    {
        double spb = MusicManager.Instance.secPerBeat;
        double totalBeatLength = 0d;

        //for (int i = 0; i <= segmentSelector; i++)
        //{
        //    totalBeatLength += segments[i].TotalLength;
        //}
        // Get the length of all beats loaded from the current segment, then subtract by 4.
        for (int i = 0; i < beats.Count; i++)
            totalBeatLength += beats[i].length;
        //for (int i = 0; i < segments[segmentSelector].Beats.Count; i++)
        //    totalBeatLength += segments[segmentSelector].Beats[i].length;

        //Debug.Log("Calculated total beat length: " + totalBeatLength);

        //int offset = currentLevelBeatmap.SongProperty.beatsPerBar >= 4 ? currentLevelBeatmap.SongProperty.beatsPerBar : 4;
        //double beatSecondLength = (totalBeatLength - currentLevelBeatmap.SongProperty.beatsPerBar) * spb;
        totalBeatLength -= currentLevelBeatmap.SongProperty.beatsPerBar;

        //return (totalBeatLength - currentLevelBeatmap.SongProperty.beatsPerBar) * spb;
        return MusicManager.Instance.ElapsedTime + totalBeatLength * spb;
        //return timestamp_beginningOfSegment + totalBeatLength * spb;
    }

    public void UpdateNextSegmentTimestamp()
    {
        timestamp_beginningOfSegment = NextEventTimestamp;
        NextSegmentTimestamp += segments[segmentSelector].TotalLength * MusicManager.Instance.secPerBeat;
        Debug.Log($"[LEVEL] Advancing NextSegmentTimestamp to {NextSegmentTimestamp}s.");
        Debug.Log($"[LEVEL] Advancing beginningOfSegment checkpoint to {timestamp_beginningOfSegment}s.");
    }
}