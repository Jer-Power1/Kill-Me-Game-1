using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("Health")]
    public float maxHealth = 60f;

    [Header("Flash (optional)")]
    public HitFlash hitFlash; // assign in Inspector or we auto-find

    [Header("Death")]
    public GameObject deathVfx;

    float hp;

    void Awake()
    {
        hp = maxHealth;
        if (!hitFlash) hitFlash = GetComponentInChildren<HitFlash>(true);
    }

    public void TakeDamage(float amount, Vector3 hitPoint, Vector3 hitNormal, GameObject source)
    {
        hp -= amount;
        if (hitFlash) hitFlash.Flash();

        if (TryGetComponent<Rigidbody>(out var rb))
            rb.AddForceAtPosition(-hitNormal * 4f, hitPoint, ForceMode.Impulse);

        if (hp <= 0f) Die();
    }

    void Die()
    {
        if (deathVfx)
            Destroy(Instantiate(deathVfx, transform.position, Quaternion.identity), 2f);
        FindObjectOfType<EnemySpawner>()?.NotifyEnemyDied(gameObject);
        Destroy(gameObject);
    }
}
