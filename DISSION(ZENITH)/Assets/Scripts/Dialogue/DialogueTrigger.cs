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

    [Tooltip("����Ʈ ���� ��")]
    public string acceptedId;

    [Tooltip("����Ʈ ��ǥ �޼� �� ���� ���")]
    public string objectiveId;

    [Tooltip("����Ʈ �Ϸ� ��")]
    public string completedId;

    public string QuestId;

    [Tooltip("�� NPC�� ����� NPC���� ����")]
    public bool isReportNpc = false;

    [Tooltip("�� NPC�� ���� ID (ex: Npc_choco)")]
    public string npcId;

    public void TriggerDialogue()
    {
        string selectedId = defaultId;

        // 1. ���� ����Ʈ �̿Ϸ� �� �� locked ��� ���
        if (!string.IsNullOrEmpty(prerequisiteQuestId) &&
            !QuestManager.Instance.HasCompleted(prerequisiteQuestId))
        {
            selectedId = lockedId;
        }
        // 2. ����Ʈ�� ������ ���
        else if (!string.IsNullOrEmpty(QuestId))
        {
            if (QuestManager.Instance.HasCompleted(QuestId))
            {
                // �̹� �Ϸ��� ��� �� �Ϸ� ���
                selectedId = completedId;
            }
            else if (QuestManager.Instance.HasAccepted(QuestId))
            {
                // ��ǥ �޼� ���¸� objective ��� ���
                if (QuestManager.Instance.IsObjectiveReached(QuestId))
                    selectedId = objectiveId;
                else
                    selectedId = acceptedId;
            }
        }

        // ��ȭ ����
        FindObjectOfType<DialogueManager>().StartDialogue(selectedId);

        // �̴ϰ��� ó��
        if (selectedId == "MINIGAME")
        {
            MiniGameManager.Instance.StartDustCleaning(gameObject);
        }

        // ����Ʈ �Ϸ� ó���� ���� NPC������ �ϵ��� ����
        if (isReportNpc && QuestManager.Instance.IsObjectiveReached(QuestId))
        {
            QuestManager.Instance.CompleteQuest(QuestId);
            Debug.Log($"[����Ʈ �Ϸ�] {QuestId}");
        }
    }
}
