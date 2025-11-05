using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;
    public float CurrentHealth { get; private set; }
    public float MaxHealth => maxHealth;

    [Header("Regen")]
    public float regenDelay = 2.0f;   // seconds after last damage before regen starts
    public float regenRate = 12.0f;  // HP per second while regenerating

    float lastDamageTime;
    bool isDead;

    void Awake()
    {
        CurrentHealth = maxHealth;
        lastDamageTime = -999f; // so we don't instantly regen on start
    }

    void Update()
    {
        if (isDead) return;

        bool canRegen = (Time.time - lastDamageTime) >= regenDelay && CurrentHealth < maxHealth;
        if (canRegen)
        {
            CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + regenRate * Time.deltaTime);
        }
    }

    public void TakeDamage(float amount)
    {
        if (isDead || amount <= 0f) return;

        CurrentHealth = Mathf.Max(0f, CurrentHealth - amount);
        lastDamageTime = Time.time;

        // Optional: quick visual bump on hit (safe even if overlay not present)
        FindObjectOfType<DamageOverlayUI>()?.Pulse(0.12f);

        if (CurrentHealth <= 0f) Die();
    }

    public void Heal(float amount)
    {
        if (isDead || amount <= 0f) return;
        CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + amount);
        // Healing doesn't reset the regen delay
    }

    void Die()
    {
        isDead = true;
        Debug.Log("PLAYER DIED");
        var deathUI = FindObjectOfType<DeathScreenUI>(true); // true finds inactive objects too
        if (deathUI) deathUI.Show();
        // Optional: also disable your movement/shooting scripts here if you don’t pause with Time.timeScale
    }
}
