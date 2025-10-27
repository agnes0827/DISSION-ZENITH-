using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneTrigger : MonoBehaviour
{
    [Header("씬 이동 설정")]
    [SerializeField] string nextSceneName;
    [SerializeField] Image fadePanel;
    [SerializeField] float fadeDuration = 0.4f;

    [Header("조건 설정")]
    [SerializeField] string requiredItemId; // 이 문을 통과하기 위해 아이템이 필요한 경우
    [SerializeField] string lockedDialogueId; // 조건 미충족시 출력할 대사

    [Header("스폰포인트 설정")]
    [SerializeField] string targetSpawnPointId;

    [Header("SFX")]
    [SerializeField] AudioClip enterSfx;
    [SerializeField] float sfxVolume = 1f;

    private float interactionCooldown = 1f;
    private float lastInteractionTime = -1f; 

    private AudioSource _audio;
    private bool _isLoading;

    void Awake()
    {
        if (fadePanel == null)
            Debug.LogError("fadePanel이 비어있어요");
        if (string.IsNullOrWhiteSpace(nextSceneName))
            Debug.LogError("nextSceneName이 비어있어요");
        if (enterSfx != null)
        {
            _audio = GetComponent<AudioSource>();
            if (_audio == null) _audio = gameObject.AddComponent<AudioSource>();
            _audio.playOnAwake = false;
            _audio.spatialBlend = 0f;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (Time.time < lastInteractionTime + interactionCooldown)
        {
            return;
        }

        if (_isLoading || !other.CompareTag("Player")) return;

        // 아이템 조건 확인
        if (!string.IsNullOrEmpty(requiredItemId) && !InventoryManager.Instance.HasItem(requiredItemId))
        {
            lastInteractionTime = Time.time;
            DialogueManager.Instance.StartDialogue(lockedDialogueId);
            return;
        }

        // 씬 이동 시, 플레이어 이동 불가
        PlayerController.Instance.StopMovement();

        // 씬 이동 직전에, 다음 스폰포인트를 GameStateManager에 기록
        GameStateManager.Instance.nextSpawnPointId = targetSpawnPointId;

        // 씬 이동
        _isLoading = true;
        StartCoroutine(LoadWithFade());
    }

    IEnumerator LoadWithFade()
    {
        if (enterSfx && _audio) _audio.PlayOneShot(enterSfx, sfxVolume);
        yield return FadeTo(1f, fadeDuration);
        var op = SceneManager.LoadSceneAsync(nextSceneName);
        while (!op.isDone) yield return null;
        yield return FadeTo(0f, fadeDuration);
    }

    IEnumerator FadeTo(float targetAlpha, float duration)
    {
        if (fadePanel == null) yield break;
        fadePanel.raycastTarget = true;
        float start = fadePanel.color.a;
        float t = 0f;
        Color c = fadePanel.color;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(start, targetAlpha, t / duration);
            fadePanel.color = c;
            yield return null;
        }
        c.a = targetAlpha;
        fadePanel.color = c;
        if (Mathf.Approximately(targetAlpha, 0f))
            fadePanel.raycastTarget = false;
    }
}
