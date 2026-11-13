using UnityEngine;

public class BossIntroPlayer : MonoBehaviour
{
    public static BossIntroPlayer Instance { get; private set; }

    public AudioClip audioClipIntro;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            throw new System.Exception("Multiple instances of BossIntroPlayer detected.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
