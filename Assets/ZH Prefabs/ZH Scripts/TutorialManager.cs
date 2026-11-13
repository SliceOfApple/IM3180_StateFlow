using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;


[System.Serializable]
public struct TutorialLine
{
    [TextArea(3, 5)]
    public string sentence;
    public UnityEvent onLineStart;
    public float X;
    public float Y;
}

public class TutorialManager : MonoBehaviour, IPointerDownHandler
{
    public TutorialLine[] scriptLines;
    private int currentIndex = -1;
    private bool isTyping = false;
    public static TutorialManager _tutorialManager; // Singleton.
    public static TutorialManager Instance => _tutorialManager;
    public GameObject TextBox;
    public Transform Canvas;
    public GameObject HighlightHealth;
    public GameObject HighlightCurrent;
    public GameObject HighlightAvailable;
    public GameObject HighlightShop;
    public GameObject HighlightCurrency;
    public GameObject HighlightBoss;
    public GameObject HighlightArrows;
    private GameObject TextBoxInstance;
    private GameObject HighlightInstance;
    public GameObject blocker;
    private TextMeshProUGUI dialogueText;
    public TextMeshProUGUI LeftText;
    public TextMeshProUGUI UpText;
    public TextMeshProUGUI DownText;
    public TextMeshProUGUI RightText;
    public TextMeshProUGUI ComboText;
    [SerializeField] private MusicManager musicManager;
    private Boolean popupActive = false;
    public GameObject Popup;
    public Boolean ComboTut = false;
    public Boolean TurretTut = false;
    public Boolean ShootTut = false;
    public Boolean income = true;
    private InputAction comboToggleButton,
                rhythmUp,
                rhythmDown,
                rhythmLeft,
                rhythmRight;

    public void removeblocker()
    {
        blocker.SetActive(false);
    }

    public void inputshow()
    {
        rhythmUp = InputSystem.actions.FindAction("RhythmUp");
        rhythmDown = InputSystem.actions.FindAction("RhythmDown");
        rhythmLeft = InputSystem.actions.FindAction("RhythmLeft");
        rhythmRight = InputSystem.actions.FindAction("RhythmRight");
        LeftText.text = rhythmLeft.GetBindingDisplayString(0);
        UpText.text = rhythmUp.GetBindingDisplayString(0);
        DownText.text = rhythmDown.GetBindingDisplayString(0);
        RightText.text = rhythmRight.GetBindingDisplayString(0);
        LeftText.enabled = true;
        UpText.enabled = true;
        DownText.enabled = true;
        RightText.enabled = true;
    }

    public void inputhide()
    {
        LeftText.enabled = false;
        UpText.enabled = false;
        DownText.enabled = false;
        RightText.enabled = false;
    }

    public void comboshow()
    {
        comboToggleButton = InputSystem.actions.FindAction("ComboToggle");
        ComboText.text = "Combo Key: " + comboToggleButton.GetBindingDisplayString(0);
        ComboText.enabled = true;
    }

    public void combohide()
    {
        ComboText.enabled = false;
    }

    private void OnEnable()
    {
        if (musicManager != null) musicManager.BeatCount += HandleBeatCount;
    }

    private void OnDisable()
    {
        if (musicManager != null) musicManager.BeatCount -= HandleBeatCount;

    }
    public void ComboTutorial()
    {
        ComboTut = true;
    }
    public void TurretTutorial()
    {
        TurretTut = true;
    }
    public void ShootTutorial()
    {
        ShootTut = true;
    }
    public void activateincome()
    {
        income = true;
    }
    public void deactivateincome()
    {
        income = false;
    }
    public void HandleBeatCount(int count)
    {
        if (count == 5 || count == 12 || count == 52) StartCutscene();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!popupActive)
        {
            if (!isTyping)
            {
                AdvanceCutscene();
            }
            else
            {
                skipTypewriter();
            }
        }
        if (eventData.pointerCurrentRaycast.gameObject != null)
        {
            Debug.Log("UI Click caught by: " + eventData.pointerCurrentRaycast.gameObject.name, eventData.pointerCurrentRaycast.gameObject);
        }

