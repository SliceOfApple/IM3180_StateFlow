using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SetPlayerHealth : MonoBehaviour
{
    public static SetPlayerHealth _instance;
    public static SetPlayerHealth Instance => _instance;

    public int MaxPlayerHealth;
    public int PlayerHealth;

    public GameObject PanelPrefab;
    public GameObject playerHealthBarObject;
        private Slider playerHealthBar;

    public Transform Canvas;
    public bool Takendamage=false;
    public Animator animator;

    private void Awake()
    {
        Time.timeScale = 1.0f;
        if (Keyboard.current != null)
        {
            InputSystem.EnableDevice(Keyboard.current);
        }
        _instance = this;
        Debug.Log($"[HEALTH] Set player health to {PlayerHealth}.");

        PlayerHealth = MaxPlayerHealth;
        if (playerHealthBarObject != null) playerHealthBar = playerHealthBarObject.GetComponent<Slider>();
        if (playerHealthBar != null)
        {
            playerHealthBar.maxValue = MaxPlayerHealth;
            playerHealthBar.value = PlayerHealth;
        }
    }

    void Update()
    {
        if (Keyboard.current.backspaceKey.wasPressedThisFrame)
        {
            TakeDamage(10);
        }

        // Let the health bar fluctuate like a real-world counter.
        if (playerHealthBar != null && PlayerHealth > 0f)
        { 
            playerHealthBar.value = Mathf.Clamp(PlayerHealth + Random.Range(-0.5f, 0.5f), 0, MaxPlayerHealth);
        }
    }

    public void TakeDamage(int Damage)
    {
        PlayerHealth = GameManager.Instance.IsTutorial ? Mathf.Clamp(PlayerHealth - Damage, 1, MaxPlayerHealth) : PlayerHealth - Damage;
        Takendamage = true;
        if (playerHealthBar != null) playerHealthBar.value = PlayerHealth;
        Debug.Log($"[HEALTH] Player took {Damage} damage, health is now {PlayerHealth}.");
        if (PlayerHealth <= 0.3f * MaxPlayerHealth) animator.SetBool("LowHealth", true);

        if (PlayerHealth <= 0)
        {
            if (TutorialManager.Instance == null)
            {
                Instantiate(PanelPrefab, Canvas, false);
                Time.timeScale = 0.0f;
                if (Keyboard.current != null)
                {
                    InputSystem.DisableDevice(Keyboard.current);
                }

                if (MusicManager.Instance != null) MusicManager.Instance.PauseMusic();
            }
            else TutorialManager.Instance.ContinueTutorial();
        }
    }

    public void HealPlayer(int HealAmount)
    {
        PlayerHealth = Mathf.Clamp(PlayerHealth + HealAmount, 0, MaxPlayerHealth);
        if (playerHealthBar != null) playerHealthBar.value = PlayerHealth;
        Debug.Log($"[HEALTH] Player healed for {HealAmount} health.");
        if (PlayerHealth >= 0.3f * MaxPlayerHealth) animator.SetBool("LowHealth", false);
    }
}