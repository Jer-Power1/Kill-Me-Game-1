using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitMarker : MonoBehaviour
{
    public Image marker;     // the small cross/dot image
    public float showTime = 0.08f;
    float timer;

    void Awake()
    {
        if (!marker) marker = GetComponent<Image>();
        if (marker) marker.enabled = false;
    }

    public void Ping()
    {
        timer = showTime;
        if (marker) marker.enabled = true;
    }

    void Update()
    {
        if (!marker) return;
        if (timer > 0f)
        {
            timer -= Time.unscaledDeltaTime;
            if (timer <= 0f) marker.enabled = false;
        }
    }
}

