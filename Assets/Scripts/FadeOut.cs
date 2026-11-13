using UnityEngine;
using UnityEngine.UI;

public class FadeOut : MonoBehaviour
{
    private Image image;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        var oldColour = image.color;
        if (image.color.a > 0) image.color = new Color(oldColour.r, oldColour.g, oldColour.b, Mathf.Lerp(image.color.a, 0, 0.05f));
    }
}
