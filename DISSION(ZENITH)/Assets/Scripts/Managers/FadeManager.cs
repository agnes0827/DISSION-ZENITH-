using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class FadeManager : MonoBehaviour
{
    public static FadeManager instance;

    public Image fadeImage; // 검은색 Panel의 Image 컴포넌트 할당
    public float fadeDuration = 1f; // 페이드 시간

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환시 파괴되지 않도록
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 씬 시작 시 페이드 인 효과 실행
        StartCoroutine(FadeIn());
    }

    public void FadeToScene(int sceneIndex)
    {
        StartCoroutine(FadeOutAndLoad(sceneIndex));
    }

    private IEnumerator FadeOutAndLoad(int sceneIndex)
    {
        // 페이드 아웃
        yield return StartCoroutine(Fade(1));

        // 씬 전환
        SceneManager.LoadScene(sceneIndex);

        // 씬 전환 후 페이드 인
        yield return StartCoroutine(Fade(0));
    }

    private IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = fadeImage.color.a;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        fadeImage.color = new Color(0, 0, 0, targetAlpha);
    }

    private IEnumerator FadeIn()
    {
        yield return Fade(0);
    }
}
