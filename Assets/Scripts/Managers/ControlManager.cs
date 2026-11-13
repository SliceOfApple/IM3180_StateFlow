using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static Utility;

/// <summary>
/// Game input control manager. A <b>singleton</b>, only one instance of this class can exist at any time.<br/>
/// Refer to the singleton with <see cref="Instance"/>.
/// </summary>
public class ControlManager : MonoBehaviour
{
    private static ControlManager _controlManager; // Singleton.
    public static ControlManager Instance => _controlManager;

    // Rhythm control
    private InputAction comboToggleButton,
                        rhythmUp,
                        rhythmDown,
                        rhythmLeft,
                        rhythmRight,
                        space,
                        enter;

    public AudioClip sfxComboLeft,
                     sfxComboUp,
                     sfxComboRight,
                     sfxComboDown,
                     sfxComboHit;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (_controlManager == null)
        {
            _controlManager = this;
        }
        else
        {
            throw new InvalidOperationException("Control manager is already instantiated.");
        }

        comboToggleButton = InputSystem.actions.FindAction("ComboToggle");
        rhythmUp = InputSystem.actions.FindAction("RhythmUp");
        rhythmDown = InputSystem.actions.FindAction("RhythmDown");
        rhythmLeft = InputSystem.actions.FindAction("RhythmLeft");
        rhythmRight = InputSystem.actions.FindAction("RhythmRight");
        space = InputSystem.actions.FindAction("Jump");
        enter = InputSystem.actions.FindAction("Attack");

        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (comboToggleButton.WasPressedThisFrame()) Debug.Log("!!! Combo toggle button held !!!");
        if (comboToggleButton.WasReleasedThisFrame()) Debug.Log("!!! Combo toggle button released !!!");

        // For combo inputs.
        if (comboToggleButton.IsPressed())
        {
            if (rhythmUp.WasPressedThisFrame() && Boss.Instance.shouldBringBossIntoView)
            {
                ComboManager.Instance.AddCombo(ArrowDirection.UP);
                //if (sfxComboUp != null) MusicManager.Instance.audioSource_Music.PlayOneShot(sfxComboUp);
                if (sfxComboHit != null) MusicManager.Instance.audioSourceSFX.PlayOneShot(sfxComboHit);
                Debug.Log("Combo up");
            }
            if (rhythmDown.WasPressedThisFrame() && Boss.Instance.shouldBringBossIntoView)
            {
                ComboManager.Instance.AddCombo(ArrowDirection.DOWN);
                //if (sfxComboDown != null) MusicManager.Instance.audioSource_Music.PlayOneShot(sfxComboDown);
                if (sfxComboHit != null) MusicManager.Instance.audioSourceSFX.PlayOneShot(sfxComboHit);
                Debug.Log("Combo down");
            }
            if (rhythmLeft.WasPressedThisFrame() && Boss.Instance.shouldBringBossIntoView)
            {
                ComboManager.Instance.AddCombo(ArrowDirection.LEFT);
                //if (sfxComboLeft != null) MusicManager.Instance.audioSource_Music.PlayOneShot(sfxComboLeft);
                if (sfxComboHit != null) MusicManager.Instance.audioSourceSFX.PlayOneShot(sfxComboHit);
                Debug.Log("Combo left");
            }
            if (rhythmRight.WasPressedThisFrame() && Boss.Instance.shouldBringBossIntoView)
            {
                ComboManager.Instance.AddCombo(ArrowDirection.RIGHT);
                //if (sfxComboRight != null) MusicManager.Instance.audioSource_Music.PlayOneShot(sfxComboRight);
                if (sfxComboHit != null) MusicManager.Instance.audioSourceSFX.PlayOneShot(sfxComboHit);
                Debug.Log("Combo right");
            }
        }
        else
        {
            GameManager gm = GameManager.Instance;

            if (rhythmUp.WasPressedThisFrame())
            {
                // Check lane 1.
                if (gm.arrows_up.Count > 0 || gm.IsInUptime)
                {
                    //HitAccuracy accuracy = MusicManager.instance.CheckAccuracy();

                    GameManager.Instance.CheckArrow(ArrowDirection.UP);
                    //Arrow earliestArrow = GameManager.instance.arrows_up[0].GetComponent<Arrow>();
                    //HitAccuracy accuracy = earliestArrow.CheckAccuracy();
                    //if (accuracy != HitAccuracy.NA && accuracy != HitAccuracy.MISS)
                    //{
                    //    GameManager.instance.arrows_up.RemoveAt(0);
                    //    Destroy(earliestArrow.gameObject);
                    //}
                    Debug.Log("Blocking up");
                }
            }
            if (rhythmDown.WasPressedThisFrame())
            {            
                if (gm.arrows_down.Count > 0 || gm.IsInUptime)
                {
                    GameManager.Instance.CheckArrow(ArrowDirection.DOWN);
                    // Check lane 2.
                    //Arrow earliestArrow = GameManager.instance.arrows_down[0].GetComponent<Arrow>();
                    //HitAccuracy accuracy = earliestArrow.CheckAccuracy();
                    //if (accuracy != HitAccuracy.NA && accuracy != HitAccuracy.MISS)
                    //{
                    //    GameManager.instance.arrows_down.RemoveAt(0);
                    //    Destroy(earliestArrow.gameObject);
                    //}
                    Debug.Log("Blocking down");
                }
            }
            if (rhythmLeft.WasPressedThisFrame())
            {
                if (gm.arrows_left.Count > 0 || gm.IsInUptime)
                {
                    GameManager.Instance.CheckArrow(ArrowDirection.LEFT);
                    // Check lane 3.
                    //Arrow earliestArrow = GameManager.instance.arrows_left[0].GetComponent<Arrow>();
                    //HitAccuracy accuracy = earliestArrow.CheckAccuracy();
                    //if (accuracy != HitAccuracy.NA && accuracy != HitAccuracy.MISS)
                    //{
                    //    GameManager.instance.arrows_left.RemoveAt(0);
                    //    Destroy(earliestArrow.gameObject);
                    //}
                    Debug.Log("Blocking left");
                }
            }
            if (rhythmRight.WasPressedThisFrame())
            {
                if (gm.arrows_right.Count > 0 || gm.IsInUptime)
                {
                    GameManager.Instance.CheckArrow(ArrowDirection.RIGHT);
                    // Check lane 4.
                    //Arrow earliestArrow = GameManager.instance.arrows_right[0].GetComponent<Arrow>();
                    //HitAccuracy accuracy = earliestArrow.CheckAccuracy();
                    //if (accuracy != HitAccuracy.NA && accuracy != HitAccuracy.MISS)
                    //{
                    //    GameManager.instance.arrows_right.RemoveAt(0);
                    //    Destroy(earliestArrow.gameObject);
                    //}
                    Debug.Log("Blocking right");
                }
            }
        }
    }

    /// <summary>
    /// Destroys the singleton instance of ControlManager.
    /// </summary>
    public void DestroyInstance() { Destroy(gameObject); _controlManager = null; }
}