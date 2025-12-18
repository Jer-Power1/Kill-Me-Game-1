using UnityEngine;
using TMPro; // remove if you don't use TMP

public class RoundManager : MonoBehaviour
{
    public EnemySpawnerPoints spawner;

    [Header("UI (optional)")]
    public TMP_Text roundText;
    public TMP_Text statusText;

    [Header("Round Settings")]
    public float timeBetweenRounds = 6f;
    public int baseEnemies = 6;
    public int enemiesPerRound = 4;

    [Header("Scaling")]
    public float healthMultiplierPerRound = 0.15f;
    public float speedMultiplierPerRound = 0.06f;

    int round = 0;
    int alive = 0;
    float nextRoundTime;
    bool waiting;

    void Awake()
    {
        if (!spawner) spawner = FindObjectOfType<EnemySpawnerPoints>();
        if (spawner) spawner.onSpawned += OnSpawned;
    }

    void Start()
    {
        StartNextRound();
    }

    void Update()
    {
        UpdateUI();

        if (!waiting && spawner && spawner.RemainingToSpawn <= 0 && alive <= 0)
        {
            waiting = true;
            nextRoundTime = Time.time + timeBetweenRounds;
        }

        if (waiting && Time.time >= nextRoundTime)
        {
            StartNextRound();
        }
    }

    void StartNextRound()
    {
        round++;
        waiting = false;

        int count = baseEnemies + enemiesPerRound * (round - 1);
        spawner.QueueSpawns(count);
    }

    void OnSpawned(GameObject enemy)
    {
        alive++;

        float hMul = 1f + healthMultiplierPerRound * (round - 1);
        float sMul = 1f + speedMultiplierPerRound * (round - 1);

        var eh = enemy.GetComponent<EnemyHealth>();
        if (eh) eh.maxHealth *= hMul;

        var chase = enemy.GetComponent<EnemyChase>();
        if (chase) chase.moveSpeed *= sMul;

        if (eh) eh.onDied += OnEnemyDied;
    }

    void OnEnemyDied()
    {
        alive = Mathf.Max(0, alive - 1);
    }

    void UpdateUI()
    {
        if (roundText) roundText.text = $"ROUND {round}";
        if (statusText && spawner)
            statusText.text = waiting
                ? $"Next round in {Mathf.CeilToInt(nextRoundTime - Time.time)}"
                : $"Alive: {alive}  Spawning: {spawner.RemainingToSpawn}";
    }
}
