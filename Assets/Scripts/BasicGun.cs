using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // only needed if you hook up ammoText

public class BasicGun : MonoBehaviour
{
    [Header("Refs")]
    public Camera cam;
    public ParticleSystem muzzleFlash;
    public AudioSource audioSource;
    public AudioClip shotClip, dryClip, reloadClip;
    public TMPro.TMP_Text ammoText; // if you use TextMeshPro
    public HitMarker hitMarker;

    [Header("Gun Settings")]
    public float damage = 10f;
    public float range = 100f;
    public float fireRate = 5f;
    public int magSize = 20;
    public float reloadTime = 1.2f;

    [Header("Layers")]
    public LayerMask hitMask = ~0;

    private float nextShotTime;
    private int ammoInMag;
    private bool reloading;

    void Awake()
    {
        if (!cam) cam = Camera.main;
        if (!hitMarker) hitMarker = FindObjectOfType<HitMarker>(true);
        ammoInMag = magSize;
        UpdateAmmoUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) StartCoroutine(Reload());
        if (Input.GetButton("Fire1")) TryShoot();
    }

    void TryShoot()
    {
        if (reloading || Time.time < nextShotTime) return;

        if (ammoInMag <= 0)
        {
            if (dryClip && audioSource) audioSource.PlayOneShot(dryClip);
            nextShotTime = Time.time + 0.25f;
            return;
        }

        nextShotTime = Time.time + 1f / fireRate;
        ammoInMag--;
        UpdateAmmoUI();

        if (muzzleFlash) muzzleFlash.Play();
        if (shotClip && audioSource) audioSource.PlayOneShot(shotClip);

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        if (Physics.Raycast(ray, out RaycastHit hit, range, hitMask, QueryTriggerInteraction.Ignore))
        {
            // try collider, then parent, for flexibility
            var dmg = hit.collider.GetComponent<IDamageable>()
                      ?? hit.collider.GetComponentInParent<IDamageable>();

            if (dmg != null)
            {
                dmg.TakeDamage(damage, hit.point, hit.normal, gameObject);

                // ✅ only ping when we actually hit an enemy
                if (hitMarker) hitMarker.Ping();
            }

            // optional physics shove for non-enemy objects
            if (hit.rigidbody) hit.rigidbody.AddForce(-hit.normal * 5f, ForceMode.Impulse);
        }
    }

    System.Collections.IEnumerator Reload()
    {
        if (reloading || ammoInMag == magSize) yield break;

        reloading = true;
        if (reloadClip && audioSource) audioSource.PlayOneShot(reloadClip);
        yield return new WaitForSeconds(reloadTime);
        ammoInMag = magSize;
        reloading = false;
        UpdateAmmoUI();
    }

    void UpdateAmmoUI()
    {
        if (ammoText) ammoText.text = $"{ammoInMag}/{magSize}";
    }

    public int CurrentAmmo => ammoInMag;
    public int MagSize => magSize;
    public bool IsReloading => reloading;

}