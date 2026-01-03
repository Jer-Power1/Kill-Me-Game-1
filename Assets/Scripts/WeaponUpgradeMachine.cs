using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WeaponUpgradeMachine : MonoBehaviour
{
    public int cost = 5000;
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
            TryUpgrade();
    }

    void TryUpgrade()
    {
        var switcher = FindObjectOfType<WeaponSwitcher>();
        if (!switcher || !switcher.CurrentWeapon) return;

        var gun = switcher.CurrentWeapon.GetComponent<WeaponController>();
        if (!gun || !gun.data) return;

        if (gun.data.upgraded)
        {
            Debug.Log("Weapon already upgraded");
            return;
        }

        if (!PointsManager.Instance || !PointsManager.Instance.SpendPoints(cost))
        {
            Debug.Log("Not enough points");
            return;
        }

        gun.ApplyUpgrade();
        Debug.Log($"Upgraded {gun.data.weaponName}");
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
