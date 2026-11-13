using TMPro;
using UnityEngine;

public class LevelSelectManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject Button1,
                      Button2,
                      Button3,
                      Button4,
                      Button5,
                      Button6,
                      Button7,
                      Button8;
    public TextMeshProUGUI Time1, Status1, Time2, Status2, Time3, Status3;
    void Start()
    {
        int progress = PlayerPrefs.GetInt("Progress");
        if (progress > 0) Button1.SetActive(true);
        if (progress > 1) Button2.SetActive(true);
        if (progress > 2) Button3.SetActive(true);
        if (progress > 3) Button4.SetActive(true);
        if (progress > 4) Button5.SetActive(true);
        if (progress > 5) Button6.SetActive(true);
        if (progress > 6) Button7.SetActive(true);
        if (progress > 7) Button8.SetActive(true);
        SetPlayerData();
    }

    public void SetPlayerData()
    {
        int progress = PlayerPrefs.GetInt("Progress");
        if (progress > 2)
        {
            Time1.text = "Fastest Time:\n" + PlayerPrefs.GetFloat("Level1time") + "s";
            int Status = PlayerPrefs.GetInt("Level1ClearState");
            if (Status == 0) Status1.text = "Not Cleared";
            if (Status == 1) Status1.text = "Cleared";
            if (Status == 2) Status1.text = "Damageless";
            if (Status == 3) Status1.text = "Full Combo";
            if (Status == 4) Status1.text = "All Perfect";
        }
        if (progress > 4)
        {
            Time2.text = "Fastest Time:\n" + PlayerPrefs.GetFloat("Level2time") + "s";
            int Status = PlayerPrefs.GetInt("Level2ClearState");
            if (Status == 0) Status2.text = "Not Cleared";
            if (Status == 1) Status2.text = "Cleared";
            if (Status == 2) Status2.text = "Damageless";
            if (Status == 3) Status2.text = "Full Combo";
            if (Status == 4) Status2.text = "All Perfect";
        }
        if (progress > 6)
        {
            Time3.text = "Fastest Time:\n" + PlayerPrefs.GetFloat("Level3time") + "s";
            int Status = PlayerPrefs.GetInt("Lever3ClearState");
            if (Status == 0) Status3.text = "Not Cleared";
            if (Status == 1) Status3.text = "Cleared";
            if (Status == 2) Status3.text = "Damageless";
            if (Status == 3) Status3.text = "Full Combo";
            if (Status == 4) Status3.text = "All Perfect";
        }
    }
}
