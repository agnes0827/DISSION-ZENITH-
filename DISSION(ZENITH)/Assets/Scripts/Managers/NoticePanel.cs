using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoticePanel : MonoBehaviour
{
    public RectTransform panel;     // 움직일 패널
    public float moveDistance = 250f; // 내려올 거리
    public float moveDuration = 1f;  // 내려오고 올라가는 데 걸리는 시간
    public float waitTime = 8f;      // 내려온 상태로 기다리는 시간

    private Vector2 originalPos;

    void Start()
    {
        originalPos = panel.anchoredPosition;
        StartCoroutine(PanelFlow());
    }

    IEnumerator PanelFlow()
    {
        // 1. 위에서 아래로 내려오기
        yield return StartCoroutine(MovePanel(originalPos, originalPos - new Vector2(0, moveDistance), moveDuration));

        // 2. 8초 대기
        yield return new WaitForSeconds(waitTime);

        // 3. 아래에서 다시 위로 올라가기
        yield return StartCoroutine(MovePanel(panel.anchoredPosition, originalPos, moveDuration));
    }

    IEnumerator MovePanel(Vector2 from, Vector2 to, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            panel.anchoredPosition = Vector2.Lerp(from, to, elapsed / duration);
            yield return null;
        }

        panel.anchoredPosition = to; // 마지막 위치 보정
    }
}
