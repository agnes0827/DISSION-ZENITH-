using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Tooltip("이 퀘스트를 시작하기 위한 선행 퀘스트 ID")]
    public string prerequisiteQuestId;

    [Tooltip("선행 퀘스트가 미완료일 때 보여줄 대화 ID")]
    public string lockedId;

    [Tooltip("기본 대화 ID")]
    public string defaultId;

    [Header("퀘스트 대화 설정")]
    public string acceptedId;
    public string objectiveId;
    public string completedId;

    public string QuestId;
    public string npcId;
    public bool isReportNpc = false;

    [Header("트리거 옵션")]
    [Tooltip("플레이어가 닿으면 자동으로 대화가 시작됩니다.")]
    public bool triggerOnEnter = false;

    [Tooltip("게임 전체에서 딱 한 번만 실행됩니다.")]
    public bool isOneTimeOnly = false;

    [Tooltip("1회성 이벤트 식별 ID (isOneTimeOnly가 true일 때만 필수)")]
    public string oneTimeEventId;



    public void TriggerDialogue()
    {
        // 1. 1회성 이벤트 체크
        if (isOneTimeOnly)
        {
            // ID가 비어있으면 경고
            if (string.IsNullOrEmpty(oneTimeEventId))
            {
                Debug.LogWarning($"[DialogueTrigger] {gameObject.name} : 1회성 이벤트 ID가 없습니다!");
                return;
            }

            // 이미 실행된 이벤트라면 대화 시작 안 함
            if (GameStateManager.Instance.HasExecutedEvent(oneTimeEventId))
            {
                // 필요하다면 여기서 collider를 끄거나 오브젝트를 비활성화 할 수도 있음
                return;
            }

            // 실행 처리 등록
            GameStateManager.Instance.SetEventExecuted(oneTimeEventId);
        }

        string selectedId = defaultId;

        // 1. 선행 퀘스트 미완료 → locked 대사 출력
        if (!string.IsNullOrEmpty(prerequisiteQuestId) &&
            !QuestManager.Instance.HasCompleted(prerequisiteQuestId))
        {
            selectedId = lockedId;
        }
        else if (!string.IsNullOrEmpty(QuestId))
        {
            if (QuestManager.Instance.HasCompleted(QuestId))
            {
                selectedId = completedId;
            }
            else if (QuestManager.Instance.HasAccepted(QuestId))
            {
                if (QuestManager.Instance.IsObjectiveReached(QuestId))
                {
                    selectedId = objectiveId;

                    // 완료 NPC라면 바로 퀘스트 완료
                    if (isReportNpc)
                    {
                        QuestManager.Instance.CompleteQuest(QuestId);
                        Debug.Log($"[퀘스트 완료] Quest={QuestId}");
                    }
                }
                else
                {
                    selectedId = acceptedId;
                }

                // 목표 대상 NPC인지 확인 → 목표 달성 처리
                if (!string.IsNullOrEmpty(npcId))
                {
                    Quest quest = QuestManager.Instance.GetQuestById(QuestId);
                    if (quest != null && quest.target_id == npcId)
                    {
                        QuestManager.Instance.SetObjectiveReached(QuestId);
                        Debug.Log($"[목표 달성 처리] NPC={npcId}, Quest={QuestId}");
                    }
                }
            }
        }

        // 2. 대사 출력
        DialogueManager.Instance.StartDialogue(selectedId, this.gameObject);
    }
}
