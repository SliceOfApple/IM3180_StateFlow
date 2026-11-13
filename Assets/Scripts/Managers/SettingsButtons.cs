using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;

public class SettingsButtons : MonoBehaviour
{
    public GameObject PanelPrefab;
    public Transform Canvas;
    public int delay;
    public TextMeshProUGUI delayText;
    public GameObject syncpanel;
    public GameObject ConfirmationPanel;
    public GameObject FadeToBlack;
    public float fadeDuration = 0.5f;
    public InputActionAsset inputActions;

    private void Awake()
    {
        Transform childTransform = GameObject.Find("HighestGameElementCanvas").transform.Find("FadeToBlack");
        if (childTransform != null)
        {
            FadeToBlack = childTransform.gameObject;
        }
    }

    public void GoToKeyBinds()
    {
        Instantiate(PanelPrefab, Canvas, false);
        Time.timeScale = 0.0f;
    }
    public void GoToConfirmation()
    {
        ConfirmationPanel.SetActive(true);
    }
    public void CancelConfirmation()
    {
        ConfirmationPanel.SetActive(false);
    }
    public void CloseMenu()
    {
        Time.timeScale = 1.0f;
        if (Keyboard.current != null)
        {
            InputSystem.EnableDevice(Keyboard.current);
        }
        if (MusicManager.Instance != null) MusicManager.Instance.ResumeMusic();
        Destroy(gameObject.transform.parent.gameObject);
    }
    public void MainMenu()
    {
        {
            if (Keyboard.current != null)
            {
                InputSystem.EnableDevice(Keyboard.current);
            }
            if (GameManager.Instance != null) GameManager.Instance.DestroyInstance();
            if (MusicManager.Instance != null)
            {
                MusicManager.Instance.StopMusic();
                MusicManager.Instance.DestroyInstance();
            }
            if (ControlManager.Instance != null) ControlManager.Instance.DestroyInstance();
            if (ComboManager.Instance != null) ComboManager.Instance.DestroyInstance();
            if (Boss.Instance != null) Boss.Instance.DestroyInstance();
            Time.timeScale = 1;

            StartCoroutine(FadeAndWaitGeneral("Title Screen"));
        }
    }
    public void LevelSelect()
    {
        {
            if (Keyboard.current != null)
            {
                InputSystem.EnableDevice(Keyboard.current);
            }
            if (GameManager.Instance != null) GameManager.Instance.DestroyInstance();
            if (MusicManager.Instance != null)
            {
                MusicManager.Instance.StopMusic();
                MusicManager.Instance.DestroyInstance();
            }
            if (ControlManager.Instance != null) ControlManager.Instance.DestroyInstance();
            if (ComboManager.Instance != null) ComboManager.Instance.DestroyInstance();
            if (Boss.Instance != null) Boss.Instance.DestroyInstance();
            Time.timeScale = 1;

            StartCoroutine(FadeAndWaitGeneral("LevelSelect"));
        }
    }
    public void LoadNextScene()
    {
        if (GameManager.Instance != null) GameManager.Instance.DestroyInstance();
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.StopMusic();
            MusicManager.Instance.DestroyInstance();
        }
        if (Keyboard.current != null)
        {
            InputSystem.EnableDevice(Keyboard.current);
        }
        if (ControlManager.Instance != null) ControlManager.Instance.DestroyInstance();
        if (ComboManager.Instance != null) ComboManager.Instance.DestroyInstance();
        if (Boss.Instance != null) Boss.Instance.DestroyInstance();
        Time.timeScale = 1;
        StartCoroutine(FadeAndWaitGeneralIndex((SceneManager.GetActiveScene().buildIndex + 1)));
    }
    public void ResetData()
    {
        Time.timeScale = 1.0f;
        PlayerPrefs.DeleteKey("FirstTimeSetup");
        if (Keyboard.current != null)
        {
            InputSystem.EnableDevice(Keyboard.current);
        }
        PlayerPrefs.Save();
        StartCoroutine(FadeAndWaitGeneral(SceneManager.GetActiveScene().name));
    }

    public void Restart()
    {
        if (GameManager.Instance != null) GameManager.Instance.DestroyInstance();
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.StopMusic();
            MusicManager.Instance.DestroyInstance();
        }
        if (Keyboard.current != null)
        {
            InputSystem.EnableDevice(Keyboard.current);
        }
        if (ControlManager.Instance != null) ControlManager.Instance.DestroyInstance();
        if (ComboManager.Instance != null) ComboManager.Instance.DestroyInstance();
        if (Boss.Instance != null) Boss.Instance.DestroyInstance();
        Time.timeScale = 1;
        Debug.LogError(SceneManager.GetActiveScene().name);
        StartCoroutine(FadeAndWaitGeneral(SceneManager.GetActiveScene().name));
    }

    public void sync()
    {
        if (Keyboard.current != null)
        {
            InputSystem.EnableDevice(Keyboard.current);
        }
        Time.timeScale = 1.0f;
        GameObject panel = Instantiate(syncpanel, Canvas, false);
        AudioSyncing listen = panel.GetComponentInChildren<AudioSyncing>();
        listen.Destroyed += updatepanel;
    }

    private void updatepanel()
    {
        delay = PlayerPrefs.GetInt("Delay", 0);
        delayText.text = delay.ToString();
    }

    public void add1()
    { AdjustDelay(1); }
    public void add10()
    { AdjustDelay(10); }
    public void add100()
    { AdjustDelay(100); }
    public void sub1()
    { AdjustDelay(-1); }
    public void sub10()
    { AdjustDelay(-10); }
    public void sub100()
    { AdjustDelay(-100); }

    public void AdjustDelay(int change)
    {
        if ((delay+change) < 10000 && (delay + change) > -10000)
        {
            delay = PlayerPrefs.GetInt("Delay", 0);
            delay += change;
            delayText.text = delay.ToString();
            PlayerPrefs.SetInt("Delay", delay);
            PlayerPrefs.Save();
        }
    }

    private IEnumerator FadeAndWaitGeneral(string Level)
    {
        FadeToBlack.SetActive(true);
        // 2. Pause this specific function while the game continues running
        yield return new WaitForSeconds(fadeDuration);
        if (!PlayerPrefs.HasKey("Keybinds"))
        {
            PlayerPrefs.SetString("Keybinds", inputActions.SaveBindingOverridesAsJson());
        }
        else { inputActions.LoadBindingOverridesFromJson(PlayerPrefs.GetString("Keybinds")); }
        SceneManager.LoadScene(Level);
    }

    private IEnumerator FadeAndWaitGeneralIndex(int index)
    {
        FadeToBlack.SetActive(true);
        // 2. Pause this specific function while the game continues running
        yield return new WaitForSeconds(fadeDuration);
        if (!PlayerPrefs.HasKey("Keybinds"))
        {
            PlayerPrefs.SetString("Keybinds", inputActions.SaveBindingOverridesAsJson());
        }
        else { inputActions.LoadBindingOverridesFromJson(PlayerPrefs.GetString("Keybinds")); }
        SceneManager.LoadScene(index);
    }
}
