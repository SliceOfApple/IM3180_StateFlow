using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Minion-spawning manager. A <b>singleton</b>, only one instance of this class can exist at any time.<br/>
/// Refer to the singleton with <see cref="Instance"/>.
/// </summary>
public class SpawnAlien : MonoBehaviour
{
    // Singleton
    private static SpawnAlien _instance;
    public static SpawnAlien Instance => _instance;

    [SerializeField] private GameObject[] spawnPoints;
    public GameObject Alien1;
    public GameObject Alien2;
    public GameObject Alien3;

    public float spawnDelayMin = 10f;
    public float spawnDelayMax = 15f;

    public List<int> laneBag = new List<int>();
    public int totalLanes = 4;
    public float AttackGrowthInverse;
    public float SpawnDelay;
    public float SpawnTimer;
    public List<GameObject> minions = new List<GameObject>();
    public Vector2 savedVelocity;

    private int minionLayer;
    private int bulletLayer;
    
    // Running timer to keep track of level progression.
    public float timer;
    public int attacklevel;
    
    /// <summary>
    /// Are minions frozen? This happens during level uptime. See <seealso cref="GameManager.IsInUptime"/>.
    /// </summary>
    public bool frozen = false;

    void Start()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            throw new Exception("SpawnAlien manager is already instantiated.");
        }

        // Convert your layer string names into the integer IDs Unity uses
        minionLayer = LayerMask.NameToLayer("Minion");
        bulletLayer = LayerMask.NameToLayer("Bullet");
        Physics2D.IgnoreLayerCollision(minionLayer, bulletLayer, false);
    }
    private void Awake()
    {
        SpawnTimer = UnityEngine.Random.Range(5f, 10f);
        fillbag();
    }

    /// <summary>
    /// Spawns a minion onto a random lane from <see cref="laneBag"/> using <see cref="BagRand"/> to choose.<br/>
    /// If the bag has fewer than 3 lanes left, it is refilled with <see cref="fillbag"/>.
    /// </summary>
    public void SpawnRand()
    {
        int randomInt = BagRand();
        if (laneBag.Count < 3) { fillbag(); }
        GameObject chosenSpawnPoint = spawnPoints[randomInt];
        GameObject newMinion = Instantiate(Alien1, chosenSpawnPoint.transform.position, Quaternion.identity);
        if(!frozen) minions.Add(newMinion);
    }
    public void SpawnRand2()
    {
        int randomInt = BagRand();
        if (laneBag.Count < 3) { fillbag(); }
        GameObject chosenSpawnPoint = spawnPoints[randomInt];
        GameObject newMinion = Instantiate(Alien2, chosenSpawnPoint.transform.position, Quaternion.identity);
        if (!frozen) minions.Add(newMinion);
    }
    public void SpawnRand3()
    {
        int randomInt = BagRand();
        if (laneBag.Count < 3) { fillbag(); }
        GameObject chosenSpawnPoint = spawnPoints[UnityEngine.Random.Range(0, 3)];
        Vector3 spawnPosition = chosenSpawnPoint.transform.position + new Vector3(0.86f, 0f, 0f);
        GameObject newMinion = Instantiate(Alien3, spawnPosition, Quaternion.identity);
        if (!frozen) minions.Add(newMinion);
    }

    public void SpawnSet(int lane)
    {
        GameObject chosenSpawnPoint = spawnPoints[lane];
        GameObject newMinion = Instantiate(Alien1, chosenSpawnPoint.transform.position, Quaternion.identity);
        if (!frozen) minions.Add(newMinion);
    }
    void Update() //Temp
    {
        if (Keyboard.current.enterKey.wasPressedThisFrame) { StartLinearAttack(); }
        if (Keyboard.current.spaceKey.wasPressedThisFrame) { FreezeMinions(); }
        if (Keyboard.current.shiftKey.wasPressedThisFrame) { UnfreezeMinions(); }

        timer += Time.deltaTime;

        // Important to not spawn minions during uptime.
        if (!frozen && timer >= SpawnTimer && attacklevel == 1)
        {
            StartLinearAttack();
            SpawnTimer = timer + UnityEngine.Random.Range(spawnDelayMin, spawnDelayMax);
        }

        if (!frozen && timer >= SpawnTimer && attacklevel == 2)
        {
            StartLinearAttack2();
            SpawnTimer = timer + UnityEngine.Random.Range(spawnDelayMin, spawnDelayMax);
        }
        if (!frozen && timer >= SpawnTimer && attacklevel == 3)
        {
            StartLinearAttack3();
            SpawnTimer = timer + UnityEngine.Random.Range(spawnDelayMin, spawnDelayMax);
        }
    }

    public void fillbag()
    {
        laneBag.Clear();
        for (int i = 0; i < 4; i++)
        {
            laneBag.Add(i);
            laneBag.Add(i);
            laneBag.Add(i);
        }
    }

    /// <summary>
    /// Selects a lane at random from <see cref="laneBag"/> and removes it from the bag to ensure even spawning across lanes.
    /// </summary>
    /// <returns>The lane position in integer form.</returns>
    public int BagRand()
    {
        int randomIndex = UnityEngine.Random.Range(0, laneBag.Count);
        int selectedLane = laneBag[randomIndex];
        laneBag.RemoveAt(randomIndex);
        return selectedLane;
    }

    public void StartLinearAttack()
    {
        // Start the coroutine helper
        StartCoroutine(LinearAttackRoutine());
    }
    private IEnumerator LinearAttackRoutine()
    {
        int strength = (int)(timer / AttackGrowthInverse) + 1;
        GameManager gm = GameManager.Instance;

        for (int i = 0; i < strength; i++)
        {
            yield return new WaitUntil(() => !frozen && !gm.segments[gm.segmentSelector].isUptime);
            Debug.Log("[SpawnAlien] Spawning alien");
            SpawnRand();
            yield return new WaitForSeconds(SpawnDelay);
        }
    }

    public void StartLinearAttack2()
    {
        // Start the coroutine helper
        StartCoroutine(LinearAttackRoutine2());
    }
    private IEnumerator LinearAttackRoutine2()
    {
        int strength = (int)(timer / AttackGrowthInverse) + 1;
        GameManager gm = GameManager.Instance;

        for (int i = 0; i < strength;)
        {
            int rand = UnityEngine.Random.Range(0, 2);
            if (rand == 0)
            {
                yield return new WaitUntil(() => !frozen && !gm.segments[gm.segmentSelector].isUptime);
                Debug.Log("[SpawnAlien] Spawning alien");
                SpawnRand();
                yield return new WaitForSeconds(SpawnDelay);
                i++;
            }
            if (rand == 1 && (strength - i)>=2)
            {
                yield return new WaitUntil(() => !frozen && !gm.segments[gm.segmentSelector].isUptime);
                Debug.Log("[SpawnAlien] Spawning alien");
                SpawnRand2();
                yield return new WaitForSeconds(SpawnDelay);
                i += 2;
            }
        }
    }
    public void StartLinearAttack3()
    {
        // Start the coroutine helper
        StartCoroutine(LinearAttackRoutine3());
    }
    private IEnumerator LinearAttackRoutine3()
    {
        int strength = (int)(timer / AttackGrowthInverse) + 1;
        GameManager gm = GameManager.Instance;

        for (int i = 0; i < strength;)
        {
            int rand = UnityEngine.Random.Range(0, 3);
            if (rand == 0)
            {
                yield return new WaitUntil(() => !frozen && !gm.segments[gm.segmentSelector].isUptime);
                Debug.Log("[SpawnAlien] Spawning alien");
                SpawnRand();
                yield return new WaitForSeconds(SpawnDelay);
                i++;
            }
            if (rand == 1 && (strength - i) >= 2)
            {
                yield return new WaitUntil(() => !frozen && !gm.segments[gm.segmentSelector].isUptime);
                Debug.Log("[SpawnAlien] Spawning alien");
                SpawnRand2();
                yield return new WaitForSeconds(SpawnDelay);
                i += 2;
            }
            if (rand == 2 && (strength - i) >= 3)
            {
                yield return new WaitUntil(() => !frozen && !gm.segments[gm.segmentSelector].isUptime);
                Debug.Log("[SpawnAlien] Spawning alien");
                SpawnRand3();
                yield return new WaitForSeconds(SpawnDelay);
                i += 3;
            }
        }
    }

    public void FreezeMinions()
    {
        if (!frozen)
        {
            minions.RemoveAll(item => item == null);
            foreach (GameObject obj in minions)
            {
                if (obj.TryGetComponent(out Rigidbody2D rb))
                {
                    savedVelocity = rb.linearVelocity;
                    rb.linearVelocity = Vector2.zero;
                    rb.bodyType = RigidbodyType2D.Kinematic;
                }
            }
            Physics2D.IgnoreLayerCollision(minionLayer, bulletLayer, true);
            frozen = true;
        }
    }
    public void UnfreezeMinions()
    {
        if (frozen)
        {
            foreach (GameObject obj in minions)
            {
                if (obj.TryGetComponent(out Rigidbody2D rb))
                {
                    rb.bodyType = RigidbodyType2D.Dynamic;
                    rb.linearVelocity = savedVelocity;
                }
            }
            Physics2D.IgnoreLayerCollision(minionLayer, bulletLayer, false);
            frozen = false;
        }
    }
}