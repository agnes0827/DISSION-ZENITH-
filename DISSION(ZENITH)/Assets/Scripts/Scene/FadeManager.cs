using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance { get; private set; }

    [SerializeField] private Image blackFadePanel; // 검은색 전체 화면 이미지

    void Awake()
    {
        // 싱글톤 패턴
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("FadeManager 싱글톤 생성");

            // 시작 시 투명하게 초기화
            FadePanel();
        }
        else
        {
            Debug.LogWarning("FadeManager 중복 인스턴스 발견. 파괴합니다.");
            Destroy(gameObject); // 중복이면 파괴
        }
    }

    private void FadePanel()
    {
        if (blackFadePanel != null)
        {
            blackFadePanel.color = new Color(0, 0, 0, 0);        // 완전 투명
            blackFadePanel.raycastTarget = false;                // 클릭 통과
            blackFadePanel.gameObject.SetActive(true);           // 항상 켜둠 (알파값으로 제어)
        }
        else
        {
            Debug.LogError("FadeManager: Fade Panel이 연결되지 않았습니다!");
        }
    }

    public Coroutine FadeOut(float duration = 0.4f)
    {
        return StartCoroutine(FadeCoroutine(1f, duration)); // 알파 1

    }

    public Coroutine FadeIn(float duration = 0.4f)
    {
        return StartCoroutine(FadeCoroutine(0f, duration)); // 알파 0
    }

    // 실제 페이드 로직 (Time.unscaledDeltaTime 사용)
    private IEnumerator FadeCoroutine(float targetAlpha, float duration)
    {
        if (blackFadePanel == null) yield break; // 패널 없으면 중단

        if (duration <= 0)
        {
            blackFadePanel.color = new Color(0, 0, 0, targetAlpha);
            blackFadePanel.raycastTarget = !Mathf.Approximately(targetAlpha, 0f);
            yield break;
        }

        blackFadePanel.raycastTarget = true; // 페이드 중에는 UI 클릭 방지
        float startAlpha = blackFadePanel.color.a;
        float time = 0f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            blackFadePanel.color = new Color(0, 0, 0, newAlpha);
            yield return null; // 다음 프레임까지 대기
        }

        // 정확한 목표 알파값으로 설정
        blackFadePanel.color = new Color(0, 0, 0, targetAlpha);

        // 페이드 인 완료 시 클릭 통과 허용
        if (Mathf.Approximately(targetAlpha, 0f))
        {
            blackFadePanel.raycastTarget = false;
        }
    }
}
