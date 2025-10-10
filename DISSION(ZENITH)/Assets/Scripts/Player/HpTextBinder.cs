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
        // ���� HP�� ��� ����
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
        string s = $"HP {Mathf.RoundToInt(current)}"; // ���ϸ� $"HP {cur}/{max}" ��

        if (tmpText != null) tmpText.text = s;
    }
}
