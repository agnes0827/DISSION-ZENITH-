using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance { get; private set; }

    [SerializeField] private Image blackFadePanel; // 검은색 전체 화면 이미지

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            FadePanel();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (blackFadePanel == null) return;

        // 1. 기본값 설정 (기존 FadeManager의 하드코딩 값과 동일)
        float finalInDuration = 0.95f; // 들어올 때 시간
        float finalDelay = 0.15f;      // 대기 시간
        Color finalColor = Color.black;
        bool shouldFade = true;

        // 2. 현재 씬에 FadeSettings가 있는지 확인
        FadeSettings settings = FindObjectOfType<FadeSettings>();

        if (settings != null)
        {
            shouldFade = settings.enableFade;
            if (shouldFade)
            {
                finalColor = settings.fadeColor;
                finalInDuration = settings.fadeInDuration; // 설정된 In 시간 적용
                finalDelay = settings.startDelay;
            }
        }

        // 3. 페이드 안 함 설정이면 패널 끄고 종료
        if (!shouldFade)
        {
            blackFadePanel.gameObject.SetActive(false);
            return;
        }

        // 4. 페이드 인 실행
        // (이전 씬에서 WipeOut을 했든 FadeOut을 했든, 새 씬은 설정된 색상으로 꽉 채워서 시작)
        finalColor.a = 1f;
        blackFadePanel.color = finalColor;
        blackFadePanel.fillAmount = 1f; // 혹시 Wipe로 닫혔을 경우를 대비해 꽉 채움
        blackFadePanel.gameObject.SetActive(true);

        // 설정된 시간으로 부드럽게 열림
        FadeIn(finalInDuration, finalDelay);
    }

    private void FadePanel()
    {
        if (blackFadePanel != null)
        {
            blackFadePanel.color = new Color(0, 0, 0, 0);        // 완전 투명
            blackFadePanel.raycastTarget = false;                // 클릭 통과
            blackFadePanel.gameObject.SetActive(true);           // 항상 켜둠 (알파값으로 제어)
        }
    }

    // 전투 진입용: 화면을 검은색으로 덮음 (0 -> 1)
    public Coroutine WipeOut(float duration = 1f)
    {
        return StartCoroutine(WipeCoroutine(1f, duration));
    }

    // 전투 종료용: 화면을 다시 걷어냄 (1 -> 0)
    public Coroutine WipeIn(float duration = 1f)
    {
        return StartCoroutine(WipeCoroutine(0f, duration));
    }

    private IEnumerator WipeCoroutine(float targetFill, float duration)
    {
        if (blackFadePanel == null) yield break;

        blackFadePanel.raycastTarget = true; // 클릭 방지
        blackFadePanel.color = Color.black;

        if (targetFill > 0.5f)
        {
            blackFadePanel.fillAmount = 0f;
        }

        else
        {
            blackFadePanel.fillAmount = 1f;
        }

        float startFill = blackFadePanel.fillAmount;
        float time = 0f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            blackFadePanel.fillAmount = Mathf.Lerp(startFill, targetFill, time / duration);
            yield return null;
        }

        blackFadePanel.fillAmount = targetFill;

        // 클릭 허용
        if (Mathf.Approximately(targetFill, 0f))
        {
            blackFadePanel.raycastTarget = false;
        }
    }

    public void SetFadeColor(Color color)
    {
        if (blackFadePanel != null)
        {
            float currentAlpha = blackFadePanel.color.a;
            color.a = currentAlpha;
            blackFadePanel.color = color;
        }
    }

    public Coroutine FadeOut(float duration = 0.4f)
    {
        return StartCoroutine(FadeCoroutine(1f, duration)); // 알파 1

    }

    public Coroutine FadeIn(float duration = 0.4f, float delay = 0f)
    {
        return StartCoroutine(FadeInSequence(duration, delay));
    }

    private IEnumerator FadeInSequence(float duration, float delay)
    {
        if (delay > 0f)
        {
            if (blackFadePanel != null)
            {
                blackFadePanel.fillAmount = 1f;
                Color c = blackFadePanel.color;
                c.a = 1f;
                blackFadePanel.color = c;
                blackFadePanel.raycastTarget = true;
            }

            yield return new WaitForSeconds(delay);
        }
        yield return StartCoroutine(FadeCoroutine(0f, duration));
    }

    // 실제 페이드 로직 (Time.unscaledDeltaTime 사용)
    private IEnumerator FadeCoroutine(float targetAlpha, float duration)
    {
        if (blackFadePanel == null) yield break; // 패널 없으면 중단

        blackFadePanel.fillAmount = 1f;

        Color startColor = blackFadePanel.color;
        float startAlpha = startColor.a;

        if (duration <= 0)
        {
            Color c = startColor;
            c.a = targetAlpha;
            blackFadePanel.color = c;
            blackFadePanel.raycastTarget = !Mathf.Approximately(targetAlpha, 0f);
            yield break;
        }

        blackFadePanel.raycastTarget = true; // 페이드 중에는 UI 클릭 방지
        float time = 0f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);

            Color newColor = startColor;
            newColor.a = newAlpha;
            blackFadePanel.color = newColor;

            yield return null;
        }

        Color finalColor = startColor;
        finalColor.a = targetAlpha;
        blackFadePanel.color = finalColor;

        // 페이드 인 완료 시 클릭 통과 허용
        if (Mathf.Approximately(targetAlpha, 0f))
        {
            blackFadePanel.raycastTarget = false;
        }
    }
}
