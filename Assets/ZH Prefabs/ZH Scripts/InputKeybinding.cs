using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputKeybinding : MonoBehaviour
{
    public TextMeshProUGUI Up;
    public TextMeshProUGUI Down;
    public TextMeshProUGUI Left;
    public TextMeshProUGUI Right;
    public TextMeshProUGUI Combo;
    public InputActionAsset inputActions;
    private InputActionRebindingExtensions.RebindingOperation rebindOperation;
    public GameObject blockingOverlay;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inputActions.LoadBindingOverridesFromJson(PlayerPrefs.GetString("Keybinds"));
        InputAction UpInput = inputActions.FindAction("RhythmUp");
        InputAction DownInput = inputActions.FindAction("RhythmDown");
        InputAction LeftInput = inputActions.FindAction("RhythmLeft");
        InputAction RightInput = inputActions.FindAction("RhythmRight");
        InputAction ComboInput = inputActions.FindAction("ComboToggle");
        Up.text = UpInput.GetBindingDisplayString(0);
        Down.text = DownInput.GetBindingDisplayString(0);
        Left.text = LeftInput.GetBindingDisplayString(0);
        Right.text = RightInput.GetBindingDisplayString(0);
        Combo.text = ComboInput.GetBindingDisplayString(0);
        if (Keyboard.current != null)
        {
            InputSystem.EnableDevice(Keyboard.current);
        }
    }

    public InputAction StartInteractiveRebind(string actionName)
    {
        blockingOverlay.SetActive(true);
        InputAction actionToRebind = inputActions.FindAction(actionName);
        actionToRebind.Disable();
        string previousOverridePath = actionToRebind.bindings[0].overridePath;
        rebindOperation = actionToRebind.PerformInteractiveRebinding()
            .WithControlsExcluding("Mouse")
            .OnComplete(operation => ValidateAndFinish(operation, actionToRebind, previousOverridePath))
            .OnCancel(operation => FinishRebind(actionToRebind))
            .Start();
        return actionToRebind;
    }

    private void ValidateAndFinish(InputActionRebindingExtensions.RebindingOperation operation, InputAction Rebind, string previousOverridePath)
    {
        string newKeyPath = Rebind.bindings[0].effectivePath;
        bool isDuplicate = false;

        foreach (InputAction action in inputActions)
        {
            if (action == Rebind) continue;

            if (action.bindings[0].effectivePath == newKeyPath)
            {
                isDuplicate = true;
                break;
            }
        }

        if (isDuplicate)
        {
            if (!string.IsNullOrEmpty(previousOverridePath))
            {
                Rebind.ApplyBindingOverride(0, previousOverridePath);
            }
            else
            {
                Rebind.RemoveBindingOverride(0);
            }
        }
        FinishRebind(Rebind);
    }

    private void FinishRebind(InputAction action)
    {
        action.Enable();
        rebindOperation.Dispose();
        PlayerPrefs.SetString("Keybinds", inputActions.SaveBindingOverridesAsJson());
        Start();
        blockingOverlay.SetActive(false);
    }

    public void RebindUp()
    {
        StartInteractiveRebind("RhythmUp");
    }
    public void RebindDown()
    {
        StartInteractiveRebind("RhythmDown");
    }
    public void RebindLeft()
    {
        StartInteractiveRebind("RhythmLeft");
    }
    public void RebindRight()
    {
        StartInteractiveRebind("RhythmRight");
    }
    public void RebindCombo()
    {
        StartInteractiveRebind("ComboToggle");
    }
}
