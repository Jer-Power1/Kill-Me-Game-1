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

    int toSpawn = 0;
    float nextSpawnTime = 0f;

    public System.Action<GameObject> onSpawned;

    void Awake()
    {
        if (!cam) cam = Camera.main;
        if (spawnPoints == null || spawnPoints.Length == 0)
            spawnPoints = FindObjectsOfType<SpawnPoint>();
    }

    void Update()
    {
        // Passive spawner: only acts when RoundManager queues spawns
        if (toSpawn <= 0) return;
        if (!enemyPrefab || spawnPoints == null || spawnPoints.Length == 0) return;

        if (Time.time < nextSpawnTime) return;
        nextSpawnTime = Time.time + spawnInterval;

        if (TrySpawnOne(out var enemy))
        {
            toSpawn--;
            onSpawned?.Invoke(enemy);
        }
        else
        {
            // Fail-safe: do NOT stall rounds forever if all points are blocked/in view.
            toSpawn--;
            Debug.LogWarning("Spawner: failed to find valid spawn point; skipping one spawn to avoid stalling.");
        }
    }

    /// <summary>
    /// Called by RoundManager to start a round's spawns. This REPLACES any previous queued count.
    /// </summary>
    public void QueueSpawns(int amount)
    {
        toSpawn = Mathf.Max(0, amount);       // reset for the round (no accumulation)
        nextSpawnTime = Time.time + 0.1f;     // small initial delay
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

            // Ensure not spawning inside geometry/another enemy
            if (Physics.CheckSphere(pos, clearRadius, blockMask, QueryTriggerInteraction.Ignore))
                continue;

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
