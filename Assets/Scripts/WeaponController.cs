using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Data")]
    public WeaponData data;

    [Header("Refs")]
    public Camera cam;
    public ParticleSystem muzzleFlash;
    public AudioSource audioSource;
    public SimpleRecoil recoil; // optional
    public HitMarker hitMarker; // optional

    [Header("Layers")]
    public LayerMask hitMask = ~0;

    float nextShotTime;
    bool reloading;
    int ammoInMag;

    // Expose for AmmoHUD
    public int CurrentAmmo => ammoInMag;
    public int MagSize => data ? data.magSize : 0;
    public bool IsReloading => reloading;
    public string WeaponName => data ? data.weaponName : "";

    void Awake()
    {
        if (!cam) cam = Camera.main;

        if (!audioSource)
        {
            audioSource = GetComponent<AudioSource>();
            if (!audioSource && cam) audioSource = cam.GetComponent<AudioSource>();
        }

        if (!audioSource)
            Debug.LogError($"[{name}] No AudioSource found! Add one to the weapon or camera.");

        if (audioSource)
        {
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f; // 2D
            audioSource.mute = false;
            if (audioSource.volume <= 0.01f) audioSource.volume = 1f;
        }

        Equip(data);
    }


    void OnEnable()
    {
        // when switching weapons, stop any reload
        reloading = false;
        nextShotTime = 0f;
    }

    public void Equip(WeaponData newData)
    {
        data = newData;
        if (!data) return;

        ammoInMag = data.magSize;
        reloading = false;
    }

    void Update()
    {
        if (!data) return;

        if (Input.GetKeyDown(KeyCode.R))
            StartCoroutine(Reload());

        // Semi-auto: only on button down
        if (data.fireMode == FireMode.SemiAuto)
        {
            if (Input.GetButtonDown("Fire1")) TryShoot();
        }
        // Full-auto: held
        else
        {
            if (Input.GetButton("Fire1")) TryShoot();
        }
        if (!audioSource) audioSource = cam.GetComponent<AudioSource>();

    }

    void TryShoot()
    {
        if (reloading) return;
        if (Time.time < nextShotTime) return;

        // no ammo -> dry click
        if (ammoInMag <= 0)
        {
            if (data.dryClip && audioSource) audioSource.PlayOneShot(data.dryClip);
            nextShotTime = Time.time + 0.2f;
            return;
        }

        nextShotTime = Time.time + 1f / Mathf.Max(0.01f, data.fireRate);
        ammoInMag--;

        if (muzzleFlash) muzzleFlash.Play();
        if (data.shotClip && audioSource) audioSource.PlayOneShot(data.shotClip);
        if (recoil) recoil.Kick(data.recoilKick);

        // Hitscan / pellets
        int pellets = Mathf.Max(1, data.pellets);
        for (int i = 0; i < pellets; i++)
        {
            ShootOneRay();
        }
    }

    void ShootOneRay()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        Vector3 dir = ApplySpread(ray.direction, data.spreadDegrees);

        if (Physics.Raycast(ray.origin, dir, out RaycastHit hit, data.range, hitMask, QueryTriggerInteraction.Ignore))
        {
            // Damage only if enemy implements IDamageable
            var dmg = hit.collider.GetComponent<IDamageable>() ?? hit.collider.GetComponentInParent<IDamageable>();
            if (dmg != null)
            {
                dmg.TakeDamage(data.damage, hit.point, hit.normal, gameObject);
                if (hitMarker) hitMarker.Ping(); // only on enemy hit
            }

            // optional impact VFX
            if (data.hitVfx)
                Destroy(Instantiate(data.hitVfx, hit.point, Quaternion.LookRotation(hit.normal)), 2f);
        }
    }

    Vector3 ApplySpread(Vector3 forward, float degrees)
    {
        if (degrees <= 0.001f) return forward;

        float rad = degrees * Mathf.Deg2Rad;
        Vector2 r = Random.insideUnitCircle * Mathf.Tan(rad);

        Vector3 up = cam.transform.up;
        Vector3 right = cam.transform.right;
        return (forward + r.x * right + r.y * up).normalized;
    }

    System.Collections.IEnumerator Reload()
    {
        if (reloading) yield break;
        if (!data) yield break;
        if (ammoInMag >= data.magSize) yield break;

        reloading = true;
        if (data.reloadClip && audioSource) audioSource.PlayOneShot(data.reloadClip);
        yield return new WaitForSeconds(data.reloadTime);
        ammoInMag = data.magSize;
        reloading = false;
    }
}
