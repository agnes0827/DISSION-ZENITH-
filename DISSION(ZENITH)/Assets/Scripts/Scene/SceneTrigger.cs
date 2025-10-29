using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneTrigger : MonoBehaviour
{
    [Header("씬 이동 설정")]
    [SerializeField] string nextSceneName;

    [Header("조건 설정")]
    [SerializeField] string requiredItemId;     // 이 문을 통과하기 위해 아이템이 필요한 경우
    [SerializeField] string lockedDialogueId;   // 조건 미충족시 출력할 대사

    [Header("스폰포인트 설정")]
    [SerializeField] string targetSpawnPointId;

    [Header("SFX")]
    [SerializeField] AudioClip enterSfx;
    [SerializeField] float sfxVolume = 1f;

    private float interactionCooldown = 1f;
    private float lastInteractionTime = -1f; 

    private AudioSource _audio;

    void Awake()
    {
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
        if (Time.time < lastInteractionTime + interactionCooldown) return;
        if (!other.CompareTag("Player")) return;

        if (SceneLoader.Instance != null && SceneLoader.Instance.isLoading)
        {
            Debug.Log("SceneTrigger: SceneLoader가 이미 로딩 중이므로 무시합니다.");
            return;
        }
        // 아이템 조건 확인
        if (!string.IsNullOrEmpty(requiredItemId) && !InventoryManager.Instance.HasItem(requiredItemId))
        {
            lastInteractionTime = Time.time;
            DialogueManager.Instance.StartDialogue(lockedDialogueId);
            return;
        }

        PlayerController.Instance.StopMovement();

        // 씬 이동 직전에, 다음 스폰포인트를 GameStateManager에 기록
        GameStateManager.Instance.nextSpawnPointId = targetSpawnPointId;

        // 효과음 재생
        if (enterSfx && _audio) _audio.PlayOneShot(enterSfx, sfxVolume);

        // SceneLoader에게 씬 로드 요청
        SceneLoader.Instance.LoadSceneWithFade(nextSceneName, 0.8f);
    }
}
