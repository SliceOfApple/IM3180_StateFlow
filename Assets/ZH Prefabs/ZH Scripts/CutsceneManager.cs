using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;


[System.Serializable]
public struct DialogueLine
{
    public string speakerName;

    [TextArea(3, 5)]
    public string sentence;
    public UnityEvent onLineStart;
    public Sprite BG;
}

public class CutsceneManager : MonoBehaviour, IPointerDownHandler
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public DialogueLine[] scriptLines;
    private int currentIndex = -1;
    private bool isTyping = false;
    private bool isFading = false;
    public Sprite AnimatedBG;
    public GameObject bgPrimary;
    public GameObject bgTransition;
    public float fadeDuration = 1.0f;
    private bool animated;
    private Boolean popupActive = false;
    public GameObject YesNo;
    public Transform Canvas;
    public GameObject syncpanel;
    public GameObject dialougeBox;
    public void OnPointerDown(PointerEventData eventData)
    {

            if (!isTyping && !isFading && !popupActive)
            {
                AdvanceCutscene();
            }
            else
            {
                StopAllCoroutines();
                dialogueText.maxVisibleCharacters = scriptLines[currentIndex].sentence.Length;
                bgTransition.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
                bgPrimary.GetComponent<SpriteRenderer>().sprite = scriptLines[currentIndex].BG;
                bgTransition.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
                if (animated)
                {
                    bgTransition.GetComponent<Animator>().enabled = true;
                    bgTransition.GetComponent<SpriteRenderer>().sprite = AnimatedBG;
                }
                isTyping = false;
                isFading = false;
                bgTransition.GetComponent<Animator>().enabled = false;
            }
  
    }
    public void removebox()
    {
        StopAllCoroutines();
        popupActive = false;
        YesNo.SetActive(false);
        Destroy(dialougeBox);
    }

    void Start()
    {
        AdvanceCutscene();
    }

    public void textUpdate()
    {
        nameText.text = scriptLines[currentIndex].speakerName;
        dialogueText.text = scriptLines[currentIndex].sentence;
        scriptLines[currentIndex].onLineStart?.Invoke();
        StartCoroutine(TypewriterEffect());
    }

    public void textBoxGenerate(string Text, Transform Canvas)
    {

    }
    
    public void SwitchBG()
    {
        isFading=true;
        StartCoroutine(FadeRoutine(scriptLines[currentIndex].BG, false));
    }

    public void StartCutscene()
    {

    }
    public void StopCutscene()
    {

    }
    public void AdvanceCutscene()
    {
        currentIndex++;
        bgTransition.GetComponent<Animator>().enabled = false;
        if (currentIndex < scriptLines.Length)
        {
            textUpdate();
        }
        else
        {
            Debug.Log("Cutscene complete! Load next scene or fade out here.");
            gameObject.SetActive(false);
        }
    }

    public void BGanimation()
    {
        isFading = true;
        StartCoroutine(FadeRoutine(scriptLines[currentIndex].BG, true));
    }
    
    private IEnumerator TypewriterEffect()
    {
        isTyping = true;
        dialogueText.maxVisibleCharacters = 0;

        int totalCharacters = scriptLines[currentIndex].sentence.Length;

        for (int i = 0; i <= totalCharacters; i++)
        {
            dialogueText.maxVisibleCharacters = i;
            yield return new WaitForSeconds(0.02f);
        }

        isTyping = false;
    }

    private IEnumerator FadeRoutine(Sprite newBackground, bool isAnimated)
    {
        animated = isAnimated;
        SpriteRenderer Transition = bgTransition.GetComponent<SpriteRenderer>();
        SpriteRenderer Primary = bgPrimary.GetComponent<SpriteRenderer>();
        Transition.sprite = newBackground;
        Transition.color = new Color(1, 1, 1, 0);

        float timer = 0;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = timer / fadeDuration;

            bgTransition.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, alpha);

            yield return null;
        }


        Transition.color = new Color(1, 1, 1, 1);
        Primary.sprite = newBackground;
        Transition.color = new Color(1, 1, 1, 0);
        isFading = false;
        if (isAnimated)
        {
            bgTransition.GetComponent<Animator>().enabled = true;
            bgTransition.GetComponent<SpriteRenderer>().sprite = AnimatedBG;
        }
    }
    public void popup()
    {
        popupActive = true;
        YesNo.SetActive(true);
    }
    public void popupnofreeze()
    {
        YesNo.SetActive(true);
    }
    public void popupdeactivate()
    {
        YesNo.SetActive(false);
        popupActive = false;
    }
    public void syncinstan()
    {
        Canvas = this.GetComponentInParent<Canvas>().transform;
        GameObject panel = Instantiate(syncpanel, Canvas, false);
        AudioSyncing listen = panel.GetComponentInChildren<AudioSyncing>();
        listen.Destroyed += AdvanceCutscene;
    }
}
