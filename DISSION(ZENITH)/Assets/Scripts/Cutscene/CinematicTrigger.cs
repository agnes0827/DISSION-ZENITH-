using UnityEngine;
using Cinemachine;
using System.Collections;

public class CinematicTrigger : MonoBehaviour
{
    [Header("기본 설정")]
    [Tooltip("이 이벤트의 고유 ID (중복 실행 방지용)")]
    public string eventId;
    [Tooltip("체크 시, 씬이 시작되자마자 자동으로 연출 시작")]
    public bool playOnStart = false;

    [Header("레터박스 설정")]
    public bool showLetterbox = true;
    public GameObject cinematicBars;

    [Header("카메라 연출")]
    [Tooltip("연출 시작 전, 현재 화면(플레이어)을 비추며 대기할 시간")]
    public float startDelay = 1.0f;

    [Tooltip("연출 시 활성화할 타겟 카메라")]
    public CinemachineVirtualCamera targetCam;
    [Tooltip("기본 플레이어 카메라")]
    public CinemachineVirtualCamera playerCam;
    [Tooltip("카메라가 타겟으로 이동한 뒤 대기할 시간")]
    public float focusDuration = 2.0f;

    [Header("대사 및 조건")]
    [Tooltip("출력할 대사 ID (비워두면 대사 출력 X)")]
    public string dialogueId;
    [Tooltip("몬스터 고유 ID, 특정 몬스터 처치 시 실행 안 함 (보스 전용, 비워두면 무시)")]
    public string disableIfMonsterDefeated;

    private bool isRunning = false;

    private void Start()
    {
        // [추가됨] 시작하자마자 실행 (보스전 인트로용)
        if (playOnStart)
        {
            // 약간의 딜레이를 주어 다른 스크립트 초기화 후 실행되게 함 (안전장치)
            StartCoroutine(CheckAndPlaySequence());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어가 밟아서 실행 (맵 탐험용)
        // playOnStart가 꺼져있을 때만 작동
        if (!playOnStart && collision.CompareTag("Player"))
        {
            StartCoroutine(CheckAndPlaySequence());
        }
    }

    IEnumerator CheckAndPlaySequence()
    {
        if (isRunning) yield break;

        // 1. 이미 본 이벤트인지 체크
        if (GameStateManager.Instance.HasExecutedEvent(eventId)) yield break;

        // 2. 보스전 조건 체크 (이미 잡았으면 스킵)
        if (!string.IsNullOrEmpty(disableIfMonsterDefeated) &&
            GameStateManager.Instance.IsMonsterDefeated(disableIfMonsterDefeated)) yield break;

        yield return StartCoroutine(PlaySequence());
    }

    IEnumerator PlaySequence()
    {
        isRunning = true;
        GameStateManager.Instance.SetEventExecuted(eventId);

        // 플레이어 정지
        if (PlayerController.Instance != null) PlayerController.Instance.StopMovement();

        // [옵션] 레터박스 ON
        if (showLetterbox && cinematicBars != null)
        {
            cinematicBars.SetActive(true);
        }

        // 카메라 이동 전, 잠시 대기
        if (startDelay > 0)
        {
            yield return new WaitForSeconds(startDelay);
        }

        // 카메라 전환
        int originalPriority = 0;
        if (targetCam != null && playerCam != null)
        {
            originalPriority = targetCam.Priority;
            targetCam.Priority = playerCam.Priority + 1; // 타겟 카메라 활성화

            yield return new WaitForSeconds(focusDuration);
        }

        // 카메라 복귀
        if (targetCam != null)
        {
            targetCam.Priority = originalPriority; // 원래대로 복구
            yield return new WaitForSeconds(1.5f); // 돌아오는 시간 대기
        }

        // 레터박스 OFF
        if (showLetterbox && cinematicBars != null)
        {
            cinematicBars.SetActive(false);
        }

        // [옵션] 대사 출력
        if (!string.IsNullOrEmpty(dialogueId))
        {
            DialogueManager.Instance.StartDialogue(dialogueId);
        }

        // 플레이어 재개
        if (PlayerController.Instance != null) PlayerController.Instance.ResumeMovement();
    }
}