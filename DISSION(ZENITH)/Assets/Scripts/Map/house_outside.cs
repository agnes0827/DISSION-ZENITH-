using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class house_outside : MonoBehaviour
{
    [SerializeField] string nextSceneName;
    [SerializeField] Image fadePanel; // 전체화면 검은 Panel이 있는 CanvasGroup
    [SerializeField] float fadeDuration = 0.4f;

    [Header("SFX")]
    [SerializeField] AudioClip enterSfx;          // 트리거 진입 효과음
    [SerializeField] float sfxVolume = 1f;
    [SerializeField] bool waitForSfxToFinish = true; // 끝까지 듣고 넘어갈지

    AudioSource _audio;  // 내부에서 자동 준비
    bool _loading;

    void Awake()
    {
        if (fadePanel == null)
            Debug.LogError("[house_outside] fadePanel이 비어있어요 (Image 참조 필요)");

        if (string.IsNullOrWhiteSpace(nextSceneName))
            Debug.LogError("[house_outside] nextSceneName이 비어있어요");

        if (enterSfx != null)
        {
            _audio = GetComponent<AudioSource>();
            if (_audio == null) _audio = gameObject.AddComponent<AudioSource>();
            _audio.playOnAwake = false;
            _audio.spatialBlend = 0f; // 2D 사운드
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_loading) return;
        if (!other.CompareTag("Player")) return;

        _loading = true;
        StartCoroutine(LoadWithFade());
    }

    IEnumerator LoadWithFade()
    {
        // 효과음 재생
        if (enterSfx && _audio) _audio.PlayOneShot(enterSfx, sfxVolume);

        // 페이드 아웃
        yield return FadeTo(1f, fadeDuration);

        // 씬 로드 (비동기 권장)
        var op = SceneManager.LoadSceneAsync(nextSceneName);
        while (!op.isDone) yield return null;

        // 페이드 인
        yield return FadeTo(0f, fadeDuration);
    }

    IEnumerator FadeTo(float targetAlpha, float duration)
    {
        if (fadePanel == null) yield break;

        // 입력 막기
        fadePanel.raycastTarget = true;

        float start = fadePanel.color.a;
        float t = 0f;
        Color c = fadePanel.color;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime; // 타임스케일 무시(일시정지 중에도 페이드)
            c.a = Mathf.Lerp(start, targetAlpha, t / duration);
            fadePanel.color = c;
            yield return null;
        }
        c.a = targetAlpha;
        fadePanel.color = c;

        // 투명해졌으면 다시 입력 허용
        if (Mathf.Approximately(targetAlpha, 0f))
            fadePanel.raycastTarget = false;
    }
}
