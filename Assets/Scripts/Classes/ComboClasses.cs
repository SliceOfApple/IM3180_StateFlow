using UnityEngine;
using static Utility;

/// <summary>
/// Stuff related to combos.
/// </summary>
public class ComboClasses
{
    public enum UserComboId
    {
        [InspectorName("None")]
        None = 1000,
        [InspectorName("USER: Earn Currency")]
        UC_EarnCurrency,
        [InspectorName("USER: Heal")]
        UC_HealPlayer
    }

    public enum TurretComboId
    {
        [InspectorName("None")]
        None = 2000,
        [InspectorName("TURRET: Increased Firerate")]
        TC_IncreasedFirerate,
        [InspectorName("TURRET: Repair")]
        TC_Repair,
        [InspectorName("TURRET: Shield Invulnerable")]
        TC_ShieldInvulnerable
    }

    /// <summary>
    /// Class with delegate <see cref="ComboHandler"/> and event <see cref="ComboEntered"/> to handle combo execution.
    /// </summary>
    public class ComboProcessor
    {
        public delegate void ComboHandler(int combo, float multiplier);
        public delegate void OnNextFrameHandler();
        public event ComboHandler ComboEntered;
        public event OnNextFrameHandler OnNextFrame;

        public void CheckAllCombos(int combo, float multiplier) => OnComboEntered(combo, multiplier);
        public void FireNextFrame() => OnNextFrameEvent();

        protected virtual void OnComboEntered(int combo, float multiplier) => ComboEntered?.Invoke(combo, multiplier);
        protected virtual void OnNextFrameEvent() => OnNextFrame?.Invoke();

        public static void GetCombo(TurretComboId id, out TurretCombo combo)
        {
            switch (id)
            {
                case TurretComboId.TC_IncreasedFirerate:
                    combo = new TC_IncreasedFirerate();
                    break;
                case TurretComboId.TC_Repair:
                    combo = new TC_Repair();
                    break;
                case TurretComboId.TC_ShieldInvulnerable:
                    combo = new TC_ShieldInvulnerable();
                    break;
                default:
                    combo = null;
                    break;
            }
        }
    }

    /// <summary>
    /// Base class for all combos. Contains properties and methods that define a combo's behavior and characteristics.
    /// </summary>
    public abstract class Combo
    {
        /// <summary>
        /// A flag to indicate if the combo should be shown on UI.<br/><br/>
        /// If this is a <b>turret combo</b>, it will be shown if an instance of that turret is selected by the player.<br/>
        /// Otherwise, if it is a <b>user combo</b>, it will be shown while the player has not selected a turret.
        /// </summary>
        public bool IsActive = false;
        /// <summary>
        /// Name of the combo, used for display purposes.
        /// </summary>
        public abstract string ComboName { get; }
        /// <summary>
        /// Description of combo.
        /// </summary>
        public abstract string ComboDescription { get; }
        /// <summary>
        /// Cooldown in seconds the combo is placed on, once it is used.
        /// </summary>
        public abstract int Cooldown { get; }
        /// <summary>
        /// The UI area displaying the combo and its cooldown.
        /// </summary>
        public ComboLine ComboLine { get; set; }
        /// <summary>
        /// Active time left in seconds until the combo can be used again.
        /// </summary>
        public float CooldownTimer = 0;
        /// <summary>
        /// The sequence to execute the combo.
        /// </summary>
        public abstract ArrowDirection[] ArrowSequence { get; }
        /// <summary>
        /// The function that runs when the combo is executed. This is where the combo's effects are defined.
        /// </summary>
        public virtual void ComboFunction(int combo, float multiplier)
        {
            Debug.Log("[COMBO] Combo executed: " + ComboName);
            Debug.Log("[COMBO] Accuracy bonus: " + multiplier + "x");
        }
        public virtual void OnNextFrame()
        {
            if (CooldownTimer > 0)
            {
                CooldownTimer -= Time.deltaTime;
                if (this.IsActive) ComboLine.UpdateComboName(CooldownTimer / Cooldown);
            }
            else
            {
                ComboLine.ComboNameText.color = Color.green;
            }
        }

