using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class HitFlash : MonoBehaviour
{
    [Header("Flash")]
    public Color flashColor = Color.red;
    public float flashHold = 0.05f;     // time to hold flash color
    public float fadeBackTime = 0.12f;  // time to fade back

    // common color property names across shaders
    static readonly int ID_Color = Shader.PropertyToID("_Color");
    static readonly int ID_BaseColor = Shader.PropertyToID("_BaseColor");

    struct Entry
    {
        public Renderer renderer;
        public int propId;
        public Color original;
        public MaterialPropertyBlock block;
    }

    private readonly List<Entry> entries = new();

    void Awake()
    {
        // collect all renderers and cache their original colors + property blocks
        var rends = GetComponentsInChildren<Renderer>(includeInactive: true);
        foreach (var r in rends)
        {
            if (!r.enabled || r.sharedMaterial == null) continue;

            var mat = r.sharedMaterial;
            int prop = 0;
            if (mat.HasProperty(ID_BaseColor)) prop = ID_BaseColor;
            else if (mat.HasProperty(ID_Color)) prop = ID_Color;
            else continue; // no color property to flash

            Color orig = mat.GetColor(prop);

            var block = new MaterialPropertyBlock();
            r.GetPropertyBlock(block);

            entries.Add(new Entry
            {
                renderer = r,
                propId = prop,
                original = orig,
                block = block
            });
        }
    }

    public void Flash()
    {
        if (!isActiveAndEnabled || entries.Count == 0) return;
        StopAllCoroutines();
        StartCoroutine(CoFlash());
    }

    private System.Collections.IEnumerator CoFlash()
    {
        // set flash color
        for (int i = 0; i < entries.Count; i++)
        {
            var e = entries[i];
            e.block.SetColor(e.propId, flashColor);
            e.renderer.SetPropertyBlock(e.block);
        }

        // hold
        if (flashHold > 0f)
            yield return new WaitForSeconds(flashHold);

        // fade back
        if (fadeBackTime > 0f)
        {
            float t = 0f;
            while (t < fadeBackTime)
            {
                t += Time.deltaTime;
                float a = Mathf.Clamp01(t / fadeBackTime);

                for (int i = 0; i < entries.Count; i++)
                {
                    var e = entries[i];
                    Color c = Color.Lerp(flashColor, e.original, a);
                    e.block.SetColor(e.propId, c);
                    e.renderer.SetPropertyBlock(e.block);
                }

                yield return null;
            }
        }

        // snap back to exact original
        for (int i = 0; i < entries.Count; i++)
        {
            var e = entries[i];
            e.block.SetColor(e.propId, e.original);
            e.renderer.SetPropertyBlock(e.block);
        }
    }
}
