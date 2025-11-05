using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Refs")]
    public Transform player;          // your Player root
    public Camera cam;                // player camera
    public GameObject enemyPrefab;    // your Enemy prefab (EnemyChase + EnemyHealth)

    [Header("Spawn Ring (around player)")]
    public float minDistance = 15f;   // inner radius
    public float maxDistance = 35f;   // outer radius

    [Header("Spawning")]
    public float spawnInterval = 2f;  // seconds between attempts
    public int batchSize = 2;       // how many per tick
    public int maxAlive = 12;       // cap concurrent enemies
    public int attemptsPerEnemy = 10;

    [Header("Grounding")]
    public LayerMask groundMask = ~0; // set to your ground/environment layers
    public float groundRayHeight = 50f;  // how high we cast from above candidate pos
    public float groundRayLength = 200f; // total ray length downward
    public float clearRadius = 0.6f;     // avoid overlapping colliders at spawn
    public LayerMask blockMask = ~0;     // things to avoid overlapping (enemies, walls, player)

    [Header("Visibility")]
    [Tooltip("How far outside the screen the point must be (0=exact edge).")]
    public float viewportMargin = 0.05f; // small buffer outside 0..1
    public bool requireInFront = false;  // if true, only spawn to the sides/outside FOV (not behind)

    readonly List<GameObject> alive = new();

    float nextTime;

    void Awake()
    {
        if (!player) player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (!cam) cam = Camera.main;
    }

    void Update()
    {
        // prune dead references
        for (int i = alive.Count - 1; i >= 0; i--)
            if (alive[i] == null) alive.RemoveAt(i);

        if (!player || !cam || !enemyPrefab) return;
        if (Time.time < nextTime) return;
        nextTime = Time.time + spawnInterval;

        int toSpawn = Mathf.Min(batchSize, maxAlive - alive.Count);
        for (int i = 0; i < toSpawn; i++)
        {
            if (TrySpawnOne(out var e)) alive.Add(e);
        }
    }

    bool TrySpawnOne(out GameObject spawned)
    {
        spawned = null;
        for (int attempt = 0; attempt < attemptsPerEnemy; attempt++)
        {
            // 1) pick random point on ring
            float dist = Random.Range(minDistance, maxDistance);
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            Vector3 dir = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
            Vector3 flatPos = player.position + dir * dist;

            // 2) drop to ground
            Vector3 rayStart = new Vector3(flatPos.x, player.position.y + groundRayHeight, flatPos.z);
            if (!Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, groundRayLength, groundMask, QueryTriggerInteraction.Ignore))
                continue;

            Vector3 spawnPos = hit.point;

            // 3) ensure outside camera view
            if (!IsOutOfView(spawnPos)) continue;

            // 4) ensure clear space
            if (Physics.CheckSphere(spawnPos, clearRadius, blockMask, QueryTriggerInteraction.Ignore))
                continue;

            // 5) spawn
            spawned = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            return true;
        }
        return false;
    }

    bool IsOutOfView(Vector3 worldPos)
    {
        Vector3 vp = cam.WorldToViewportPoint(worldPos);
        // if behind camera, it's out of view
        if (vp.z < 0f) return true;

        // optionally only consider points in front of camera
        if (requireInFront && vp.z <= 0f) return false;

        // outside viewport with margin?
        if (vp.x < -viewportMargin || vp.x > 1f + viewportMargin) return true;
        if (vp.y < -viewportMargin || vp.y > 1f + viewportMargin) return true;

        // inside view -> reject
        return false;
    }

    // Optional: call when enemies die to free slots immediately
    public void NotifyEnemyDied(GameObject enemy)
    {
        int idx = alive.IndexOf(enemy);
        if (idx >= 0) alive.RemoveAt(idx);
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (!player) return;
        Gizmos.color = new Color(0f, 1f, 0f, 0.15f);
        DrawCircle(player.position, minDistance);
        DrawCircle(player.position, maxDistance);
    }
    void DrawCircle(Vector3 center, float r, int seg = 64)
    {
        Vector3 prev = center + new Vector3(r, 0, 0);
        for (int i = 1; i <= seg; i++)
        {
            float a = (i / (float)seg) * Mathf.PI * 2f;
            Vector3 p = center + new Vector3(Mathf.Cos(a) * r, 0, Mathf.Sin(a) * r);
            Gizmos.DrawLine(prev, p);
            prev = p;
        }
    }
#endif
}
