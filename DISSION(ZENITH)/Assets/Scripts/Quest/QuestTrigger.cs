using UnityEngine;

public class QuestTrigger : MonoBehaviour
{
    [Header("퀘스트 설정")]
    [Tooltip("CSV에 등록된 퀘스트 ID")]
    public string questId;

    [Header("동작 모드")]
    [Tooltip("체크 시, 게임 시작(Start)과 동시에 퀘스트를 수락합니다.")]
    public bool acceptOnStart = false;

    [Tooltip("체크 시, 플레이어가 콜라이더에 닿으면 퀘스트를 '수락'합니다.")]
    public bool acceptOnTrigger = false;

    [Tooltip("체크 시, 플레이어가 콜라이더에 닿으면 퀘스트를 '완료'합니다.")]
    public bool completeOnTrigger = false;

    private void Start()
    {
        // 1. 씬 시작 시 자동 수락
        if (acceptOnStart)
        {
            AcceptQuest();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // 2. 특정 위치(콜라이더) 도달 시 수락 [추가된 로직]
            if (acceptOnTrigger)
            {
                AcceptQuest();
            }

            // 3. 특정 위치(콜라이더) 도달 시 완료
            if (completeOnTrigger)
            {
                CompleteQuest();
            }
        }
    }

    private void AcceptQuest()
    {
        if (QuestManager.Instance != null)
        {
            // 이미 받았거나 완료한 퀘스트가 아닐 때만 수락
            if (!QuestManager.Instance.HasAccepted(questId) && !QuestManager.Instance.HasCompleted(questId))
            {
                QuestManager.Instance.AcceptQuest(questId);
                Debug.Log($"[QuestTrigger] 퀘스트 '{questId}' 수락됨 (조건: {(acceptOnStart ? "Start" : "Trigger")})");
            }
        }
    }
    private void CompleteQuest()
    {
        if (QuestManager.Instance != null)
        {
            // 이미 완료된 퀘스트가 아닐 때만 실행
            if (QuestManager.Instance.HasAccepted(questId) && !QuestManager.Instance.HasCompleted(questId))
            {
                // 목표 달성 처리
                QuestManager.Instance.SetObjectiveReached(questId);

                // 완료 처리
                QuestManager.Instance.CompleteQuest(questId);

                Debug.Log($"[QuestTrigger] 퀘스트 '{questId}' 위치 도달 완료!");
            }
        }
    }
}