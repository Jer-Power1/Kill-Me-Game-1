using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathScreenUI : MonoBehaviour
{
    [Header("Refs")]
    public CanvasGroup group;          // the whole death screen (panel) as a CanvasGroup
    public Button retryButton;         // optional, wire in Inspector
    public Button quitButton;          // optional, wire in Inspector

    [Header("Behavior")]
    public float fadeDuration = 0.6f;  // seconds
    public string mainMenuSceneName = ""; // optional: if set, Quit -> load this

    bool showing;

    void Awake()
    {
        if (!group) group = GetComponent<CanvasGroup>();
        HideInstant();
        // Wire buttons if assigned
        if (retryButton) retryButton.onClick.AddListener(Retry);
        if (quitButton) quitButton.onClick.AddListener(QuitToMenu);
    }

    public void Show()
    {
        if (showing) return;
        showing = true;
        Time.timeScale = 0f; // pause the game
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        StartCoroutine(Fade(0f, 1f, fadeDuration));
        group.interactable = true;
        group.blocksRaycasts = true;
    }

    public void HideInstant()
    {
        if (!group) return;
        group.alpha = 0f;
        group.interactable = false;
        group.blocksRaycasts = false;
    }

    System.Collections.IEnumerator Fade(float from, float to, float t)
    {
        float elapsed = 0f;
        group.alpha = from;
        while (elapsed < t)
        {
            elapsed += (Time.unscaledDeltaTime); // unaffected by pause
            group.alpha = Mathf.Lerp(from, to, Mathf.Clamp01(elapsed / t));
            yield return null;
        }
        group.alpha = to;
    }

    public void Retry()
    {
        Time.timeScale = 1f;

        // Hide & lock cursor for gameplay BEFORE reloading
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        var s = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        UnityEngine.SceneManagement.SceneManager.LoadScene(s.buildIndex);
    }


    public void QuitToMenu()
    {
        Time.timeScale = 1f;

        if (!string.IsNullOrEmpty(mainMenuSceneName))
        {
            SceneManager.LoadScene(mainMenuSceneName);
            return;
        }

#if UNITY_EDITOR
        // In the editor, stop play mode
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
