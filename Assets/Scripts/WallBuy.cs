using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WallBuy : MonoBehaviour
{
    [Header("Weapon")]
    public GameObject weaponToBuy;
    public int cost = 1000;

    [Header("UI")]
    public TMP_Text promptText;

    bool playerInRange;

    void Start()
    {
        if (promptText)
            promptText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!playerInRange) return;

        if (Input.GetKeyDown(KeyCode.E))
            TryBuyWeapon();
    }

    void TryBuyWeapon()
    {
        if (!weaponToBuy) return;
        if (!PointsManager.Instance) return;

        var switcher = FindObjectOfType<WeaponSwitcher>();
        if (!switcher) return;

        // If already unlocked, just switch to it
        if (switcher.IsUnlocked(weaponToBuy.transform))
        {
            switcher.SelectWeapon(weaponToBuy.transform);
            return;
        }

        // Try to buy
        if (!PointsManager.Instance.SpendPoints(cost))
        {
            Debug.Log("Not enough points");
            return;
        }

        // Unlock and equip
        switcher.UnlockWeapon(weaponToBuy.transform, equip: true);
        Debug.Log($"Purchased {weaponToBuy.name}");
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = true;
        if (promptText)
            promptText.gameObject.SetActive(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = false;
        if (promptText)
            promptText.gameObject.SetActive(false);
    }
}
