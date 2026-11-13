using UnityEngine;
using UnityEngine.InputSystem;

public class MainMenuSettings : MonoBehaviour
{
    public GameObject PanelPrefab;
    public Transform Canvas;
    public InputActionAsset inputActions;
    public void GoToSettings()
    {
        if (!PlayerPrefs.HasKey("Keybinds"))
        {
            PlayerPrefs.SetString("Keybinds", inputActions.SaveBindingOverridesAsJson());
        }
        else { inputActions.LoadBindingOverridesFromJson(PlayerPrefs.GetString("Keybinds")); }
        Instantiate(PanelPrefab, Canvas, false);
        Time.timeScale = 0.0f;
        if (MusicManager.Instance != null) MusicManager.Instance.PauseMusic();
    }

}
