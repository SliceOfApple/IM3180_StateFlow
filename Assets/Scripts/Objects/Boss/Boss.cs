using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A singleton class for the boss object. Only one instance can exist at any time.<br/>
/// Refer to the singleton with <see cref="Instance"/>.
/// </summary>
public class Boss : MonoBehaviour
{
    public GameObject healthBar;

    private static Boss _instance;
    public static Boss Instance => _instance;

    // Movement states
    private bool isMoving = false,
                 bossInView = false;
    public bool shouldBringBossIntoView = false;

    // Boss movement
    public Vector3 startingPoint = new Vector3(0, 3.5f, 0);
    private Vector3 newLocation;
    private Vector3 MoveInVelocity = Vector3.zero;
    public float MovementSpeed = 0.001f; // units per second
    public float TimeToReachPoint = 0.5f;

    public int Damage;
    public int Health;

    // Start is called before the first frame update
    void Start()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            throw new InvalidOperationException("Boss instance already instantiated.");
        }

        Slider healthBarSlider = healthBar.GetComponent<Slider>();
        healthBarSlider.maxValue = Health > 0 ? Health : 5000;
        healthBarSlider.value = healthBarSlider.maxValue;
    }

    // Update is called once per frame
    void Update()
    {
        if (!bossInView && shouldBringBossIntoView) { BringBossIntoView(); return; }
        if (!isMoving) DeriveNewPosition();
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, newLocation, MovementSpeed * Time.deltaTime);  
            if (Vector3.Distance(transform.position, newLocation) < 0.0001f) isMoving = false;
        }
    }

    public void DestroyInstance() => _instance = null;

    // TODO - Not sure if this is necessary. Check with ZH.
    private void OnTriggerEnter2D(Collider2D hit)
    {
        if (hit.gameObject.CompareTag("Boss"))
        {
            SetBossHealth.Instance.TakeDamage(Damage);
            Destroy(gameObject);
        }
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        Slider healthBarSlider = healthBar.GetComponent<Slider>();
        healthBarSlider.value = Health;

        if (Health <= 0)
        {
            // Boss defeated
            Destroy(gameObject);
            if (TutorialManager.Instance == null) SetBossHealth.Instance.ShowVictoryPanel(); // TODO - Bunch it all in here and get rid of SetBossHealth.cs.
            else TutorialManager.Instance.StartCutscene();
        }
    }

    /// <summary>
    /// Gets a new position to move the boss.
    /// </summary>
    private void DeriveNewPosition()
    {
        // Pick a random point near the defined starting point.
        float x = UnityEngine.Random.Range(-0.25f, 0.25f) + startingPoint.x;
        float y = UnityEngine.Random.Range(-0.25f, 0.25f) + startingPoint.y;
        newLocation = new Vector3(x, y, transform.position.z); // Lock z.

        isMoving = true;
    }

    private void BringBossIntoView()
    {
        // Move boss into view in 0.5 seconds.
        transform.position = Vector3.SmoothDamp(transform.position, startingPoint, ref MoveInVelocity, TimeToReachPoint);
        //transform.position = Vector3.MoveTowards(transform.position, startingPoint, 1f * Time.deltaTime);
        if (Vector3.Distance(transform.position, startingPoint) <= 0.05f)
        { 
            bossInView = true;
            MusicManager.Instance.lastBeat = 0;
            AudioListener.pause = true;
            MusicManager.Instance.startTime = AudioSettings.dspTime;
            GameManager.Instance.StartLevel();
        }
    }
}