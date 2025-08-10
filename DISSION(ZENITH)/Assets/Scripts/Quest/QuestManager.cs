using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    private HashSet<string> acceptedQuests = new HashSet<string>();
    private HashSet<string> completedQuests = new HashSet<string>();

    private QuestLoader questLoader;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �� ��ȯ���� ����
            questLoader = FindObjectOfType<QuestLoader>();  // QuestLoader ����
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AcceptQuest(string questId)
    {
        if (!acceptedQuests.Contains(questId))
        {
            acceptedQuests.Add(questId);

            // UI�� ����Ʈ ǥ��
            var quest = GetQuestById(questId);
            FindObjectOfType<QuestUI>()?.ShowQuest(quest);
        }

        // ����Ʈ ������ ����
        foreach (var icon in FindObjectsOfType<QuestIconUI>())
        {
            icon.UpdateIcon();
        }
    }

    public bool HasAccepted(string questId)
    {
        return acceptedQuests.Contains(questId);
    }

    // �Ϸ� ó��
    public void TryCompleteTalkToNPC(string npcId)
    {
        foreach (var questId in acceptedQuests)
        {
            Quest quest = GetQuestById(questId);
            if (quest != null && quest.type == "TalkToNPC" && quest.target_id == npcId)
            {
                Debug.Log($"[����Ʈ �Ϸ�] {quest.quest_title} - NPC�� ��ȭ �Ϸ�");
                CompleteQuest(questId);
                break; // �ϳ��� �Ϸ�
            }
        }
    }

    public void CompleteQuest(string questId)
    {
        if (acceptedQuests.Contains(questId) && !completedQuests.Contains(questId))
        {
            acceptedQuests.Remove(questId);
            completedQuests.Add(questId);

            var quest = GetQuestById(questId);
            GiveReward(quest.reward); // ���߿� ������ ���� ó��

            // UI �ݱ�
            var questUI = FindObjectOfType<QuestUI>();
            if (questUI != null && questUI.GetCurrentQuestId() == questId)
                questUI.Hide();
        }
    }

    public bool HasCompleted(string questId)
    {
        return completedQuests.Contains(questId);
    }

    public Quest GetQuestById(string questId)
    {
        return questLoader?.GetQuestById(questId);
    }

    private void GiveReward(string reward)
    {
        Debug.Log($"���� ����: {reward}");
    }
}
