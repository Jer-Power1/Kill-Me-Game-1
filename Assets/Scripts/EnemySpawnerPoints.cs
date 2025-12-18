using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerPoints : MonoBehaviour
{
    [Header("Refs")]
    public Camera cam;
    public GameObject enemyPrefab;
    public SpawnPoint[] spawnPoints;

    [Header("Spawn Rules")]
    public float spawnInterval = 0.6f;
    public int attemptsPerEnemy = 12;
    public float viewportMargin = 0.06f;
    public LayerMask blockMask = ~0;
    public float clearRadius = 0.6f;

    int toSpawn;
    float nextSpawnTime;

    public System.Action<GameObject> onSpawned;

    void Awake()
    {
        if (!cam) cam = Camera.main;
        if (spawnPoints == null || spawnPoints.Length == 0)
            spawnPoints = FindObjectsOfType<SpawnPoint>();
    }

    void Update()
    {
        if (toSpawn <= 0 || !enemyPrefab || spawnPoints.Length == 0) return;
        if (Time.time < nextSpawnTime) return;

        nextSpawnTime = Time.time + spawnInterval;

        if (TrySpawnOne(out var e))
        {
            toSpawn--;
            onSpawned?.Invoke(e);
        }
    }

    public void QueueSpawns(int amount)
    {
        toSpawn += Mathf.Max(0, amount);
        if (toSpawn > 0) nextSpawnTime = Mathf.Min(nextSpawnTime, Time.time + 0.1f);
    }

    public int RemainingToSpawn => toSpawn;

    bool TrySpawnOne(out GameObject spawned)
    {
        spawned = null;

        for (int attempt = 0; attempt < attemptsPerEnemy; attempt++)
        {
            var sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Vector3 pos = sp.transform.position;

            if (!IsOutOfView(pos)) continue;
            if (Physics.CheckSphere(pos, clearRadius, blockMask, QueryTriggerInteraction.Ignore)) continue;

            spawned = Instantiate(enemyPrefab, pos, sp.transform.rotation);
            return true;
        }

        return false;
    }

    bool IsOutOfView(Vector3 worldPos)
    {
        if (!cam) return true;

        Vector3 vp = cam.WorldToViewportPoint(worldPos);
        if (vp.z < 0f) return true;

        return (vp.x < -viewportMargin || vp.x > 1f + viewportMargin ||
                vp.y < -viewportMargin || vp.y > 1f + viewportMargin);
    }
}
