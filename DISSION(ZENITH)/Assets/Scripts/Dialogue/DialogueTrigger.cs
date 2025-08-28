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

    [Tooltip("퀘스트 진행 중")]
    public string acceptedId;

    [Tooltip("퀘스트 목표 달성 후 보고 대사")]
    public string objectiveId;

    [Tooltip("퀘스트 완료 후")]
    public string completedId;

    public string QuestId;

    [Tooltip("이 NPC가 보고용 NPC인지 여부")]
    public bool isReportNpc = false;

    [Tooltip("이 NPC의 고유 ID (ex: Npc_choco)")]
    public string npcId;

    public void TriggerDialogue()
    {
        string selectedId = defaultId;

        // 1. 선행 퀘스트 미완료 시 → locked 대사 출력
        if (!string.IsNullOrEmpty(prerequisiteQuestId) &&
            !QuestManager.Instance.HasCompleted(prerequisiteQuestId))
        {
            selectedId = lockedId;
        }
        // 2. 퀘스트가 설정된 경우
        else if (!string.IsNullOrEmpty(QuestId))
        {
            if (QuestManager.Instance.HasCompleted(QuestId))
            {
                // 이미 완료한 경우 → 완료 대사
                selectedId = completedId;
            }
            else if (QuestManager.Instance.HasAccepted(QuestId))
            {
                // 목표 달성 상태면 objective 대사 출력
                if (QuestManager.Instance.IsObjectiveReached(QuestId))
                    selectedId = objectiveId;
                else
                    selectedId = acceptedId;
            }
        }

        // 대화 시작
        FindObjectOfType<DialogueManager>().StartDialogue(selectedId);

        // 미니게임 처리
        if (selectedId == "MINIGAME")
        {
            MiniGameManager.Instance.StartDustCleaning(gameObject);
        }

        // 퀘스트 완료 처리를 보고 NPC에서만 하도록 제한
        if (isReportNpc && QuestManager.Instance.IsObjectiveReached(QuestId))
        {
            QuestManager.Instance.CompleteQuest(QuestId);
            Debug.Log($"[퀘스트 완료] {QuestId}");
        }
    }
}
