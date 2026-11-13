using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class SetBossHealth : MonoBehaviour
{

    public static SetBossHealth Instance;
    //public TextMeshProUGUI TextElement;
    public int BossHealth;
    public GameObject PanelPrefab;
    public Transform Canvas;
    public int level;
    public TextMeshProUGUI Timer;
    public float time;
    public int Clear = 0;

    private void Awake()
    {
        Instance = this;
        //TextElement.text = "Boss Health: " + BossHealth;
    }

    public void TakeDamage(int Damage)
    {
        //BossHealth -= Damage;
        //TextElement.text = "Boss Health: " + BossHealth;
        if (BossHealth <= 0)
        {
            if (TutorialManager.Instance == null) ShowVictoryPanel();
        }
    }

    public void ShowVictoryPanel()
    {
        Time.timeScale = 0.0f;
        if (Keyboard.current != null)
        {
            InputSystem.DisableDevice(Keyboard.current);
        }
        if (MusicManager.Instance != null) MusicManager.Instance.PauseMusic();
        GameManager gm = GameManager.Instance; 
        time = float.Parse(Timer.text);
        Clear = 1;
        if (!SetPlayerHealth.Instance.Takendamage) Clear = 2;
        if (!SetPlayerHealth.Instance.Takendamage && gm.hitMiss == 0) Clear = 3;
        if (!SetPlayerHealth.Instance.Takendamage && gm.hitMiss == 0 && gm.hitEarly == 0 && gm.hitLate == 0 && gm.hitOk == 0 && gm.hitGood == 0) Clear = 4;
        if (level == 1)
        {
            if (PlayerPrefs.GetInt("Progress") < 4)
            {
                PlayerPrefs.SetInt("Progress", 4);
            }
            if (PlayerPrefs.GetFloat("Level1time") > time || PlayerPrefs.GetFloat("Level1time") == 0) PlayerPrefs.SetFloat("Level1time", time);
            if (PlayerPrefs.GetInt("Level1ClearState") < Clear) PlayerPrefs.SetInt("Level1ClearState", Clear);
            PlayerPrefs.Save();
        }
        if (level == 2)
        {
            if (PlayerPrefs.GetInt("Progress") < 6)
            {
                PlayerPrefs.SetInt("Progress", 6);
            }
            if (PlayerPrefs.GetFloat("Level2time") > time || PlayerPrefs.GetFloat("Level2time") == 0) PlayerPrefs.SetFloat("Level2time", time);
            if (PlayerPrefs.GetInt("Level2ClearState") < Clear) PlayerPrefs.SetInt("Level2ClearState", Clear);
            PlayerPrefs.Save();
        }
        if (level == 3)
        {
            if (PlayerPrefs.GetInt("Progress") < 8)
            {
                PlayerPrefs.SetInt("Progress", 8);
            }
            if (PlayerPrefs.GetFloat("Level3time") > time || PlayerPrefs.GetFloat("Level3time") == 0) PlayerPrefs.SetFloat("Level3time", time);
            if (PlayerPrefs.GetInt("Level3ClearState") < Clear) PlayerPrefs.SetInt("Level3ClearState", Clear);
            PlayerPrefs.Save();
        }
        Instantiate(PanelPrefab, Canvas, false);
    }
}
