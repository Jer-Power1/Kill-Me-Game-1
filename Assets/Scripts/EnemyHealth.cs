using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    public float maxHealth = 60f;
    public HitFlash hitFlash;
    public GameObject deathVfx;

    public event Action onDied;

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
        if (hp <= 0f) Die();
    }

    void Die()
    {
        onDied?.Invoke();
        if (deathVfx) Destroy(Instantiate(deathVfx, transform.position, Quaternion.identity), 2f);
        Destroy(gameObject);
    }
}
