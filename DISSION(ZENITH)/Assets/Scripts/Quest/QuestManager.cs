using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    private HashSet<string> acceptedQuests = new HashSet<string>();           // ���� ��
    private HashSet<string> completedQuests = new HashSet<string>();          // �Ϸ�
    private HashSet<string> objectiveReachedQuests = new HashSet<string>();   // ��ǥ �޼� (��ȭ ��)

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
            UpdateQuestIcons();
        }
    }

    // ��ǥ �޼� (DialogueTrigger���� ���� ȣ��)
    public void SetObjectiveReached(string questId)
    {
        if (acceptedQuests.Contains(questId) && !objectiveReachedQuests.Contains(questId))
        {
            objectiveReachedQuests.Add(questId);
            Debug.Log($"[��ǥ �޼�] {questId}");
        }
    }

    // NPC���� ���� �� TalkToNPC ����Ʈ �Ϸ� �õ�
    public void TryCompleteTalkToNPC(string npcId)
    {
        foreach (var questId in acceptedQuests)
        {
            Quest quest = GetQuestById(questId);
            if (quest == null) continue;

            // TalkToNPC Ÿ���̰� ��ǥ ����̸� �Ϸ� ó��
            if (quest.Type == QuestType.HaveItem && quest.target_id == npcId)
            {
                if (objectiveReachedQuests.Contains(questId))
                {
                    CompleteQuest(questId);
                    Debug.Log($"[����Ʈ �Ϸ�] {quest.quest_title}");
                }
                break;
            }
        }
    }

    public void CheckQuestItem(string itemId)
    {
        foreach (var questId in new List<string>(acceptedQuests))
        {
            var quest = GetQuestById(questId);
            if (quest == null) continue;

            if (quest.Type == QuestType.HaveItem && quest.target_id == itemId)
            {
                SetObjectiveReached(questId);  
                CompleteQuest(questId);        
                Debug.Log($"[������ ���� ������Ϸ�] {quest.quest_id} - {quest.quest_title}");
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

            UpdateQuestIcons();
        }
    }

    // ���� üũ
    public bool HasAccepted(string questId) => acceptedQuests.Contains(questId);
    public bool HasCompleted(string questId) => completedQuests.Contains(questId);
    public bool IsObjectiveReached(string questId) => objectiveReachedQuests.Contains(questId);

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

    // ������ ����
    private void UpdateQuestIcons()
    {
        foreach (var icon in FindObjectsOfType<QuestIconUI>())
        {
            icon.UpdateIcon();
        }
    }
}
