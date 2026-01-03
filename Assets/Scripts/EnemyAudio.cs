using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EnemyAudio : MonoBehaviour
{
    [Header("Idle Sounds")]
    public AudioClip[] idleClips;
    public float minIdleDelay = 3f;
    public float maxIdleDelay = 7f;

    [Header("Death Sounds")]
    public AudioClip[] deathClips;

    AudioSource source;
    bool dead;

    void Awake()
    {
        source = GetComponent<AudioSource>();

        source.spatialBlend = 1f;   // 3D sound
        source.playOnAwake = false;
        source.loop = false;
        source.minDistance = 3f;
        source.maxDistance = 20f;
    }

    void Start()
    {
        ScheduleNextIdle();
    }

    void ScheduleNextIdle()
    {
        if (dead || idleClips.Length == 0) return;

        float delay = Random.Range(minIdleDelay, maxIdleDelay);
        Invoke(nameof(PlayIdle), delay);
    }

    void PlayIdle()
    {
        if (dead || idleClips.Length == 0) return;

        AudioClip clip = idleClips[Random.Range(0, idleClips.Length)];
        source.PlayOneShot(clip);

        ScheduleNextIdle();
    }

    public void PlayDeath()
    {
        if (deathClips.Length == 0) return;

        AudioClip clip = deathClips[Random.Range(0, deathClips.Length)];

        AudioSource.PlayClipAtPoint(
            clip,
            transform.position,
            source ? source.volume : 1f
        );
    }


}
