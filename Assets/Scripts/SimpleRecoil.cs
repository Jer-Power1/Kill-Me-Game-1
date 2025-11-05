using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRecoil : MonoBehaviour
{
    public float returnSpeed = 12f;  // how quickly it settles
    public float maxKick = 10f;      // clamp
    float pitchKick;

    void LateUpdate()
    {
        pitchKick = Mathf.Lerp(pitchKick, 0f, returnSpeed * Time.deltaTime);
        var local = transform.localEulerAngles;
        if (local.x > 180f) local.x -= 360f;   // convert to -180..180
        local.x = Mathf.Clamp(local.x - pitchKick, -89f, 89f);
        transform.localEulerAngles = local;
    }

    public void Kick(float amount)
    {
        pitchKick = Mathf.Clamp(pitchKick + amount, 0f, maxKick);
    }
}
