using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeTrigger : MonoBehaviour
{
    [Header("과일 타이밍 미니게임")]
    public FruitTimingMiniGame miniGame;    // Canvas 내에 있는 미니게임 오브젝트

    [Header("퀘스트 설정")]
    public string questId = "Q03";          // 열매 3개 모으기 퀘스트 ID
    public int requiredCount = 3;           // 필요한 열매 개수 (3개)

    // 모든 나무 객체 공유
    private static int collectedCount = 0;

    private bool used = false;              // 이미 열매를 얻었는지 여부
    private bool isPlayerInZone = false;    // 플레이어가 범위 안에 있는지

    private void Update()
    {
        // 플레이어가 범위 안에 있고, F키를 눌렀고, 아직 사용되지 않았다면 미니게임 시작
        if (isPlayerInZone && Input.GetKeyDown(KeyCode.F) && !used)
        {
            // 이미 게임이 활성화된 경우 중복 실행 방지
            if (miniGame.gameObject.activeSelf) return;

            // 미니게임 성공 시 호출할 콜백 지정
            miniGame.onSuccess = OnMiniGameSuccess;

            // 미니게임 활성화 및 시작
            miniGame.gameObject.SetActive(true);
            miniGame.StartMiniGame();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInZone = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInZone = false;
        }
    }

    private void OnMiniGameSuccess()
    {
        used = true;  // 이 나무는 더 이상 사용 불가

        // 열매 획득 +1
        collectedCount++;
        Debug.Log($"[FruitTree] {questId} 열매 획득! 현재 개수: {collectedCount}/{requiredCount}");

        // 퀘스트를 수락한 상태이고, 아직 완료하지 않았으며, 3개를 다 모았다면 완료 처리
        if (QuestManager.Instance != null &&
            QuestManager.Instance.HasAccepted(questId) &&
            !QuestManager.Instance.HasCompleted(questId) &&
            collectedCount >= requiredCount)
        {
            // 목표 달성 처리
            QuestManager.Instance.SetObjectiveReached(questId);
            QuestManager.Instance.CompleteQuest(questId);

            Debug.Log($"[FruitTree] 퀘스트 '{questId}' 완료! {requiredCount}개 모으기 성공!");
        }
    }
}