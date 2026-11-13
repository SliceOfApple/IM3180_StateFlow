using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public float time;
    public float StartTime;
    public TextMeshProUGUI TextElement;
    private void Start()
    {
        StartTime = Time.time;
    }
    void Update()
    {
        time = Time.time - StartTime;
        TextElement.text = time.ToString("F2");
    }
}
