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
            Debug.Log($"[����Ʈ ����] {questId}");
            acceptedQuests.Add(questId);

            // UI�� ����Ʈ ǥ��
            var quest = GetQuestById(questId);
            Debug.Log($"[����Ʈ ������ Ȯ��] ����: {quest?.quest_title}");

            FindObjectOfType<QuestUI>()?.ShowQuest(quest);
        }
    }

    public bool HasAccepted(string questId)
    {
        return acceptedQuests.Contains(questId);
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
        Debug.Log($"���� ����: {reward}");  // ���߿� �κ��丮�� ��� �ý��� ����
    }
}
