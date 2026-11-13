using UnityEngine;
using static ComboClasses;

/// <summary>
/// A script that all Turret types have.
/// </summary>
public class Turret : MonoBehaviour
{
    // Max health for the turret
    public int health;
        private int _health; // Current turret health
    public GameObject explode;
    public bool isInvulnerable = false;

    public TurretComboId id_firstCombo;
    public TurretComboId id_secondCombo;
    public TurretComboId id_thirdCombo;

    public TurretCombo comboOne;
    public TurretCombo comboTwo;
    public TurretCombo comboThree;
    private Animator animator;

    private Shooting shootingScript;

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        if (!ComboManager.Instance.TurretCombosActive) return;

        shootingScript = GetComponentInChildren<Shooting>();

        if (id_firstCombo != TurretComboId.None) ComboProcessor.GetCombo(id_firstCombo, out comboOne);
        animator = GetComponent<Animator>();
        if (comboOne != null)
        {
            comboOne.turret = this;
            comboOne.ComboLine = ComboManager.Instance.comboLine1Component;
            ComboManager.Instance.comboProcessor.ComboEntered += comboOne.ComboFunction;
            ComboManager.Instance.comboProcessor.OnNextFrame += comboOne.OnNextFrame;
        }
            
        if (id_secondCombo != TurretComboId.None) ComboProcessor.GetCombo(id_secondCombo, out comboTwo);
        if (comboTwo != null)
        {
            comboTwo.turret = this;
            comboTwo.ComboLine = ComboManager.Instance.comboLine2Component;
            ComboManager.Instance.comboProcessor.ComboEntered += comboTwo.ComboFunction;
            ComboManager.Instance.comboProcessor.OnNextFrame += comboTwo.OnNextFrame;
        }

        if (id_thirdCombo != TurretComboId.None) ComboProcessor.GetCombo(id_thirdCombo, out comboThree);
        if (comboThree != null)
        {
            comboThree.turret = this;
            comboThree.ComboLine = ComboManager.Instance.comboLine3Component;
            ComboManager.Instance.comboProcessor.ComboEntered += comboThree.ComboFunction;
            ComboManager.Instance.comboProcessor.OnNextFrame += comboThree.OnNextFrame;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _health = health;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        if (isInvulnerable) return;

        _health -= damage;
        animator.SetTrigger("Taking Damage");
        if (_health <= 0)
        {
            if (gameObject.transform.parent != null)
            {
                GameObject Tile = gameObject.transform.parent.gameObject;
                Tile.tag = "Place";
                int newLayer = LayerMask.NameToLayer("Tile");
                Tile.gameObject.layer = newLayer;
            }

            Instantiate(explode, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    public void TakeDamageNoAni(int damage)
    {
        if (isInvulnerable) return;

        _health -= damage;
        if (_health <= 0)
        {
            if (gameObject.transform.parent != null)
            {
                GameObject Tile = gameObject.transform.parent.gameObject;
                Tile.tag = "Place";
                int newLayer = LayerMask.NameToLayer("Tile");
                Tile.gameObject.layer = newLayer;
            }

            Instantiate(explode, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    public void Heal(float amount)
    {
        float newHealthAmount = _health + amount;
        _health = (int) Mathf.Clamp(newHealthAmount, newHealthAmount, health);
    }

    public float GetFireRate() => shootingScript.repeatRate;
    public void SetFireRate(float value) => shootingScript.repeatRate = value;
}
