using System.Collections;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Data")]
    public WeaponData data;

    [Header("Refs")]
    public Camera cam;
    public AudioSource audioSource;
    public SimpleRecoil recoil;
    public HitMarker hitMarker;
    public Transform muzzlePoint;

    [Header("Layers")]
    public LayerMask hitMask = ~0;

    float nextShotTime;
    bool reloading;

    int ammoInMag;
    int reserveAmmo;

    // ===== UI / External Access =====
    public int CurrentAmmo => ammoInMag;
    public int ReserveAmmo => reserveAmmo;
    public int MagSize => data ? data.magSize : 0;
    public bool IsReloading => reloading;
    public string WeaponName => data ? data.weaponName : "";

    // ===== SETUP =====
    void Awake()
    {
        if (!cam) cam = Camera.main;

        if (!audioSource)
        {
            audioSource = GetComponent<AudioSource>();
            if (!audioSource && cam)
                audioSource = cam.GetComponent<AudioSource>();
        }

        if (audioSource)
        {
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f;
            audioSource.mute = false;
            if (audioSource.volume <= 0.01f)
                audioSource.volume = 1f;
        }

        Equip(data);
    }

    void OnEnable()
    {
        reloading = false;
        nextShotTime = 0f;
    }

    // ===== EQUIP =====
    public void Equip(WeaponData newData)
    {
        data = newData;
        if (!data) return;

        ammoInMag = data.magSize;
        reserveAmmo = data.maxReserveAmmo;
        reloading = false;
    }

    // ===== INPUT =====
    void Update()
    {
        if (!data) return;

        if (Input.GetKeyDown(KeyCode.R))
            StartCoroutine(Reload());

        if (data.fireMode == FireMode.SemiAuto)
        {
            if (Input.GetButtonDown("Fire1"))
                TryShoot();
        }
        else // Full-auto
        {
            if (Input.GetButton("Fire1"))
                TryShoot();
        }
    }

    // ===== SHOOTING =====
    void TryShoot()
    {
        if (reloading) return;
        if (Time.time < nextShotTime) return;

        if (ammoInMag <= 0)
        {
            if (data.dryClip && audioSource)
                audioSource.PlayOneShot(data.dryClip);

            nextShotTime = Time.time + 0.2f;
            return;
        }

        ammoInMag--;
        nextShotTime = Time.time + 1f / Mathf.Max(0.01f, data.fireRate);

        SpawnMuzzleFlash();

        if (data.shotClip && audioSource)
            audioSource.PlayOneShot(data.shotClip);

        if (recoil)
            recoil.Kick(data.recoilKick);

        int pellets = Mathf.Max(1, data.pellets);
        for (int i = 0; i < pellets; i++)
            ShootOneRay();
    }

    void ShootOneRay()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        Vector3 dir = ApplySpread(ray.direction, data.spreadDegrees);

        if (Physics.Raycast(ray.origin, dir, out RaycastHit hit, data.range, hitMask, QueryTriggerInteraction.Ignore))
        {
            var dmg = hit.collider.GetComponent<IDamageable>() ??
                      hit.collider.GetComponentInParent<IDamageable>();

            if (dmg != null)
            {
                dmg.TakeDamage(data.damage, hit.point, hit.normal, gameObject);
                if (hitMarker) hitMarker.Ping();
            }

            GameObject vfx = data.upgraded && data.upgradedHitVfx
    ? data.upgradedHitVfx
    : data.hitVfx;

            if (vfx)
                Destroy(Instantiate(vfx, hit.point, Quaternion.LookRotation(hit.normal)), 2f);

        }
    }

    Vector3 ApplySpread(Vector3 forward, float degrees)
    {
        if (degrees <= 0.001f) return forward;

        float rad = degrees * Mathf.Deg2Rad;
        Vector2 r = Random.insideUnitCircle * Mathf.Tan(rad);

        return (forward +
                r.x * cam.transform.right +
                r.y * cam.transform.up).normalized;
    }

    // ===== RELOAD =====
    IEnumerator Reload()
    {
        if (reloading) yield break;
        if (!data) yield break;
        if (ammoInMag >= data.magSize) yield break;
        if (reserveAmmo <= 0) yield break;

        reloading = true;

        if (data.reloadClip && audioSource)
            audioSource.PlayOneShot(data.reloadClip);

        yield return new WaitForSeconds(data.reloadTime);

        int needed = data.magSize - ammoInMag;
        int taken = Mathf.Min(needed, reserveAmmo);

        ammoInMag += taken;
        reserveAmmo -= taken;

        reloading = false;
    }

    // ===== AMMO BUY =====
    public void RefillAmmo()
    {
        if (!data) return;
        reserveAmmo = data.maxReserveAmmo;
    }

    // ===== MUZZLE FLASH =====
    void SpawnMuzzleFlash()
    {
        if (!data || !muzzlePoint)
            return;

        GameObject flashPrefab = data.upgraded && data.upgradedMuzzleFlash
            ? data.upgradedMuzzleFlash
            : data.muzzleFlashPrefab;

        if (!flashPrefab) return;

        GameObject flash = Instantiate(
            flashPrefab,
            muzzlePoint.position,
            muzzlePoint.rotation,
            muzzlePoint
        );

        Destroy(flash, data.muzzleFlashLifetime);
    }


    public void ApplyUpgrade()
    {
        if (!data || data.upgraded)
            return;

        data.upgraded = true;

        // ===== STAT UPGRADES =====
        data.damage *= data.damageMultiplier;
        data.fireRate *= data.fireRateMultiplier;
        data.magSize += data.magBonus;
        data.maxReserveAmmo += data.reserveBonus;

        // Refill ammo to new capacity
        ammoInMag = data.magSize;
        reserveAmmo = data.maxReserveAmmo;

        // ===== VISUAL UPGRADE =====
        if (data.upgradedMaterial)
        {
            var renderers = GetComponentsInChildren<MeshRenderer>();
            foreach (var r in renderers)
                r.material = data.upgradedMaterial;
        }

        Debug.Log($"{data.weaponName} upgraded!");
    }


}
