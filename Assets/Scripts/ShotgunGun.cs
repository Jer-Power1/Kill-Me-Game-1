using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunGun : MonoBehaviour
{
    [Header("Refs")]
    public Camera cam;
    public ParticleSystem muzzleFlash;
    public AudioSource audioSource;
    public AudioClip shotClip, dryClip, reloadClip;

    [Header("Stats")]
    public int pellets = 8;
    public float spreadDegrees = 4f;
    public float damagePerPellet = 8f;
    public float range = 60f;
    public float fireRate = 1.0f;     // shots per second
    public int magSize = 6;
    public float reloadTime = 1.0f;

    public LayerMask hitMask = ~0;

    float nextShotTime;
    int ammoInMag;
    bool reloading;

    void Awake()
    {
        if (!cam) cam = Camera.main;
        ammoInMag = magSize;
    }

    void OnEnable() { reloading = false; } // cancel reload if we switched away

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

        if (muzzleFlash) muzzleFlash.Play();
        if (shotClip && audioSource) audioSource.PlayOneShot(shotClip);

        Vector3 origin = cam.transform.position;
        Vector3 forward = cam.transform.forward;

        for (int i = 0; i < pellets; i++)
        {
            Vector3 dir = SpreadDirection(forward, spreadDegrees);
            if (Physics.Raycast(origin, dir, out RaycastHit hit, range, hitMask, QueryTriggerInteraction.Ignore))
            {
                var dmg = hit.collider.GetComponent<IDamageable>() ?? hit.collider.GetComponentInParent<IDamageable>();
                if (dmg != null)
                    dmg.TakeDamage(damagePerPellet, hit.point, hit.normal, gameObject);

                if (hit.rigidbody) hit.rigidbody.AddForce(-hit.normal * 3f, ForceMode.Impulse);
            }
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
    }

    // random cone around forward
    Vector3 SpreadDirection(Vector3 forward, float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        Vector2 r = Random.insideUnitCircle * Mathf.Tan(rad);
        Vector3 up = cam.transform.up;
        Vector3 right = cam.transform.right;
        Vector3 dir = (forward + r.x * right + r.y * up).normalized;
        return dir;
    }

    public int CurrentAmmo => ammoInMag;
    public int MagSize => magSize;
    public bool IsReloading => reloading;

}
