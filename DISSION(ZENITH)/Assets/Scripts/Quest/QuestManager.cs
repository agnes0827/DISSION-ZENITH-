using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    private HashSet<string> acceptedQuests = new HashSet<string>();          // ���� ���� ����Ʈ
    private HashSet<string> completedQuests = new HashSet<string>();         // �Ϸ��� ����Ʈ
    private HashSet<string> objectiveReachedQuests = new HashSet<string>();  // ��ǥ �޼������� �Ϸ� �� ����

    private QuestLoader questLoader;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            questLoader = FindObjectOfType<QuestLoader>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ����Ʈ ����
    public void AcceptQuest(string questId)
    {
        if (!acceptedQuests.Contains(questId))
        {
            acceptedQuests.Add(questId);

            // UI ǥ��
            var quest = GetQuestById(questId);
            FindObjectOfType<QuestUI>()?.ShowQuest(quest);

            Debug.Log($"[����Ʈ ����] {quest.quest_title}");
        }

        // ����Ʈ ������ ����
        foreach (var icon in FindObjectsOfType<QuestIconUI>())
        {
            icon.UpdateIcon();
        }
    }

    // ��ǥ �޼�

    public void SetObjectiveReached(string questId)
    {
        if (acceptedQuests.Contains(questId) && !objectiveReachedQuests.Contains(questId))
        {
            objectiveReachedQuests.Add(questId);
            Debug.Log($"[����Ʈ ��ǥ �޼�] {questId}");
        }
    }
    
    // NPC ��ȭ �� ����Ʈ �Ϸ� ���� Ȯ��
    public void TryCompleteTalkToNPC(string npcId)
    {
        foreach (var questId in acceptedQuests)
        {
            Quest quest = GetQuestById(questId);

            if (quest == null) continue;

            // ����Ʈ Ÿ���� TalkToNPC�̰� ����� npcId�� ��츸 ó��
            if (quest.type == "TalkToNPC" && quest.target_id == npcId)
            {
                // ��ǥ�� �޼��� ���¿��߸� �Ϸ� ó��
                if (objectiveReachedQuests.Contains(questId))
                {
                    Debug.Log($"[����Ʈ �Ϸ�] {quest.quest_title} - NPC���� ���� �Ϸ�");
                    CompleteQuest(questId);
                }
                else
                {
                    Debug.Log($"[����Ʈ ���� ��] {quest.quest_title} - ��ǥ�� ���� �޼����� ����");
                }
                break;
            }
        }
    }


    // ����Ʈ �Ϸ� ó��
    public void CompleteQuest(string questId)
    {
        if (acceptedQuests.Contains(questId) && !completedQuests.Contains(questId))
        {
            acceptedQuests.Remove(questId);
            objectiveReachedQuests.Remove(questId);
            completedQuests.Add(questId);

            var quest = GetQuestById(questId);
            GiveReward(quest.reward);

            // UI �ݱ�
            var questUI = FindObjectOfType<QuestUI>();
            if (questUI != null && questUI.GetCurrentQuestId() == questId)
                questUI.Hide();
        }
    }

    // ���� üũ �Լ�
    public bool HasAccepted(string questId)
    {
        return acceptedQuests.Contains(questId);
    }

    public bool HasCompleted(string questId)
    {
        return completedQuests.Contains(questId);
    }

    public bool IsObjectiveReached(string questId)
    {
        return objectiveReachedQuests.Contains(questId);
    }

    // ����Ʈ ������ �ε�
    public Quest GetQuestById(string questId)
    {
        return questLoader?.GetQuestById(questId);
    }

    // ���� ����
    private void GiveReward(string reward)
    {
        Debug.Log($"[����Ʈ ���� ����] {reward}");
    }
}
