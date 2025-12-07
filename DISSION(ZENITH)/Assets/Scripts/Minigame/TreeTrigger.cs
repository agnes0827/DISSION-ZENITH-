using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeTrigger : MonoBehaviour
{
    [Header("이 나무에서 사용할 미니게임")]
    public FruitTimingMiniGame miniGame;   // Canvas 밑에 있는 미니게임 오브젝트

    [Header("퀘스트 연동")]
    public string questId = "Q03";  // 열매 3개 모으는 퀘스트 ID
    public int requiredCount = 3;   // 필요한 열매 개수 (3개)

    // ★ 전체 열매 개수 (모든 나무가 공유)
    private static int collectedCount = 0;

    private bool used = false;  // 이미 열매를 땄는지 여부

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (used) return;
        if (!collision.CompareTag("Player")) return;

        // 이미 켜져 있으면 중복 실행 막기
        if (miniGame.gameObject.activeSelf) return;

        // 미니게임 성공 시 호출될 콜백 등록
        miniGame.onSuccess = OnMiniGameSuccess;

        // 미니게임 켜고 시작
        miniGame.gameObject.SetActive(true);
        miniGame.StartMiniGame();
    }

    private void OnMiniGameSuccess()
    {
        used = true;  // 이 나무는 더 이상 사용 X

        // 열매 개수 +1
        collectedCount++;
        Debug.Log($"[FruitTree] {questId} 열매 획득! 현재 개수: {collectedCount}/{requiredCount}");

        // 퀘스트가 수락되어 있고, 아직 완료 안 됐고, 3개 다 모았으면 완료 처리
        if (QuestManager.Instance != null &&
            QuestManager.Instance.HasAccepted(questId) &&
            !QuestManager.Instance.HasCompleted(questId) &&
            collectedCount >= requiredCount)
        {
            // 목표 달성 처리 (기존 QuestTrigger에서 쓰던 패턴 따라감)
            QuestManager.Instance.SetObjectiveReached(questId);
            QuestManager.Instance.CompleteQuest(questId);

            Debug.Log($"[FruitTree] 퀘스트 '{questId}' 열매 {requiredCount}개 모아서 완료!");
        }
    }
}
