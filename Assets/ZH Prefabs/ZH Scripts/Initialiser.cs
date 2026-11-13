using UnityEngine;
using UnityEngine.InputSystem;

public class Initialiser : MonoBehaviour
{
    public InputActionAsset inputActions;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(!PlayerPrefs.HasKey("FirstTimeSetup"))
        {
            PlayerPrefs.SetInt("Progress", 0);
            PlayerPrefs.SetFloat("MasterVolume", 1.0f);
            PlayerPrefs.SetFloat("MusicVolume", 1.0f);
            PlayerPrefs.SetFloat("EffectsVolume", 1.0f);
            if (PlayerPrefs.HasKey("Keybinds"))
            {
                PlayerPrefs.DeleteKey("Keybinds");
                inputActions.RemoveAllBindingOverrides();
            }
            PlayerPrefs.SetInt("FirstTimeSetup", 1);
            PlayerPrefs.SetInt("Delay", 0);
            PlayerPrefs.SetFloat("Level1time", 0.0f);
            PlayerPrefs.SetInt("Level1ClearState", 0);
            PlayerPrefs.SetFloat("Level2time", 0.0f);
            PlayerPrefs.SetInt("Level2ClearState", 0);
            PlayerPrefs.SetFloat("Level3time", 0.0f);
            PlayerPrefs.SetInt("Level3ClearState", 0);
            PlayerPrefs.Save();
        }
    }

}
