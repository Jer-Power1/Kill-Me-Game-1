using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;


[RequireComponent(typeof(CanvasGroup))]
public class DeathScreenUI : MonoBehaviour
{
    [Header("Refs (auto-filled)")]
    public CanvasGroup group;
    public Button retryButton;
    public Button quitButton;

    [Header("Behavior")]
    public float fadeDuration = 0.6f;
    public string mainMenuSceneName = "";

    bool showing;

    void Awake()
    {
        if (!group) group = GetComponent<CanvasGroup>();
        group.alpha = 0f;
        group.interactable = false;
        group.blocksRaycasts = false;

        // Try to auto-find buttons if not assigned
        if (!retryButton || !quitButton)
        {
            foreach (var b in GetComponentsInChildren<Button>(true))
            {
                var n = b.name.ToLower();
                if (!retryButton && n.Contains("retry")) retryButton = b;
                if (!quitButton && (n.Contains("quit") || n.Contains("exit"))) quitButton = b;
            }
        }

        // As a fallback, if still missing retry, grab the first Button found
        if (!retryButton) retryButton = GetComponentInChildren<Button>(true);

        // Wire listeners (clear first to avoid duplicates)
        if (retryButton)
        {
            retryButton.onClick.RemoveAllListeners();
            retryButton.onClick.AddListener(Retry);
        }
        if (quitButton)
        {
            quitButton.onClick.RemoveAllListeners();
            quitButton.onClick.AddListener(QuitToMenu);
        }
    }

    public void Show()
    {
        if (showing) return;
        showing = true;

        group.interactable = true;
        group.blocksRaycasts = true;

        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        StopAllCoroutines();
        StartCoroutine(Fade(0f, 1f, fadeDuration));

        // Focus the retry button so keyboard works immediately
        if (retryButton) EventSystem.current?.SetSelectedGameObject(retryButton.gameObject);
    }

    void Update()
    {
        if (!showing) return;

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            Retry();

        if (Input.GetKeyDown(KeyCode.Escape))
            QuitToMenu();
    }


    System.Collections.IEnumerator Fade(float from, float to, float dur)
    {
        float t = 0f;
        group.alpha = from;
        while (t < dur)
        {
            t += Time.unscaledDeltaTime;
            group.alpha = Mathf.Lerp(from, to, t / dur);
            yield return null;
        }
        group.alpha = to;
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        var s = SceneManager.GetActiveScene();
        SceneManager.LoadScene(s.buildIndex);
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
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