        /// <summary>
        /// Converts a combo into a unique integer representation for comparison.
        /// </summary>
        /// <param name="a">First arrow.</param>
        /// <param name="b">Second arrow.</param>
        /// <param name="c">Third arrow.</param>
        /// <param name="d">Fourth arrow.</param>
        /// <returns>An 8-bit <b>integer</b> representation of the combo.</returns>
        public static int Integerify(ArrowDirection a, ArrowDirection b, ArrowDirection c, ArrowDirection d)
        {
            return ((int)a << 6) |
                   ((int)b << 4) |
                   ((int)c << 2) |
                   (int)d;
        }
    }

    /// <summary>
    /// Base class for all turret combos.
    /// </summary>
    public abstract class TurretCombo : Combo
    {
        public Turret turret;
        public int EffectDuration;
        public float EffectDurationLeft;
    }

    //#region User-based combos
    public class UC_EarnCurrency : Combo
    {
        public override string ComboName { get; } = "Pocket Change";
        public override string ComboDescription { get; } = "Credits 50 currency units when executed.";
        public override int Cooldown { get; } = 10;
        //public float CooldownTimer { get; set; }
        public override ArrowDirection[] ArrowSequence { get; } = new ArrowDirection[4] { ArrowDirection.DOWN, ArrowDirection.DOWN, ArrowDirection.DOWN, ArrowDirection.DOWN };
        /// <summary>
        /// On instantiation, this property is calculated to be a comparable integer representation of the ArrowSequence. This is used for comparison purposes in the ComboManager.
        /// </summary>
        public int ArrowSequenceComparable { get; set; }

        /// <summary>
        /// Gives the player at least 50 currency units when executed.<br/><br/>
        /// Combo: <b>DOWN</b>, <b>DOWN</b>, <b>DOWN</b>, <b>DOWN</b><br/>
        /// </summary>
        public UC_EarnCurrency()
        {
            CooldownTimer = 0f;
            ArrowSequenceComparable = Integerify(ArrowSequence[0], ArrowSequence[1], ArrowSequence[2], ArrowSequence[3]);

            //ComboManager.Instance.OnComboEntered += ComboFunction;
        }

        public override void ComboFunction(int combo, float multiplier)
        {
            if (CooldownTimer > 0f) return;
            if (combo != ArrowSequenceComparable) return;
            SetCurrency.Instance.ChangeCurrency((int)(50 * multiplier)); // Credits base of 50 credits, up to 178 credits.
            CooldownTimer = Cooldown;
            base.ComboFunction(combo, multiplier);
        }
    }

    /// <summary>
    /// Heals the player by at least 25 HP.<br/><br/>
    /// Combo: <b>DOWN</b>, <b>UP</b>, <b>UP</b>, <b>UP</b><br/>
    /// </summary>
    public class UC_HealPlayer : Combo
    {
        public override string ComboName { get; } = "Maintenance";
        public override string ComboDescription { get; } = "Heals the player for 25 HP when executed.";
        public override int Cooldown { get; } = 30;
        public override ArrowDirection[] ArrowSequence { get; } = new ArrowDirection[4] { ArrowDirection.DOWN, ArrowDirection.UP, ArrowDirection.UP, ArrowDirection.UP };
        public int ArrowSequenceComparable { get; set; }
        public UC_HealPlayer()
        {
            CooldownTimer = 0f;
            ArrowSequenceComparable = Integerify(ArrowSequence[0], ArrowSequence[1], ArrowSequence[2], ArrowSequence[3]);
        }
        public override void ComboFunction(int combo, float multiplier)
        {
            if (CooldownTimer > 0f) return;
            if (combo != ArrowSequenceComparable) return;
            SetPlayerHealth.Instance.HealPlayer((int)(25 * multiplier)); // Heals base of 25 hp, up to ~89 hp.
            CooldownTimer = Cooldown;
            base.ComboFunction(combo, multiplier);
        }
    }
    //#endregion

    //#region Turret-based combos
    public class TC_IncreasedFirerate : TurretCombo
    {
        public override string ComboName { get; } = "Overdrive";
        public override string ComboDescription { get; } = "Increases turret fire rate for 10 seconds when executed.";
        public override int Cooldown { get; } = 45;
        public override ArrowDirection[] ArrowSequence { get; } = new ArrowDirection[4] { ArrowDirection.UP, ArrowDirection.UP, ArrowDirection.RIGHT, ArrowDirection.RIGHT };
        public int ArrowSequenceComparable { get; set; }

        public float multiplier; // To store the multiplier for use in OnNextFrame.
        public float oldRepeatValue; // To store the old repeat value for use in OnNextFrame.

