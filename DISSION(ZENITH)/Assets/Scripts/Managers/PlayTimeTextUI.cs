using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayTimeTextUI : MonoBehaviour
{
    [Header("플레이 타임을 표시할 TMP_Text 컴포넌트")]
    public TMP_Text playTimeText;

    public void ShowPlayTime()
    {
        if (GameStateManager.Instance != null && playTimeText != null)
        {
            playTimeText.text = GameStateManager.Instance.GetFormattedPlayTime();
        }
        else
        {
            Debug.LogWarning("GameStateManager 또는 playTimeText가 연결되지 않았습니다.");
        }
    }
}