        // Prints the object Unity thinks received the initial press down event
        if (eventData.pointerPress != null)
        {
            Debug.Log("Pointer Press Object: " + eventData.pointerPress.name, eventData.pointerPress);
        }
    }
    public void skipTypewriter()
    {
        StopAllCoroutines();
        TextBoxInstance.transform.GetComponentInChildren<TextMeshProUGUI>().maxVisibleCharacters = scriptLines[currentIndex].sentence.Length;
        isTyping = false;
    }

    public void removebox()
    {
        StopAllCoroutines();
        Destroy(TextBoxInstance);
    }

    void Start()
    {
        if (_tutorialManager == null)
        {
            _tutorialManager = this;
        }
    }

    public void textUpdate()
    {
        dialogueText = TextBoxInstance.transform.GetComponentInChildren<TextMeshProUGUI>();
        dialogueText.text = scriptLines[currentIndex].sentence;
        scriptLines[currentIndex].onLineStart?.Invoke();
        StartCoroutine(TypewriterEffect());
    }

    public void StartCutscene()
    {
        if (TextBoxInstance != null) Destroy(TextBoxInstance);
            this.GetComponent<Image>().raycastTarget = true;
        Time.timeScale = 0.0f;
        BlockKeyboard();
        MusicManager.Instance.PauseMusic();
        TextBoxInstance = Instantiate(TextBox, Canvas);
        AdvanceCutscene();
    }

    public void highlighthealth()
    {
        highlightstop();
        HighlightInstance = Instantiate(HighlightHealth, Canvas);
    }
    public void highlightcurrent()
    {
        highlightstop();
        HighlightInstance = Instantiate(HighlightCurrent, Canvas);
    }
    public void highlightavailable()
    {
                highlightstop();
        HighlightInstance = Instantiate(HighlightAvailable, Canvas);
    }
    public void highlightshop()
    {
        highlightstop();
        HighlightInstance = Instantiate(HighlightShop, Canvas);
    }
    public void highlightcurrency()
    {
        highlightstop();
        HighlightInstance = Instantiate(HighlightCurrency, Canvas);
    }
    public void highlightboss()
    {
        highlightstop();
        HighlightInstance = Instantiate(HighlightBoss, Canvas);
    }
    public void highlightarrows()
    {
        highlightstop();
        HighlightInstance = Instantiate(HighlightArrows, Canvas);
    }
    public void highlightstop()
    {
        if(HighlightInstance != null) Destroy(HighlightInstance);
    }

    public void MoveTextBox()
    {
        TextBoxInstance.GetComponent<Transform>().position = new Vector3(scriptLines[currentIndex].X, scriptLines[currentIndex].Y, transform.position.z);
    }
    public void StopCutscene()
    {
        StopAllCoroutines();
        Destroy(TextBoxInstance);
        Time.timeScale = 1.0f;
        UnblockKeyboard();
        MusicManager.Instance.PlayMusic();
        this.GetComponent<Image>().raycastTarget = false;
    }
    public void StopCutscenePersistent()
    {
        StopAllCoroutines();
        Time.timeScale = 1.0f;
        UnblockKeyboard();
        MusicManager.Instance.PlayMusic();
        this.GetComponent<Image>().raycastTarget = false;
    }

    public void AdvanceCutscene()
    {
        if (isTyping == true) skipTypewriter();
        currentIndex++;
        MoveTextBox();
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

    public void ContinueTutorial()
    {
        currentIndex+=3;
        MoveTextBox();
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

    public void popup()
    {
        popupActive = true;
        Popup.SetActive(true);
    }
    public void popupdeactivate()
    {
        Popup.SetActive(false);
        popupActive = false;
    }

    private IEnumerator TypewriterEffect()
    {
        isTyping = true;
        dialogueText.maxVisibleCharacters = 0;

        int totalCharacters = scriptLines[currentIndex].sentence.Length;

        for (int i = 0; i <= totalCharacters; i++)
        {
            dialogueText.maxVisibleCharacters = i;
            yield return new WaitForSecondsRealtime(0.02f);
        }

        isTyping = false;
    }
    public void startAttack()
    {
        SpawnAlien.Instance.timer = 0f;
        SpawnAlien.Instance.AttackGrowthInverse = 50f;
    }
    public void BlockKeyboard()
    {
        if (Keyboard.current != null)
        {
            InputSystem.DisableDevice(Keyboard.current);
        }
    }
    public void UnblockKeyboard()
    {
        if (Keyboard.current != null)
        {
            InputSystem.EnableDevice(Keyboard.current);
        }
    }
}
