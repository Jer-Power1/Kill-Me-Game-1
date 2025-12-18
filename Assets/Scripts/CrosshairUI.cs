using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairUI : MonoBehaviour
{
    public WeaponSwitcher switcher;   // drag GunPivot here
    public Image img;                 // Crosshair UI Image
    public Sprite defaultSprite;

    WeaponController lastGun;

    void Awake()
    {
        if (!img) img = GetComponent<Image>();
        if (img) img.raycastTarget = false;
    }

    void LateUpdate()
    {
        if (!switcher || !img) return;

        var w = switcher.CurrentWeapon;
        var gun = w ? w.GetComponent<WeaponController>() : null;

        if (gun == lastGun) return;
        lastGun = gun;

        if (gun && gun.data && gun.data.crosshairSprite)
        {
            img.sprite = gun.data.crosshairSprite;
            img.enabled = true;
        }
        else
        {
            img.sprite = defaultSprite;
            img.enabled = defaultSprite != null;
        }
    }
}
