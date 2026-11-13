using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static Utility;
using static ComboClasses;

public class ComboManager : MonoBehaviour
{
    private static ComboManager _comboManager; // Singleton.
    public static ComboManager Instance => _comboManager;
    public ComboProcessor comboProcessor;

    //public event Action OnComboEntered; // Event triggered when a combo is entered.

    // Combo lines
    public GameObject comboLine1,
                      comboLine2,
                      comboLine3;
        public ComboLine comboLine1Component,
                          comboLine2Component,
                          comboLine3Component;

    public Sprite leftArrowSprite,
                  upArrowSprite,
                  downArrowSprite,
                  rightArrowSprite;

    public GameObject comboUIRegion;
        private GameObject[] comboIndicators;

    private Combo combo_EarnCurrency,
                  combo_HealPlayer;

    public bool TurretCombosActive = true; // Flag to enable or disable turret combo processing.

    /// <summary>
    /// A list containing the entered combo directions.
    /// </summary>
    public List<ComboHitAccuracy> EnteredCombo { get; } = new List<ComboHitAccuracy>(); // LEFT, RIGHT, UP, DOWN / DOWN, DOWN, DOWN, DOWN

    public float comboRevealDuration; // Duration for which the combo persists before removal.
        private float _comboRevealTimer = 0f;

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        if (comboRevealDuration == 0f) comboRevealDuration = 0.5f; // Default duration if not set in the inspector.
        if (comboUIRegion != null) comboIndicators = comboUIRegion.GetComponentsInChildren<Transform>().Select(t => t.gameObject).ToArray();

        if (comboLine1 != null) comboLine1Component = comboLine1.GetComponent<ComboLine>();
        if (comboLine2 != null) comboLine2Component = comboLine2.GetComponent<ComboLine>();
        if (comboLine3 != null) comboLine3Component = comboLine3.GetComponent<ComboLine>();

        // Load combos
        comboProcessor = new ComboProcessor();
        combo_EarnCurrency = new UC_EarnCurrency();
        combo_HealPlayer = new UC_HealPlayer();

        // By default:
        // Combo 1 makes money, combo 2 heals the player.
        comboLine1Component.ComboName = combo_EarnCurrency.ComboName;
        comboLine2Component.ComboName = combo_HealPlayer.ComboName;
        combo_EarnCurrency.ComboLine = comboLine1Component;
        combo_HealPlayer.ComboLine = comboLine2Component;

        // Subscribe to combo events
        comboProcessor.ComboEntered += combo_EarnCurrency.ComboFunction;
        comboProcessor.ComboEntered += combo_HealPlayer.ComboFunction;
        comboProcessor.OnNextFrame += combo_EarnCurrency.OnNextFrame;
        comboProcessor.OnNextFrame += combo_HealPlayer.OnNextFrame;

        comboLine3.SetActive(false); // Combo 3 is unassigned so hide it for now.
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (_comboManager == null)
        {
            _comboManager = this;
        }
        else
        {
            throw new InvalidOperationException("Combo manager is already instantiated.");
        }

