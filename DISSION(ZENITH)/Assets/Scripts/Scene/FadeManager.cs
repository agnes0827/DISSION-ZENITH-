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
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬이 로드되면 검은 화면(Alpha 1)에서 투명하게(Alpha 0) 만듦
        // duration은 필요에 따라 조절 (예: 0.5f)
        if (blackFadePanel != null)
        {
            blackFadePanel.color = Color.black;

            Color c = blackFadePanel.color;
            c.a = 1f;
            blackFadePanel.color = c;

            blackFadePanel.fillAmount = 1f;
            blackFadePanel.gameObject.SetActive(true);
        }
        FadeIn(0.95f, 0.15f);
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
