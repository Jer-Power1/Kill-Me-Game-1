using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PointsManager : MonoBehaviour
{
    public static PointsManager Instance;

    [Header("Settings")]
    public int pointsPerKill = 5000;

    [Header("UI")]
    public TMP_Text pointsText;

    [Header("Audio")]
    public AudioClip purchaseClip;
    public AudioClip deniedClip;
    AudioSource audioSource;

    int points;

    void Awake()
    {
        // Singleton (safe version)
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        UpdateUI();
    }

    public void AddPoints(int amount)
    {
        points += amount;
        UpdateUI();
    }

    public bool CanAfford(int cost)
    {
        return points >= cost;
    }

    public bool SpendPoints(int amount)
    {
        if (points < amount)
        {
            if (deniedClip && audioSource)
                audioSource.PlayOneShot(deniedClip);

            return false;
        }

        points -= amount;
        UpdateUI();

        if (purchaseClip && audioSource)
            audioSource.PlayOneShot(purchaseClip);

        return true;
    }


    void UpdateUI()
    {
        if (pointsText)
            pointsText.text = points.ToString();
    }

    public int CurrentPoints => points;
}
