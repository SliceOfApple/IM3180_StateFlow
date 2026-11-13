using TMPro;
using UnityEngine;

public class LevelTextReveal : MonoBehaviour
{
    public float revealDuration;
    public float revealSpeed;
    public float hideSpeed;
    public int state;

    private float _revealTimeLeft;

    private TMP_Text textComp;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        state = 0;
        _revealTimeLeft = revealDuration;
        textComp = gameObject.GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case 0: // Reveal text
                textComp.color = new Color(textComp.color.r, textComp.color.g, textComp.color.b, textComp.color.a + revealSpeed * Time.deltaTime);
                if (textComp.color.a >= 1f) state = 1;
                break;
            case 1: // Wait for revealDuration amount.
                //StartCoroutine(ExecuteAfterSeconds(revealDuration, () => state = 2));
                // NOTE: DO NOT USE COROUTINE HERE. THIS WILL CAUSE update() IN MusicManager TO FIRE MULTIPLE TIMES AND BREAK THE GAME. Instead, use a timer.
                _revealTimeLeft -= Time.deltaTime;
                if (_revealTimeLeft <= 0f) state = 2;
                break;
            case 2: // Hide text
                textComp.color = new Color(textComp.color.r, textComp.color.g, textComp.color.b, textComp.color.a - hideSpeed * Time.deltaTime);
                if (textComp.color.a <= 0f)
                {
                    state = 3;
                    Boss.Instance.shouldBringBossIntoView = true;
                    if (BossIntroPlayer.Instance != null)
                    {
                        AudioListener.pause = false;
                        MusicManager.Instance.audioSource.PlayOneShot(BossIntroPlayer.Instance.audioClipIntro);
                    }
                    //GameManager.Instance.StartLevel();
                    Destroy(gameObject);
                }
                break;
        }
    }
}
