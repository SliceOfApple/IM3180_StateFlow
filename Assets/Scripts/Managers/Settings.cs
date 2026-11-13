using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Settings : MonoBehaviour
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

        if (Keyboard.current != null)
        {
            InputSystem.DisableDevice(Keyboard.current);
        }

        if (MusicManager.Instance!=null) MusicManager.Instance.PauseMusic();
    }
}
