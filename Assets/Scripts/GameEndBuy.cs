using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameEndBuy : MonoBehaviour
{
    public int cost = 10000;
    public TMP_Text promptText;
    bool playerInRange;

    void Start()
    {
        if (promptText)
            promptText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!playerInRange) return;

        if (Input.GetKeyDown(KeyCode.E))
            TryEndGame();
    }

    void TryEndGame()
    {
        if (!PointsManager.Instance) return;

        if (!PointsManager.Instance.SpendPoints(cost))
            return;

        GameEndManager.Instance.WinGame();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = true;
        if (promptText)
            promptText.gameObject.SetActive(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = false;
        if (promptText)
            promptText.gameObject.SetActive(false);
    }
}
