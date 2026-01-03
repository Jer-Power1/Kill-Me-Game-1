using UnityEngine;

public enum FireMode { SemiAuto, FullAuto }

[CreateAssetMenu(menuName = "FPS/Weapon Data")]
public class WeaponData : ScriptableObject
{
    void OnEnable()
    {
        // Capture base values once
        if (baseDamage <= 0f)
        {
            baseDamage = damage;
            baseFireRate = fireRate;
            baseMagSize = magSize;
            baseReserveAmmo = maxReserveAmmo;
        }
    }

    [Header("Identity")]
    public string weaponName = "Weapon";

    [Header("Core")]
    public FireMode fireMode = FireMode.SemiAuto;
    [Tooltip("Shots per second")]
    public float fireRate = 6f;
    public float damage = 20f;
    public float range = 120f;

    [Header("Ammo")]
    public int magSize = 30;
    public int maxReserveAmmo = 120;
    public float reloadTime = 1.2f;

    [Header("Hitscan / Shotgun")]
    [Tooltip("1 = normal hitscan. >1 = shotgun pellets.")]
    public int pellets = 1;
    [Tooltip("Cone spread in degrees (0 = perfectly accurate).")]
    public float spreadDegrees = 0.5f;

    [Header("Kick / Feedback")]
    public float recoilKick = 0.6f;
    public AudioClip shotClip;
    public AudioClip dryClip;
    public AudioClip reloadClip;

    [Header("Impact (optional)")]
    public GameObject hitVfx;

    [Header("UI")]
    public Sprite crosshairSprite;
    public Vector2 crosshairSize = new Vector2(24, 24);

    [Header("VFX")]
    public GameObject muzzleFlashPrefab;
    public float muzzleFlashLifetime = 0.05f;

    [Header("Upgrade")]
    public bool upgraded;

    // Stat upgrades
    public float damageMultiplier = 5f;
    public float fireRateMultiplier = 2f;
    public int magBonus = 10;
    public int reserveBonus = 60;

    // Visual upgrades
    public Material upgradedMaterial;
    public GameObject upgradedMuzzleFlash;
    public GameObject upgradedHitVfx;

    [Header("Base Values (do not edit at runtime)")]
    public float baseDamage;
    public float baseFireRate;
    public int baseMagSize;
    public int baseReserveAmmo;

}