        public TC_IncreasedFirerate()
        {
            CooldownTimer = 0f;
            EffectDuration = 10;
            ArrowSequenceComparable = Integerify(ArrowSequence[0], ArrowSequence[1], ArrowSequence[2], ArrowSequence[3]);
        }

        public override void ComboFunction(int combo, float multiplier)
        {
            if (CooldownTimer > 0f) return;
            if (combo != ArrowSequenceComparable) return;

            // Combo Logic: Implement the logic to increase turret fire rate for 15 seconds
            oldRepeatValue = turret.GetFireRate();
            turret.SetFireRate(oldRepeatValue / (1.25f * multiplier));
            this.multiplier = multiplier;

            EffectDurationLeft = EffectDuration;
            CooldownTimer = Cooldown;
            base.ComboFunction(combo, multiplier);
        }

        public override void OnNextFrame()
        {
            if (CooldownTimer > 0)
            {
                EffectDurationLeft -= Time.deltaTime;
                if (EffectDurationLeft <= 0)
                {
                    // Reset the turret fire rate to normal after the effect duration ends
                    turret.SetFireRate(oldRepeatValue);
                    EffectDurationLeft = 0;
                }
            }
            base.OnNextFrame();
        }
    }

    public class TC_Repair : TurretCombo
    {
        public override string ComboName { get; } = "Repair";
        public override string ComboDescription { get; } = "Repairs the turret for 25 HP.";
        public override int Cooldown { get; } = 30;
        public override ArrowDirection[] ArrowSequence { get; } = new ArrowDirection[4] { ArrowDirection.UP, ArrowDirection.UP, ArrowDirection.LEFT, ArrowDirection.LEFT };
        public int ArrowSequenceComparable { get; set; }

        public float multiplier;

        public TC_Repair()
        {
            CooldownTimer = 0f;
            ArrowSequenceComparable = Integerify(ArrowSequence[0], ArrowSequence[1], ArrowSequence[2], ArrowSequence[3]);
        }

        public override void ComboFunction(int combo, float multiplier)
        {
            if (CooldownTimer > 0f) return;
            if (combo != ArrowSequenceComparable) return;

            // Combo Logic: Repair the turret
            turret.Heal(3 * multiplier); // Repairs base of 3 hp, up to 6 hp.

            CooldownTimer = Cooldown;
            base.ComboFunction(combo, multiplier);
        }
    }

    public class TC_ShieldInvulnerable : TurretCombo
    {
        public override string ComboName { get; } = "Shield Overclock";
        public override string ComboDescription { get; } = "Makes the turret invulnerable for 5 seconds.";
        public override int Cooldown { get; } = 45;
        public override ArrowDirection[] ArrowSequence { get; } = new ArrowDirection[4] { ArrowDirection.UP, ArrowDirection.LEFT, ArrowDirection.RIGHT, ArrowDirection.LEFT };
        public int ArrowSequenceComparable { get; set; }

        public Shielding shield;

        public float multiplier;

        public TC_ShieldInvulnerable()
        {
            CooldownTimer = 0f;
            EffectDuration = 5;
            ArrowSequenceComparable = Integerify(ArrowSequence[0], ArrowSequence[1], ArrowSequence[2], ArrowSequence[3]);
        }

        public override void ComboFunction(int combo, float multiplier)
        {
            if (CooldownTimer > 0f) return;
            if (combo != ArrowSequenceComparable) return;

            // Combo Logic: Implement the logic to make the turret invulnerable for 10 seconds
            shield = turret.GetComponentInChildren<Shielding>();
            if (shield.transform.childCount == 0) shield.ForceGenerateShield();
            shield.shieldInstance.GetComponent<Turret>().isInvulnerable = true;
            EffectDurationLeft = EffectDuration;

            CooldownTimer = Cooldown;
            base.ComboFunction(combo, multiplier);
        }

        public override void OnNextFrame()
        {
            if (EffectDurationLeft > 0) EffectDurationLeft -= Time.deltaTime;
            if (EffectDurationLeft <= 0)
            {
                // Make shield vulnerable again.
                if (shield != null) shield.shieldInstance.GetComponent<Turret>().isInvulnerable = false;
                EffectDurationLeft = 0;
                shield = null;
            }
            base.OnNextFrame();
        }
    }
    //#endregion
}
