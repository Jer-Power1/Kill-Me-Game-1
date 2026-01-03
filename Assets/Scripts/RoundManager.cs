using UnityEngine;
using System.Collections;

public class RoundManager : MonoBehaviour
{
    public EnemySpawnerPoints spawner;
    public RoundUI roundUI;

    [Header("Rounds")]
    public float breakBetweenRounds = 5f;
    public int baseEnemies = 6;
    public int enemiesPerRound = 4;

    [Header("Enemy Scaling Per Round")]
    public float healthIncreasePerRound = 0.25f;
    public float speedIncreasePerRound = 0.10f;
    public float damageIncreasePerRound = 0.20f;

    int round = 1;   // START AT ROUND 1
    int alive = 0;

    enum State { Break, InRound }
    State state = State.Break;

    Coroutine roundRoutine;

    void Awake()
    {
        if (!spawner) spawner = FindObjectOfType<EnemySpawnerPoints>();
        if (!roundUI) roundUI = FindObjectOfType<RoundUI>(true);

        if (!spawner)
            Debug.LogError("RoundManager: No EnemySpawnerPoints found.");
    }

    void OnEnable()
    {
        if (spawner != null)
            spawner.onSpawned += OnEnemySpawned;
    }

    void OnDisable()
    {
        if (spawner != null)
            spawner.onSpawned -= OnEnemySpawned;
    }

    void Start()
    {
        // Reset ScriptableObject upgrade state
        foreach (var data in Resources.FindObjectsOfTypeAll<WeaponData>())
        {
            data.upgraded = false;
        }

        // Show ROUND 1 instantly
        roundUI?.SetInstant(round);

        // Start first round after break
        StartBreakThenNextRound();
    }

    void Update()
    {
        if (state == State.InRound && spawner != null)
        {
            if (alive <= 0 && spawner.RemainingToSpawn <= 0)
            {
                StartBreakThenNextRound();
            }
        }
    }

    void StartBreakThenNextRound()
    {
        if (roundRoutine != null) return;

        state = State.Break;
        roundRoutine = StartCoroutine(BreakThenStartRound());
    }

    IEnumerator BreakThenStartRound()
    {
        // Break before spawning
        yield return new WaitForSeconds(breakBetweenRounds);

        // Only fade/update UI AFTER the first round
        if (round > 1 && roundUI != null)
            yield return roundUI.FadeToRound(round);

        alive = 0;

        int count = baseEnemies + enemiesPerRound * (round - 1);
        if (spawner != null)
            spawner.QueueSpawns(count);

        state = State.InRound;
        roundRoutine = null;
    }

    void OnEnemySpawned(GameObject enemy)
    {
        alive++;

        int r = Mathf.Max(0, round - 1);

        var eh = enemy.GetComponent<EnemyHealth>();
        if (eh)
        {
            float healthMultiplier = 1f + healthIncreasePerRound * r;
            eh.ApplyHealthMultiplier(healthMultiplier);
            eh.onDied += OnEnemyDied;
        }

        var chase = enemy.GetComponent<EnemyChase>();
        if (chase)
        {
            chase.moveSpeed = chase.baseMoveSpeed * (1f + speedIncreasePerRound * r);
            chase.attackDamage = chase.baseAttackDamage * (1f + damageIncreasePerRound * r);
        }
    }

    void OnEnemyDied()
    {
        alive = Mathf.Max(0, alive - 1);

        // When a round ends, increment for NEXT round
        if (alive == 0 && spawner.RemainingToSpawn <= 0)
        {
            round++;
        }
    }
}
