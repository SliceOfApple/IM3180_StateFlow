using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class tutSetCurrency : MonoBehaviour
{
    public static tutSetCurrency Instance;
    public int Currency;
    public TextMeshProUGUI TextElement;
    private void Awake()
    {
        Instance = this;
    }

    public void ChangeCurrency(int currency)
    {
        Currency += currency;
        if (TutorialManager.Instance.ComboTut == true)
        {
            TutorialManager.Instance.ComboTut = false;
            ChangeCurrency(100);
            TutorialManager.Instance.AdvanceCutscene();
        }
    }
    public int CheckCurrency()
    {
        return Currency;
    }

    void Update()
    {
        TextElement.text = "" + Currency;
        if (Keyboard.current.spaceKey.wasPressedThisFrame) { ChangeCurrency(10); }
    }

}
