using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class SetCurrency : MonoBehaviour
{
    public static SetCurrency Instance;
    public int Currency;
    public TextMeshProUGUI TextElement;
    private void Awake()
    {
        Instance = this;
    }

    public void ChangeCurrency(int currency)
    {
        Currency += currency;
        if (TutorialManager.Instance != null)
        {
            if (TutorialManager.Instance.ComboTut == true)
            {
                TutorialManager.Instance.ComboTut = false;
                ChangeCurrency(100);
                TutorialManager.Instance.StartCutscene();
            }
            else if(!TutorialManager.Instance.income) Currency -= currency;
        }
    }
    public int CheckCurrency()
    {
        return Currency;
    }

    void Update()
    {
        TextElement.text = ""+Currency;
        if(Keyboard.current.spaceKey.wasPressedThisFrame) { ChangeCurrency(10); }
    }

}
