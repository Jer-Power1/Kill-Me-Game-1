using UnityEngine;
using TMPro;
using System.Collections;

public class RoundUI : MonoBehaviour
{
    public TMP_Text roundText;
    public float fadeDuration = 0.5f;

    void Awake()
    {
        if (!roundText)
            roundText = GetComponent<TMP_Text>();
    }

    // Used at game start (no fade)
    public void SetInstant(int round)
    {
        roundText.text = round.ToString();
        SetAlpha(1f);
    }

    // Used between rounds (fade out -> change -> fade in)
    public IEnumerator FadeToRound(int newRound)
    {
        // Fade out
        yield return StartCoroutine(Fade(1f, 0f));

        // Change number while invisible
        roundText.text = newRound.ToString();

        // Small pause (optional)
        yield return new WaitForSeconds(0.15f);

        // Fade back in
        yield return StartCoroutine(Fade(0f, 1f));
    }

    IEnumerator Fade(float from, float to)
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            SetAlpha(Mathf.Lerp(from, to, t / fadeDuration));
            yield return null;
        }
        SetAlpha(to);
    }

    void SetAlpha(float a)
    {
        Color c = roundText.color;
        c.a = a;
        roundText.color = c;
    }
}
