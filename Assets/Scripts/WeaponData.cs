using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FireMode { SemiAuto, FullAuto }

[CreateAssetMenu(menuName = "FPS/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("Identity")]
    public string weaponName = "Weapon";

    [Header("Core")]
    public FireMode fireMode = FireMode.SemiAuto;
    [Tooltip("Shots per second")]
    public float fireRate = 6f;
    public float damage = 20f;
    public float range = 120f;

    [Header("Magazine")]
    public int magSize = 12;
    public float reloadTime = 1.2f;

    [Header("Hitscan / Shotgun")]
    [Tooltip("1 = normal hitscan. >1 = shotgun pellets.")]
    public int pellets = 1;
    [Tooltip("Cone spread in degrees (0 = perfectly accurate).")]
    public float spreadDegrees = 0.5f;

    [Header("Kick / Feedback")]
    public float recoilKick = 0.6f; // your recoil script can use this
    public AudioClip shotClip;
    public AudioClip dryClip;
    public AudioClip reloadClip;

    [Header("Impact (optional)")]
    public GameObject hitVfx; // sparks/blood etc.

    [Header("UI")]
    public Sprite crosshairSprite;
    public Vector2 crosshairSize = new Vector2(24, 24); // optional

}

