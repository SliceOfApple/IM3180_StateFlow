using TMPro;
using UnityEngine;

public class TextFloatAndFade : MonoBehaviour
{
    public float floatSpeed = 1.0f; // Speed at which the text floats
    public float fadeSpeed = 1.0f; // Speed at which the text fades

    public TMP_Text textComp;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textComp = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        // Float upwards
        gameObject.transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        // Transparent fade
        Color newColor = textComp.color;
        newColor.a -= fadeSpeed * Time.deltaTime; // Decrease alpha over time
        textComp.color = newColor;

        if (newColor.a <= 0f) Destroy(gameObject); // Destroy the text object when fully transparent
    }
}
