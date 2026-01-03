using UnityEngine;
using System.Collections.Generic;

public class WeaponSwitcher : MonoBehaviour
{
    [Tooltip("Leave empty to auto-use all direct children as weapons.")]
    public Transform[] weapons;

    public int startIndex = 0;
    public float switchCooldown = 0.15f;

    int current = -1;
    float nextSwitchTime;

    // Track which weapons are unlocked
    HashSet<Transform> unlocked = new HashSet<Transform>();

    void Awake()
    {
        if (weapons == null || weapons.Length == 0)
        {
            weapons = new Transform[transform.childCount];
            for (int i = 0; i < weapons.Length; i++)
                weapons[i] = transform.GetChild(i);
        }

        // Lock everything first
        foreach (var w in weapons)
        {
            if (w) w.gameObject.SetActive(false);
        }

        // Unlock starting weapon
        if (startIndex >= 0 && startIndex < weapons.Length)
        {
            UnlockWeapon(weapons[startIndex], equip: true);
        }
    }

    void Update()
    {
        if (Time.time < nextSwitchTime) return;

        if (unlocked.Count <= 1) return;

        // Scroll wheel
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0.02f) SelectNext();
        else if (scroll < -0.02f) SelectPrevious();

        // Number keys
        for (int i = 0; i < Mathf.Min(weapons.Length, 9); i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                if (unlocked.Contains(weapons[i]))
                    SelectWeapon(weapons[i]);
            }
        }
    }

    // ===== PUBLIC API FOR WALL BUYS =====

    public void UnlockWeapon(Transform weapon, bool equip = false)
    {
        if (!weapon || unlocked.Contains(weapon)) return;

        unlocked.Add(weapon);
        weapon.gameObject.SetActive(false);

        if (equip)
            SelectWeapon(weapon);
    }

    public void SelectWeapon(Transform weapon)
    {
        if (!weapon || !unlocked.Contains(weapon)) return;

        // Holster current
        if (current >= 0 && current < weapons.Length && weapons[current])
            weapons[current].gameObject.SetActive(false);

        current = System.Array.IndexOf(weapons, weapon);
        weapon.gameObject.SetActive(true);

        nextSwitchTime = Time.time + switchCooldown;
    }

    // ===== INTERNAL SWITCHING =====

    void SelectNext()
    {
        SelectByOffset(1);
    }

    void SelectPrevious()
    {
        SelectByOffset(-1);
    }

    void SelectByOffset(int dir)
    {
        if (unlocked.Count == 0) return;

        int idx = current;
        for (int i = 0; i < weapons.Length; i++)
        {
            idx = (idx + dir + weapons.Length) % weapons.Length;
            if (unlocked.Contains(weapons[idx]))
            {
                SelectWeapon(weapons[idx]);
                return;
            }
        }
    }

    // ===== READ-ONLY ACCESS =====

    public Transform CurrentWeapon =>
        (current >= 0 && current < weapons.Length) ? weapons[current] : null;

    public bool IsUnlocked(Transform weapon) => unlocked.Contains(weapon);
}
