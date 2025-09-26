using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Tooltip("�� ����Ʈ�� �����ϱ� ���� ���� ����Ʈ ID")]
    public string prerequisiteQuestId;

    [Tooltip("���� ����Ʈ�� �̿Ϸ��� �� ������ ��ȭ ID")]
    public string lockedId;

    [Tooltip("�⺻ ��ȭ ID")]
    public string defaultId;

    [Header("����Ʈ ��ȭ ����")]
    public string acceptedId;
    public string objectiveId;
    public string completedId;

    public string QuestId;
    public string npcId;
    public bool isReportNpc = false;



    public void TriggerDialogue()
    {
        string selectedId = defaultId;

        // 1. ���� ����Ʈ �̿Ϸ� �� locked ��� ���
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

                    // �Ϸ� NPC��� �ٷ� ����Ʈ �Ϸ�
                    if (isReportNpc)
                    {
                        QuestManager.Instance.CompleteQuest(QuestId);
                        Debug.Log($"[����Ʈ �Ϸ�] Quest={QuestId}");
                    }
                }
                else
                {
                    selectedId = acceptedId;
                }

                // ��ǥ ��� NPC���� Ȯ�� �� ��ǥ �޼� ó��
                if (!string.IsNullOrEmpty(npcId))
                {
                    Quest quest = QuestManager.Instance.GetQuestById(QuestId);
                    if (quest != null && quest.target_id == npcId)
                    {
                        QuestManager.Instance.SetObjectiveReached(QuestId);
                        Debug.Log($"[��ǥ �޼� ó��] NPC={npcId}, Quest={QuestId}");
                    }
                }
            }
        }

        // 2. ��� ���
        DialogueManager.Instance.StartDialogue(selectedId, this.gameObject);
    }
}