        DontDestroyOnLoad(gameObject);
        ShowUserCombos(null);
    }

    // Update is called once per frame
    void Update()
    {
        comboProcessor.FireNextFrame();

        if (_comboRevealTimer > 0f)
        {
            _comboRevealTimer -= Time.deltaTime;
            if (_comboRevealTimer <= 0f)
            {
                ClearIndicator();
                EnteredCombo.Clear();
            }
        }
    }

    public void AddCombo(ArrowDirection arrow)
    {
        MusicManager mm = MusicManager.Instance;

        if (EnteredCombo.Count >= 4) return;

        // Get nearest beat timestamp to the current time.
        double nearestBeatTimestamp = Math.Round(mm.ElapsedTime / mm.secPerBeat + mm.audioLatency) * mm.secPerBeat;
        double offsetFromNearestBeat = mm.ElapsedTime - nearestBeatTimestamp;
        double absolutePercentageError = Math.Abs(offsetFromNearestBeat / mm.secPerBeat * 100);
        Debug.Log("Absolute Percentage error: " + absolutePercentageError + "%");

        ComboHitAccuracy comboArrow = new ComboHitAccuracy()
        {
            dir = arrow,
            acc = absolutePercentageError switch
            {
                < 10 => HitAccuracy.PERFECT, // +20% effect
                >= 10 and < 20  => HitAccuracy.GOOD, // +10% effect
                >= 20 and < 30 => HitAccuracy.OK, // +5% effect
                _ => HitAccuracy.MISS, // No bonus effect
            }
        };

        Debug.Log("[COMBO] Adding combo");
        EnteredCombo.Add(comboArrow);
        // Index is enteredCombo.Count to ignore first element, which refers to the parent.
        Image imgComp = comboIndicators[EnteredCombo.Count].AddComponent<Image>();

        imgComp.sprite = arrow switch
        {
            ArrowDirection.LEFT => leftArrowSprite,
            ArrowDirection.UP => upArrowSprite,
            ArrowDirection.DOWN => downArrowSprite,
            ArrowDirection.RIGHT => rightArrowSprite,
            _ => null
        };

        if (EnteredCombo.Count == 4)
        {
            _comboRevealTimer = comboRevealDuration;
            // Get each arrow's accuracy and calculate the total combo's multiplier.
            float totalMultiplier = 1f;
            foreach (var arrowFromEnteredCombo in EnteredCombo)
            {
                totalMultiplier *= arrowFromEnteredCombo.acc switch
                {
                    HitAccuracy.PERFECT => 1.2f,
                    HitAccuracy.GOOD => 1.1f,
                    HitAccuracy.OK => 1.05f,
                    _ => 1f
                };
            }

            Debug.Log("[COMBO] Total multiplier: " + totalMultiplier);

            // Check entered combo and execute a function below.
            comboProcessor.CheckAllCombos(Combo.Integerify(EnteredCombo[0].dir, EnteredCombo[1].dir, EnteredCombo[2].dir, EnteredCombo[3].dir), totalMultiplier);
        }
    }

    public void ClearIndicator()
    {
        foreach (GameObject pos in comboIndicators)
        {
            Image img = pos.GetComponent<Image>();
            if (img != null) Destroy(img);
            Debug.Log("[COMBO] Clearing Image");
        }
    }

    public void ShowUserCombos(GameObject oldTurretObject)
    {
        Turret oldTurret = oldTurretObject != null ? oldTurretObject?.GetComponent<Turret>() : null;

        if (!comboLine1.activeInHierarchy) comboLine1.SetActive(true);
        comboLine1Component.ChangeDisplay(combo_EarnCurrency.ComboName, combo_EarnCurrency.ArrowSequence);
        if (combo_EarnCurrency.CooldownTimer > 0f) comboLine1Component.DarkenArrows();
        else comboLine1Component.LightenArrows();
        combo_EarnCurrency.IsActive = true;

        if (!comboLine2.activeInHierarchy) comboLine2.SetActive(true);
        comboLine2Component.ChangeDisplay(combo_HealPlayer.ComboName, combo_HealPlayer.ArrowSequence);
        if (combo_HealPlayer.CooldownTimer > 0f) comboLine2Component.DarkenArrows();
        else comboLine2Component.LightenArrows();
        combo_HealPlayer.IsActive = true;

        if (oldTurretObject == null) return;

        oldTurret.comboOne.IsActive = false;
        oldTurret.comboTwo.IsActive = false;

        //if (!comboLine3.activeInHierarchy) comboLine3.SetActive(true);
    }

    public void ShowSpecificTurretComboStatuses(GameObject turretObject)
    {
        Debug.Log("[TURRET] Clicked on turret: " + turretObject.name);
        Turret targetTurret = turretObject.GetComponent<Turret>();

        if (targetTurret.comboOne != null && comboLine1 != null)
        {
            combo_EarnCurrency.IsActive = false;
            targetTurret.comboOne.IsActive = true;
            if (!comboLine1.activeInHierarchy) comboLine1.SetActive(true);
            comboLine1Component.ChangeDisplay(targetTurret.comboOne.ComboName, targetTurret.comboOne.ArrowSequence);

            if (targetTurret.comboOne.CooldownTimer > 0f) comboLine1Component.DarkenArrows();
            else comboLine1Component.LightenArrows();
        }
        else
        {
            if (comboLine1.activeInHierarchy) comboLine1.SetActive(false); // Hide combo 1 area if there are no combos.
        }
        if (targetTurret.comboTwo != null && comboLine2 != null)
        {
            combo_HealPlayer.IsActive = false;
            targetTurret.comboTwo.IsActive = true;
            if (!comboLine2.activeInHierarchy) comboLine2.SetActive(true);
            comboLine2Component.ChangeDisplay(targetTurret.comboTwo.ComboName, targetTurret.comboTwo.ArrowSequence);

            if (targetTurret.comboTwo.CooldownTimer > 0f) comboLine2Component.DarkenArrows();
            else comboLine2Component.LightenArrows();
        }
        else
        {
            if (comboLine2.activeInHierarchy) comboLine2.SetActive(false); // Hide combo 2 area if there are no combos.
        }
        if (targetTurret.comboThree != null && comboLine3 != null)
        {
            targetTurret.comboThree.IsActive = true;
            if (!comboLine3.activeInHierarchy) comboLine3.SetActive(true);
            comboLine3Component.ChangeDisplay(targetTurret.comboThree.ComboName, targetTurret.comboThree.ArrowSequence);

            if (targetTurret.comboThree.CooldownTimer > 0f) comboLine3Component.DarkenArrows();
            else comboLine3Component.LightenArrows();
        }
        else
        {
            if (comboLine3.activeInHierarchy) comboLine3.SetActive(false); // Hide combo 3 area if there are no combos.
        }
    }

    /// <summary>
    /// Destroys the singleton instance of the ComboManager.
    /// </summary>
    public void DestroyInstance() { Destroy(gameObject); _comboManager = null; }
}
