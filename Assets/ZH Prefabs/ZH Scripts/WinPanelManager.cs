using TMPro;
using UnityEngine;

public class WinPanelManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public TextMeshProUGUI TimeText;
    public TextMeshProUGUI ClearText;
    public int Clear;
    public float time;

    void Start()
    {
        Clear = SetBossHealth.Instance.Clear;
        time = SetBossHealth.Instance.time;
        TimeText.text = "Time: " + time + "s";
        if (Clear == 0) ClearText.text = "Not Cleared";
        if (Clear == 1) ClearText.text = "Cleared";
        if (Clear == 2) ClearText.text = "Damageless";
        if (Clear == 3) ClearText.text = "Full Combo";
        if (Clear == 4) ClearText.text = "All Perfect";
    }
}
