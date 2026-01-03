using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    public float maxHealth = 40f;

    float hp;
    bool dead;

    public event Action onDied;

    void Awake()
    {
        hp = maxHealth;
    }

    /// <summary>
    /// Call this ON SPAWN to apply round scaling correctly
    /// </summary>
    public void ApplyHealthMultiplier(float multiplier)
    {
        maxHealth *= multiplier;
        hp = maxHealth; // IMPORTANT: reset current health
    }

    public void TakeDamage(float amount, Vector3 hitPoint, Vector3 hitNormal, GameObject source)
    {
        if (dead) return;

        hp -= amount;
        if (hp <= 0f)
            Die();
    }
    void Die()
    {
        if (dead) return;
        dead = true;

        // Play death sound (detached)
        var audio = GetComponent<EnemyAudio>();
        if (audio)
            audio.PlayDeath();

        // Award points immediately
        if (PointsManager.Instance)
            PointsManager.Instance.AddPoints(100);

        // Notify round manager immediately
        onDied?.Invoke();

        // Destroy enemy immediately
        Destroy(gameObject);
    }

}
