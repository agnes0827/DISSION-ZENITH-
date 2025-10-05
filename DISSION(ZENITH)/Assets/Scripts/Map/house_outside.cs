using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class house_outside : MonoBehaviour
{
    [SerializeField] string nextSceneName;
    [SerializeField] Image fadePanel; // ��üȭ�� ���� Panel�� �ִ� CanvasGroup
    [SerializeField] float fadeDuration = 0.4f;

    [Header("SFX")]
    [SerializeField] AudioClip enterSfx;          // Ʈ���� ���� ȿ����
    [SerializeField] float sfxVolume = 1f;
    [SerializeField] bool waitForSfxToFinish = true; // ������ ��� �Ѿ��

    AudioSource _audio;  // ���ο��� �ڵ� �غ�
    bool _loading;

    void Awake()
    {
        if (fadePanel == null)
            Debug.LogError("[house_outside] fadePanel�� ����־�� (Image ���� �ʿ�)");

        if (string.IsNullOrWhiteSpace(nextSceneName))
            Debug.LogError("[house_outside] nextSceneName�� ����־��");

        if (enterSfx != null)
        {
            _audio = GetComponent<AudioSource>();
            if (_audio == null) _audio = gameObject.AddComponent<AudioSource>();
            _audio.playOnAwake = false;
            _audio.spatialBlend = 0f; // 2D ����
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
        // ȿ���� ���
        if (enterSfx && _audio) _audio.PlayOneShot(enterSfx, sfxVolume);

        // ���̵� �ƿ�
        yield return FadeTo(1f, fadeDuration);

        // �� �ε� (�񵿱� ����)
        var op = SceneManager.LoadSceneAsync(nextSceneName);
        while (!op.isDone) yield return null;

        // ���̵� ��
        yield return FadeTo(0f, fadeDuration);
    }

    IEnumerator FadeTo(float targetAlpha, float duration)
    {
        if (fadePanel == null) yield break;

        // �Է� ����
        fadePanel.raycastTarget = true;

        float start = fadePanel.color.a;
        float t = 0f;
        Color c = fadePanel.color;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime; // Ÿ�ӽ����� ����(�Ͻ����� �߿��� ���̵�)
            c.a = Mathf.Lerp(start, targetAlpha, t / duration);
            fadePanel.color = c;
            yield return null;
        }
        c.a = targetAlpha;
        fadePanel.color = c;

        // ������������ �ٽ� �Է� ���
        if (Mathf.Approximately(targetAlpha, 0f))
            fadePanel.raycastTarget = false;
    }
}
