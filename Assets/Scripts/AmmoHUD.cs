using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AmmoHUD : MonoBehaviour
{
    public WeaponSwitcher switcher;
    public TMP_Text label;
    public string reloadingText = "RELOADING…";

    void Reset() { label = GetComponent<TMP_Text>(); }

    void LateUpdate() // run after most logic & switcher updates
    {
        if (!switcher || !label) return;

        var w = switcher.CurrentWeapon;
        if (!w) { label.text = ""; return; }

        // read whichever weapon is active
        var pistol = w.GetComponent<BasicGun>();
        var shotgun = w.GetComponent<ShotgunGun>();

        if (pistol)
        {
            label.text = pistol.IsReloading
                ? reloadingText
                : $"{pistol.CurrentAmmo}/{pistol.MagSize}";
            return;
        }

        if (shotgun)
        {
            label.text = shotgun.IsReloading
                ? reloadingText
                : $"{shotgun.CurrentAmmo}/{shotgun.MagSize}";
            return;
        }

        label.text = "";
    }
}

