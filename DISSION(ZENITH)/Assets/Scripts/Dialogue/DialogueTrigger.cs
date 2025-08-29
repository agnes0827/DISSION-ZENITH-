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
    [Tooltip("퀘스트 진행 중 출력할 대화 ID (목표 달성 전)")]
    public string acceptedId;

    [Tooltip("퀘스트 목표 달성 후 출력할 대화 ID")]
    public string objectiveId;

    [Tooltip("퀘스트 완료 후 출력할 대화 ID")]
    public string completedId;

    [Tooltip("퀘스트 ID")]
    public string QuestId;

    [Tooltip("이 NPC의 고유 ID (ex: Npc_choco)")]
    public string npcId;

    [Tooltip("퀘스트 보고 NPC 여부")]
    public bool isReportNpc = false;

    public void TriggerDialogue()
    {
        string selectedId = defaultId;

        // 1. 선행 퀘스트 미완료 시 → locked 대사 출력
        if (!string.IsNullOrEmpty(prerequisiteQuestId) &&
            !QuestManager.Instance.HasCompleted(prerequisiteQuestId))
        {
            selectedId = lockedId;
        }
        else if (!string.IsNullOrEmpty(QuestId))
        {
            // 2. 퀘스트 완료 상태 → completedId 출력
            if (QuestManager.Instance.HasCompleted(QuestId))
            {
                selectedId = completedId;
            }
            // 3. 퀘스트 진행 중 상태
            else if (QuestManager.Instance.HasAccepted(QuestId))
            {
                // 목표 달성 상태인지 확인
                if (QuestManager.Instance.IsObjectiveReached(QuestId))
                {
                    // 목표 달성 대사 출력
                    selectedId = objectiveId;

                    // 보고 NPC일 경우 퀘스트 완료 처리
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

                // 목표 대상 NPC에 닿으면 목표 달성 처리
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

        // 4. 대화 시작
        FindObjectOfType<DialogueManager>().StartDialogue(selectedId);
    }
}
