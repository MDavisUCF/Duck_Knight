
using System.Collections.Generic;
using UnityEngine;

public class GoblinSpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject[] goblinPrefabs;
    public Transform duckKnight;
    public InputManager inputManager;
    public Collider swordCollider;

    [Header("Spawn Area")]
    public float minX = -4f;
    public float maxX = 4f;
    public float spawnZ = 2f;
    public float spawnHeight = 0.5f;

    [Header("Difficulty")]
    public float roundDuration = 90f;
    public float startingInterval = 5f;
    public float minimumInterval = 1.5f;
    public int maxGoblins = 8;

    private float roundStartTime;
    private float nextSpawnTime;

    private readonly List<GameObject> spawnedGoblins =
        new List<GameObject>();

    void Start()
    {
        roundStartTime = Time.time;
        nextSpawnTime = Time.time + startingInterval;
    }

    void Update()
    {
        if (GameManager.instance != null &&
            !GameManager.instance.gameRunning)
            return;

        spawnedGoblins.RemoveAll(
            goblin => goblin == null || !goblin.activeInHierarchy
        );

        if (Time.time < nextSpawnTime)
            return;

        float elapsed = Time.time - roundStartTime;

        float progress = Mathf.Clamp01(
            elapsed / roundDuration
        );

        float currentInterval = Mathf.Lerp(
            startingInterval,
            minimumInterval,
            progress
        );

        if (spawnedGoblins.Count < maxGoblins)
        {
            SpawnGoblin();
        }

        nextSpawnTime = Time.time + currentInterval;
    }

    void SpawnGoblin()
    {
        if (goblinPrefabs == null ||
            goblinPrefabs.Length == 0)
            return;

        // Randomly select a goblin variant.
        GameObject selectedPrefab =
            goblinPrefabs[
                Random.Range(0, goblinPrefabs.Length)
            ];
            Debug.Log("DUCK KNIGHT: Spawning " + selectedPrefab.name);

        if (selectedPrefab == null)
            return;

        Vector3 spawnPosition = new Vector3(
            Random.Range(minX, maxX),
            spawnHeight,
            spawnZ
        );

        GameObject goblin = Instantiate(
            selectedPrefab,
            spawnPosition,
            Quaternion.identity
        );

        goblin.SetActive(true);

        GoblinChase chase =
            goblin.GetComponent<GoblinChase>();

        if (chase != null)
            chase.duckKnight = duckKnight;

        ScoreZone scoreZone =
            goblin.GetComponent<ScoreZone>();

        if (scoreZone != null)
        {
            scoreZone.inputManager = inputManager;
            scoreZone.swordCollider = swordCollider;
        }

        spawnedGoblins.Add(goblin);
    }
}
