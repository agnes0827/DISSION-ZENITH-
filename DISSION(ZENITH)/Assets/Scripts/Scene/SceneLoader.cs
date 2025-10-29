using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }
    public bool isLoading = false; // 중복 로딩 방지

    void Awake()
    {
        // 싱글톤 패턴
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 파괴되지 않도록 설정
            Debug.Log("SceneLoader 싱글톤 생성 및 DontDestroyOnLoad 설정 완료.");
        }
        else
        {
            Debug.LogWarning("SceneLoader 중복 인스턴스 발견. 파괴합니다.");
            Destroy(gameObject);
        }
    }

    public void LoadSceneWithFade(string sceneName, float fadeDuration = 0.4f)
    {
        // 이미 로딩 중이면 중복 실행 방지
        if (isLoading) return;
        StartCoroutine(LoadSceneFadeCoroutine(sceneName, fadeDuration));
    }

    // 페이드 효과와 함께 씬을 로드하는 코루틴
    IEnumerator LoadSceneFadeCoroutine(string sceneName, float duration)
    {
        isLoading = true; // 로딩 시작

        // 1. 페이드 아웃 시작 및 완료 대기
        if (FadeManager.Instance != null)
        {
            yield return FadeManager.Instance.FadeOut(duration);
        }
        else { Debug.LogWarning("FadeManager Instance 없음!"); }

        // 2. 씬 로드
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        // 씬 로드가 완료될 때까지 대기
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        float delayAfterLoad = 0.5f;
        yield return new WaitForSecondsRealtime(delayAfterLoad);

        // 3. 페이드 인 시작 및 완료 대기
        if (FadeManager.Instance != null)
        {
            // FadeManager의 FadeIn 코루틴을 기다림
            yield return FadeManager.Instance.FadeIn(duration);
        }
        isLoading = false; // 로딩 완료
    }

    // 페이드 없이 바로 씬 로드하는 함수
    public void LoadSceneImmediately(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}