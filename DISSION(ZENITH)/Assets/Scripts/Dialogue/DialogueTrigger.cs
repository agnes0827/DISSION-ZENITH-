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
    [Tooltip("����Ʈ ���� �� ����� ��ȭ ID (��ǥ �޼� ��)")]
    public string acceptedId;

    [Tooltip("����Ʈ ��ǥ �޼� �� ����� ��ȭ ID")]
    public string objectiveId;

    [Tooltip("����Ʈ �Ϸ� �� ����� ��ȭ ID")]
    public string completedId;

    [Tooltip("����Ʈ ID")]
    public string QuestId;

    [Tooltip("�� NPC�� ���� ID (ex: Npc_choco)")]
    public string npcId;

    [Tooltip("����Ʈ ���� NPC ����")]
    public bool isReportNpc = false;

    public void TriggerDialogue()
    {
        string selectedId = defaultId;

        // 1. ���� ����Ʈ �̿Ϸ� �� �� locked ��� ���
        if (!string.IsNullOrEmpty(prerequisiteQuestId) &&
            !QuestManager.Instance.HasCompleted(prerequisiteQuestId))
        {
            selectedId = lockedId;
        }
        else if (!string.IsNullOrEmpty(QuestId))
        {
            // 2. ����Ʈ �Ϸ� ���� �� completedId ���
            if (QuestManager.Instance.HasCompleted(QuestId))
            {
                selectedId = completedId;
            }
            // 3. ����Ʈ ���� �� ����
            else if (QuestManager.Instance.HasAccepted(QuestId))
            {
                // ��ǥ �޼� �������� Ȯ��
                if (QuestManager.Instance.IsObjectiveReached(QuestId))
                {
                    // ��ǥ �޼� ��� ���
                    selectedId = objectiveId;

                    // ���� NPC�� ��� ����Ʈ �Ϸ� ó��
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

                // ��ǥ ��� NPC�� ������ ��ǥ �޼� ó��
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

        // 4. ��ȭ ����
        FindObjectOfType<DialogueManager>().StartDialogue(selectedId);
    }
}
