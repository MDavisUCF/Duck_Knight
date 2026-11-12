
using System.Collections.Generic;
using UnityEngine;

public class FireballSpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject fireballPrefab;
    public Transform duckKnight;
    public InputManager inputManager;
    public Collider shieldCollider;

    [Header("Spawn Area")]
    public float minX = -4f;
    public float maxX = 4f;
    public float spawnZ = 2f;
    public float spawnHeight = 1f;

    [Header("Difficulty")]
    public float roundDuration = 90f;
    public float startingInterval = 8f;
    public float minimumInterval = 3f;
    public int maxFireballs = 5;

    [Header("Cleanup")]
    public float fireballLifetime = 8f;

    private float roundStartTime;
    private float nextSpawnTime;

    private readonly List<GameObject> activeFireballs =
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

        activeFireballs.RemoveAll(
            fireball => fireball == null || !fireball.activeInHierarchy
        );

        if (Time.time < nextSpawnTime)
            return;

        float progress = Mathf.Clamp01(
            (Time.time - roundStartTime) / roundDuration
        );

        float currentInterval = Mathf.Lerp(
            startingInterval,
            minimumInterval,
            progress
        );

        if (activeFireballs.Count < maxFireballs)
        {
            SpawnFireball();
        }

        nextSpawnTime = Time.time + currentInterval;
    }

    void SpawnFireball()
    {
        if (fireballPrefab == null)
            return;

        Vector3 spawnPosition = new Vector3(
            Random.Range(minX, maxX),
            spawnHeight,
            spawnZ
        );

        GameObject fireball = Instantiate(
            fireballPrefab,
            spawnPosition,
            Quaternion.identity
        );

        fireball.SetActive(true);

        // Set scene references before Start runs on the next frame.
        DangerZone fireballScript =
            fireball.GetComponent<DangerZone>();

        if (fireballScript != null)
        {
            fireballScript.duckKnight = duckKnight;
            fireballScript.inputManager = inputManager;
            fireballScript.shieldCollider = shieldCollider;
            fireballScript.Initialize(duckKnight);
        }

        activeFireballs.Add(fireball);

        // Remove missed projectiles so they don't accumulate.
        Destroy(fireball, fireballLifetime);
    }
}
