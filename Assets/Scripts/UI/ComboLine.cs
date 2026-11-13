using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Utility;

public class ComboLine : MonoBehaviour
{
    public TMP_Text ComboNameText;
    private string comboName;
    public string ComboName 
    { 
        get => comboName; 
        set
        {
            comboName = value;
            ComboNameText.text = $"<nobr>{comboName}</nobr>";
        } 
    }
    public GameObject[] arrowDisplays;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Updates the <b>colour</b> on the combo name to reflect cooldown progress.
    /// </summary>
    /// <param name="cooldownProgress">Ratio of cooldown progress.</param>
    public void UpdateComboName(float cooldownProgress)
    {
        if (cooldownProgress == 0) 
        {
            ComboNameText.text = $"<color=#00ff00><nobre>{ComboName}</nobre></color>";
            return;
        }

        string newTextValue = "<nobr>";
        int threshold = (int) ((1 - cooldownProgress) * ComboName.Length);
        bool thresholdPassed = false;

        for (int i = 0; i < ComboName.Length; i++)
        {
            if (i >= threshold && !thresholdPassed)
            {
                thresholdPassed = true;
                newTextValue += "<color=#ff0000>";
            }

            newTextValue += i == threshold 
                          ? "<gradient=\"RtoG\">" + ComboName[i] + "</gradient>" 
                          : ComboName[i];
        }
        newTextValue += "</color></nobr>";
        ComboNameText.text = newTextValue;
    }

    /// <summary>
    /// Changes the combo line display to show a new combo name and arrow sequence.
    /// </summary>
    /// <param name="name">Name of the combo</param>
    /// <param name="arrows">Arrow sequence of the combo</param>
    /// <exception cref="System.Exception">This should not happen unless a new <see cref="ArrowDirection"/> appears for some reason.</exception>
    public void ChangeDisplay(string name, ArrowDirection[] arrows)
    {
        ComboName = name;

        for (int i = 0; i < arrows.Length; i++)
        {
            arrowDisplays[i].GetComponent<Image>().sprite = arrows[i] switch
            {
                ArrowDirection.LEFT => ComboManager.Instance.leftArrowSprite,
                ArrowDirection.UP => ComboManager.Instance.upArrowSprite,
                ArrowDirection.DOWN => ComboManager.Instance.downArrowSprite,
                ArrowDirection.RIGHT => ComboManager.Instance.rightArrowSprite,
                _ => throw new System.Exception("Invalid arrow direction") // Should not happen.
            };
        }
    }

    /// <summary>
    /// Darkens alpha value of arrows to show that the combo is <b><u>on</u></b> cooldown.
    /// </summary>
    public void DarkenArrows()
    {
        for (int i = 0; i < arrowDisplays.Length; i++)
        {
            Image arrowImage = arrowDisplays[i].GetComponent<Image>();
            Color color = arrowImage.color;
            color.r = 0.25f;
            color.g = 0.25f;
            color.b = 0.25f;
            arrowImage.color = color;
        }
    }

    /// <summary>
    /// Lightens alpha value of arrows to show that the combo is <b><u>off</u></b> cooldown.
    /// </summary>
    public void LightenArrows()
    {
        for (int i = 0; i < arrowDisplays.Length; i++)
        {
            Image arrowImage = arrowDisplays[i].GetComponent<Image>();
            Color color = arrowImage.color;
            color.r = 1f;
            color.g = 1f;
            color.b = 1f;
            arrowImage.color = color;
        }
    }
}
