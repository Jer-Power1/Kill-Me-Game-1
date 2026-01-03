using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDataResetter : MonoBehaviour
{
    public WeaponData[] allWeapons;

    void Awake()
    {
        foreach (var data in allWeapons)
        {
            if (!data) continue;

            data.upgraded = false;

            // IMPORTANT: reset to original values
            data.damage = data.baseDamage;
            data.fireRate = data.baseFireRate;
            data.magSize = data.baseMagSize;
            data.maxReserveAmmo = data.baseReserveAmmo;
        }
    }
}
