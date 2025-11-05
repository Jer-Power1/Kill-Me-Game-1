using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class DamageOverlayUI : MonoBehaviour
{
    [Header("Refs")]
    public PlayerHealth player;      // drag your Player root here
    public Image overlay;            // auto-filled

    [Header("Effect")]
    [Tooltip("Alpha when HP = full (usually 0)")]
    public float minAlphaAtFullHP = 0f;
    [Tooltip("Alpha when HP = 0 (how red it gets)")]
    public float maxAlphaAtZeroHP = 0.65f;
    [Tooltip("Smoothing time for fade (seconds)")]
    public float smoothTime = 0.12f;

    float _vel;
    float _currentAlpha;

    void Awake()
    {
        if (!overlay) overlay = GetComponent<Image>();
        // Make sure clicks pass through
        if (overlay) overlay.raycastTarget = false;
        if (!player)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.GetComponent<PlayerHealth>();
        }
    }

    void Update()
    {
        if (!player || !overlay) return;

        float frac = Mathf.Clamp01(player.CurrentHealth / Mathf.Max(1f, player.MaxHealth)); // 1=full, 0=dead
        float targetAlpha = Mathf.Lerp(maxAlphaAtZeroHP, minAlphaAtFullHP, frac);

        _currentAlpha = Mathf.SmoothDamp(_currentAlpha, targetAlpha, ref _vel, smoothTime);

        var c = overlay.color;
        c.a = _currentAlpha;
        overlay.color = c;
    }

    // Optional: call this briefly on damage to add a tiny pulse
    public void Pulse(float extra = 0.1f)
    {
        _currentAlpha = Mathf.Clamp01(_currentAlpha + extra);
    }
}
