using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AmmoHUD : MonoBehaviour
{
    public WeaponSwitcher switcher;
    public TMP_Text label;

    void Reset() { label = GetComponent<TMP_Text>(); }

    void LateUpdate()
    {
        if (!switcher || !label) return;
        var w = switcher.CurrentWeapon;
        if (!w) { label.text = ""; return; }

        var gun = w.GetComponent<WeaponController>();
        if (!gun || gun.data == null) { label.text = ""; return; }

        label.text = gun.IsReloading ? "RELOADING…" : $"{gun.CurrentAmmo}/{gun.MagSize}";
    }
}

