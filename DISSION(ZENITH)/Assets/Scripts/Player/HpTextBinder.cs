using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HpTextBinder : MonoBehaviour
{
    [Header("TMP")]
    [SerializeField] private TextMeshProUGUI tmpText; // TextMeshProUGUI

    private void OnEnable()
    {
        // 현재 HP로 즉시 갱신
        UpdateTextImmediate();

        if (GameStateManager.Instance != null)
            GameStateManager.Instance.OnPlayerHpChanged += HandleHpChanged;
    }

    private void OnDisable()
    {
        if (GameStateManager.Instance != null)
            GameStateManager.Instance.OnPlayerHpChanged -= HandleHpChanged;
    }

    private void HandleHpChanged(float current, float max)
    {
        SetText(current, max);
    }

    private void UpdateTextImmediate()
    {
        var gsm = GameStateManager.Instance;
        if (gsm != null)
            SetText(gsm.playerHP, gsm.playerMaxHP);
    }

    private void SetText(float current, float max)
    {
        string s = $"HP {Mathf.RoundToInt(current)}"; // 원하면 $"HP {cur}/{max}" 로

        if (tmpText != null) tmpText.text = s;
    }
}
