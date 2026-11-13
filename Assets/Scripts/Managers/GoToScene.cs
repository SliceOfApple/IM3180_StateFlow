using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;

public class GoToScene : MonoBehaviour
{
    private static GoToScene _goToScene; // Singleton.
    public static GoToScene Instance => _goToScene;
    public InputActionAsset inputActions;
    public GameObject FadeToBlack;
    public float fadeDuration = 0.5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void LevelSelect()
    {
        FadeToBlack.SetActive(true);
        if (!PlayerPrefs.HasKey("Keybinds"))
        {
            PlayerPrefs.SetString("Keybinds", inputActions.SaveBindingOverridesAsJson());
        }
        else { inputActions.LoadBindingOverridesFromJson(PlayerPrefs.GetString("Keybinds")); }
        if (PlayerPrefs.GetInt("Progress", 0) > 0)
        {
            LevelSelectScene();
        }
        else Intro();
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
    public void TitleScreen()
    {
        clearInstances();
        StartCoroutine(FadeAndWaitGeneral("Title Screen"));
    }

    public void LevelSelectScene()
    {
        clearInstances();
        FadeToBlack.SetActive(true);
        StartCoroutine(FadeAndWaitGeneral("LevelSelect"));
    }
    public void Intro()
    {
        clearInstances();
        FadeToBlack.SetActive(true);
        stopMusic();
        StartCoroutine(FadeAndWaitGeneral("Intro"));
    }
    public void Tutorial()
    {
        FadeToBlack.SetActive(true);
        if (PlayerPrefs.GetInt("Progress") < 1)
        {
            PlayerPrefs.SetInt("Progress", 1);
            PlayerPrefs.Save();
        }
        clearInstances();
        stopMusic();
        StartCoroutine(FadeAndWaitGeneral("Tutorial"));
    }
    public void Story1()
    {
        FadeToBlack.SetActive(true);
        if (PlayerPrefs.GetInt("Progress") < 2)
        {
            PlayerPrefs.SetInt("Progress", 2);
            PlayerPrefs.Save();
        }
        clearInstances();
        stopMusic();
        StartCoroutine(FadeAndWaitGeneral("Cutscene 1"));
    }
    public void Level1()
    {
        FadeToBlack.SetActive(true);
        if (PlayerPrefs.GetInt("Progress") < 3)
        {
            PlayerPrefs.SetInt("Progress", 3);
            PlayerPrefs.Save();
        }
        clearInstances();
        stopMusic();
        StartCoroutine(FadeAndWaitGeneral("Level 1"));
    }
    public void Story2()
    {
        FadeToBlack.SetActive(true);
        if (PlayerPrefs.GetInt("Progress") < 4)
        {
            PlayerPrefs.SetInt("Progress", 4);
            PlayerPrefs.Save();
        }
        clearInstances();
        stopMusic();
        StartCoroutine(FadeAndWaitGeneral("Cutscene 2"));
    }
    public void Level2()
    {
        FadeToBlack.SetActive(true);
        if (PlayerPrefs.GetInt("Progress") < 5)
        {
            PlayerPrefs.SetInt("Progress", 5);
            PlayerPrefs.Save();
        }
        clearInstances();
        stopMusic();
        StartCoroutine(FadeAndWaitGeneral("Level 2"));
    }
    public void Story3()
    {
        FadeToBlack.SetActive(true);
        if (PlayerPrefs.GetInt("Progress") < 6)
        {
            PlayerPrefs.SetInt("Progress", 6);
            PlayerPrefs.Save();
        }
        clearInstances();
        stopMusic();
        StartCoroutine(FadeAndWaitGeneral("Cutscene 3"));
    }
    public void Level3()
    {
        FadeToBlack.SetActive(true);
        if (PlayerPrefs.GetInt("Progress") < 7)
        {
            PlayerPrefs.SetInt("Progress", 7);
            PlayerPrefs.Save();
        }
        clearInstances();
        stopMusic();
        StartCoroutine(FadeAndWaitGeneral("Level 3"));
    }
    public void Conclusion()
    {
        FadeToBlack.SetActive(true);
        if (PlayerPrefs.GetInt("Progress") < 8)
        {
            PlayerPrefs.SetInt("Progress", 8);
            PlayerPrefs.Save();
        }
        clearInstances();
        stopMusic();
        StartCoroutine(FadeAndWaitGeneral("Conclusion"));
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void stopMusic()
    {
        if (TitleScreenMusicManager.Instance != null) TitleScreenMusicManager.Instance.SoftenAndStopMusic();
    }

    public void clearInstances()
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
    }

    public void PlayMouseDownSFX()
    {
        TitleScreenMusicManager tsmm = TitleScreenMusicManager.Instance;
        if (tsmm != null && tsmm.sfxMouseDown != null) tsmm.audioSource_Music.PlayOneShot(tsmm.sfxMouseDown, 0.375f);
    }

    public void PlayMouseUpSFX()
    {
        TitleScreenMusicManager tsmm = TitleScreenMusicManager.Instance;
        if (tsmm != null && tsmm.sfxMouseUp != null) tsmm.audioSource_Music.PlayOneShot(tsmm.sfxMouseUp, 0.375f);
    }
}
