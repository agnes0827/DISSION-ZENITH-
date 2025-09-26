using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;
    [SerializeField] private QuestLoader questLoader;

    // ����Ʈ ����
    private HashSet<string> acceptedQuests = new HashSet<string>();           // ���� ��
    private HashSet<string> completedQuests = new HashSet<string>();          // �Ϸ�
    private HashSet<string> objectiveReachedQuests = new HashSet<string>();   // ��ǥ �޼� (��ȭ ��)

    // UI ����
    private QuestUI questUI;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (questLoader != null)
            {
                questLoader.LoadQuestData();
            }
            else
            {
                Debug.LogError("QuestLoader�� ������� �ʾҽ��ϴ�!");
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegisterQuestUI(QuestUI ui)
    {
        questUI = ui;
        Debug.Log("QuestUI�� QuestManager�� ���������� ��ϵǾ����ϴ�.");
    }

    public void UnregisterQuestUI()
    {
        questUI = null;
        Debug.Log("QuestUI�� �ı��Ǿ� ��� �����Ǿ����ϴ�.");
    }

    // ����Ʈ ����
    public void AcceptQuest(string questId)
    {
        // �̹� �����߰ų� �Ϸ��� ����Ʈ�� �ٽ� �������� �ʽ��ϴ�.
        if (HasAccepted(questId) || HasCompleted(questId))
        {
            Debug.Log($"����Ʈ '{questId}'�� �̹� ó���� ����Ʈ�̹Ƿ� �ٽ� �������� �ʽ��ϴ�.");
            return;
        }

        acceptedQuests.Add(questId);
        Debug.Log($"[����Ʈ ����] ID: {questId}");

        // ��ϵ� UI�� �ִٸ�, UI�� ǥ���մϴ�.
        var quest = GetQuestById(questId);
        if (questUI != null && quest != null)
        {
            questUI.ShowQuest(quest);
        }

        UpdateQuestIcons();
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

            if (questUI != null && questUI.GetCurrentQuestId() == questId)
            {
                questUI.Hide();
            }

            UpdateQuestIcons();
            Debug.Log($"[����Ʈ �Ϸ�] ID: {questId}");
        }
    }

    // ��ǥ �޼�
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
