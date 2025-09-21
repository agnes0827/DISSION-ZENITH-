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



    public void TriggerDialogue()
    {
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
