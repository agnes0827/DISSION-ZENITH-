using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Tooltip("�⺻ ��ȭ ID")]
    public string defaultId;

    [Header("����Ʈ ��ȭ ����")]

    [Tooltip("����Ʈ ���� ��")]
    public string acceptedId;
    [Tooltip("����Ʈ �Ϸ� ��")]
    public string completedId;

    public string QuestId;

    [Tooltip("�� NPC�� ���� ID (ex: Npc_choco)")]
    public string npcId;

    public void TriggerDialogue()
    {
        string selectedId = defaultId;

        if (!string.IsNullOrEmpty(QuestId))
        {
            if (QuestManager.Instance.HasCompleted(QuestId))
            {
                selectedId = completedId;
            }
            else if (QuestManager.Instance.HasAccepted(QuestId))
            {
                selectedId = acceptedId;
            }
        }

        FindObjectOfType<DialogueManager>().StartDialogue(selectedId);

        // ����Ʈ ���� ���� �˻�
        QuestManager.Instance.TryCompleteTalkToNPC(npcId);

    }
}
