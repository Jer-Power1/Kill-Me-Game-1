
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class HitscanGun : MonoBehaviour
{
    [Header("Refs")]
    public Camera cam;                         // typically the player camera
    public ParticleSystem muzzleFlash;         // optional
    public Transform impactVfxPrefab;          // optional: small spark/dust prefab
    public AudioSource audioSource;            // put on the gun
    public AudioClip shotClip, dryClip, reloadClip;

    [Header("Stats")]
    public float damage = 25f;
    public float range = 120f;
    public float fireRate = 10f;               // shots per second
    public int magSize = 20;
    public float reloadTime = 1.2f;

    [Header("Filtering")]
    public LayerMask hitMask = ~0;             // default: hit everything
    public QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore;

    [Header("Recoil (optional)")]
    public SimpleRecoil recoil;                // drag the component below if you use it
    public float recoilKick = 1.2f;

    int ammoInMag;
    float nextShotTime;
    bool reloading;

    void Start()
    {
        if (!cam) cam = Camera.main;
        if (!audioSource) audioSource = GetComponent<AudioSource>();
        ammoInMag = magSize;

        // If you want to ignore the Player layer by default:
        // int playerLayer = LayerMask.NameToLayer("Player");
        // hitMask &= ~(1 << playerLayer);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) StartCoroutine(Reload());
        if (Input.GetButton("Fire1")) TryShoot();
    }

    void TryShoot()
    {
        if (reloading) return;
        if (Time.time < nextShotTime) return;

        if (ammoInMag <= 0)
        {
            if (dryClip) audioSource.PlayOneShot(dryClip);
            nextShotTime = Time.time + 0.25f;
            return;
        }

        Shoot();
    }

    void Shoot()
    {
        nextShotTime = Time.time + 1f / fireRate;
        ammoInMag--;

        if (muzzleFlash) muzzleFlash.Play();
        if (shotClip) audioSource.PlayOneShot(shotClip);
        if (recoil) recoil.Kick(recoilKick);

        // Center-screen ray (like DOOM—no bullet drop)
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out RaycastHit hit, range, hitMask, triggerInteraction))
        {
            // Damage hook
            var dmg = hit.collider.GetComponent<IDamageable>();
            if (dmg != null)
                dmg.TakeDamage(damage, hit.point, hit.normal, hit.collider.gameObject);

            // Impact VFX
            if (impactVfxPrefab)
            {
                var fx = Instantiate(impactVfxPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(fx.gameObject, 2f);
            }
        }
    }

    IEnumerator Reload()
    {
        if (reloading || ammoInMag == magSize) yield break;
        reloading = true;
        if (reloadClip) audioSource.PlayOneShot(reloadClip);
        yield return new WaitForSeconds(reloadTime);
        ammoInMag = magSize;
        reloading = false;
    }

    // Optional public getters for UI
    public int CurrentAmmo => ammoInMag;
    public int MagSize => magSize;
}
