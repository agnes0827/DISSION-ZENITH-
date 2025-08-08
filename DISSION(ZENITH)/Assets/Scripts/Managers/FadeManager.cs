using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class FadeManager : MonoBehaviour
{
    public static FadeManager instance;

    public Image fadeImage; // ������ Panel�� Image ������Ʈ �Ҵ�
    public float fadeDuration = 1f; // ���̵� �ð�

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // �� ��ȯ�� �ı����� �ʵ���
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // �� ���� �� ���̵� �� ȿ�� ����
        StartCoroutine(FadeIn());
    }

    public void FadeToScene(int sceneIndex)
    {
        StartCoroutine(FadeOutAndLoad(sceneIndex));
    }

    private IEnumerator FadeOutAndLoad(int sceneIndex)
    {
        // ���̵� �ƿ�
        yield return StartCoroutine(Fade(1));

        // �� ��ȯ
        SceneManager.LoadScene(sceneIndex);

        // �� ��ȯ �� ���̵� ��
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
