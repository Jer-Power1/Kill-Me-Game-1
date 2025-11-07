using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    [Tooltip("Leave empty to auto-use all direct children as weapons.")]
    public Transform[] weapons;      // each child holds a weapon script (BasicGun, ShotgunGun, etc.)
    public int startIndex = 0;
    public float switchCooldown = 0.15f;

    int current = -1;
    float nextSwitchTime;

    void Awake()
    {
        if (weapons == null || weapons.Length == 0)
        {
            weapons = new Transform[transform.childCount];
            for (int i = 0; i < weapons.Length; i++)
                weapons[i] = transform.GetChild(i);
        }

        // deactivate all first
        for (int i = 0; i < weapons.Length; i++)
            if (weapons[i]) weapons[i].gameObject.SetActive(false);

        Select(startIndex, instant: true);
    }


    void Update()
    {
        if (Time.time < nextSwitchTime || weapons.Length == 0) return;

        // Scroll wheel
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0.02f) Next();
        else if (scroll < -0.02f) Prev();

        // Number keys 1..9
        for (int i = 0; i < Mathf.Min(weapons.Length, 9); i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                Select(i);
                break;
            }
        }
    }

    void Next() => Select((current + 1) % weapons.Length);
    void Prev() => Select((current - 1 + weapons.Length) % weapons.Length);

    public void Select(int index, bool instant = false)
    {
        if (weapons.Length == 0) return;
        index = Mathf.Clamp(index, 0, weapons.Length - 1);
        if (index == current) return;

        // Holster old
        if (current >= 0 && current < weapons.Length && weapons[current])
            weapons[current].gameObject.SetActive(false);

        // Equip new
        current = index;
        if (weapons[current])
            weapons[current].gameObject.SetActive(true);

        nextSwitchTime = Time.time + (instant ? 0f : switchCooldown);
    }

    public Transform CurrentWeapon => (current >= 0 && current < weapons.Length) ? weapons[current] : null;
    public int CurrentIndex => current;
}
