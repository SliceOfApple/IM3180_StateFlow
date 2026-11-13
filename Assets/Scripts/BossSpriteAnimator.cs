using UnityEngine;

public class BossSpriteAnimator : MonoBehaviour
{
    public Sprite[] bossSprites; // Array of sprites for the boss animation
    public int framesPerSecond = 24; // Number of frames per second for the animation
        private double secondsPerFrame; // Time in seconds for each frame

    private float timer = 0f; // Timer to track the elapsed time
    private int currentFrame = 0; // Current frame index

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        secondsPerFrame = 1d / framesPerSecond;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= secondsPerFrame)
        {
            timer = 0;
            // Advance to the next sprite frame
            if (currentFrame < bossSprites.Length - 1) currentFrame++;
            else currentFrame = 0;
            GetComponent<SpriteRenderer>().sprite = bossSprites[currentFrame];
        }
    }
}
