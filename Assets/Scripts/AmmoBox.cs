using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AmmoBox : MonoBehaviour
{
    [Header("Settings")]
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
            TryBuyAmmo();
    }

    void TryBuyAmmo()
    {
        if (!PointsManager.Instance) return;

        if (!PointsManager.Instance.SpendPoints(cost))
        {
            Debug.Log("Not enough points to buy ammo");
            return;
        }

        var switcher = FindObjectOfType<WeaponSwitcher>();
        if (!switcher || !switcher.CurrentWeapon) return;

        var gun = switcher.CurrentWeapon.GetComponent<WeaponController>();
        if (!gun) return;

        gun.RefillAmmo();
        Debug.Log("Ammo purchased");
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
